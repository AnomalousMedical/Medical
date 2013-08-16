using Anomalous.Medical.StoreManager.Config;
using Anomalous.Medical.StoreManager.Models;
using Anomalous.Medical.StoreManager.Util;
using Medical;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
using Medical.GUI;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Anomalous.Medical.StoreManager.Controller
{
    class EditPluginDetailsController
    {
        private const String IconSourceFile = "Resources/IconSource.png";

        private bool allowSavePluginDetails = true;

        public EditPluginDetailsController(AnomalousMvcContext context, LicenseManager licenseManager, PluginCreationTool tool, ResourceProvider resourceProvider)
        {
            ((RunCommandsAction)context.Controllers["EditPluginDetails"].Actions["Opening"]).addCommand(new CallbackCommand((executingContext) =>
            {
                setImage(resourceProvider, executingContext);
            }));


            ((RunCommandsAction)context.Controllers["EditPluginDetails"].Actions["BrowseImage"]).addCommand(new CallbackCommand((executingContext) =>
            {
                FileOpenDialog openDialog = new FileOpenDialog(MainWindow.Instance, "Choose Image", wildcard: "Images|*");
                openDialog.showModal((result, paths) =>
                {
                    if (result == NativeDialogResult.OK)
                    {
                        String path = paths.First();
                        String extension = Path.GetExtension(path);
                        if (extension.Equals(".png", StringComparison.InvariantCultureIgnoreCase) || extension.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase) || extension.Equals(".jpeg", StringComparison.InvariantCultureIgnoreCase))
                        {
                            using (Bitmap source = (Bitmap)Bitmap.FromFile(path))
                            {
                                String iconPath = Path.GetDirectoryName(IconSourceFile);
                                if (!resourceProvider.exists(iconPath))
                                {
                                    resourceProvider.createDirectory("", iconPath);
                                }
                                using (Stream stream = resourceProvider.openWriteStream(IconSourceFile))
                                {
                                    LogoUtil.SaveResizedImage(source, stream, 100, 100);
                                }
                            }
                            setImage(resourceProvider, executingContext);
                        }
                        else
                        {
                            MessageBox.show(String.Format("Cannot open a file with extension '{0}'. Please choose a file that is a Png Image (.png) or a Jpeg (.jpg or .jpeg).", extension), "Can't Load Image", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
                        }
                    }
                });
            }));

            ((RunCommandsAction)context.Controllers["EditPluginDetails"].Actions["SavePluginDetails"]).addCommand(new CallbackCommand((executingContext) =>
            {
                if (allowSavePluginDetails)
                {
                    allowSavePluginDetails = false;
                    bool submitToServer = true;
                    String errorMessage = "";

                    ViewHost viewHost = executingContext.RunningActionViewHost;
                    ViewHostControl message = viewHost.findControl("Message");
                    ViewHostControl error = viewHost.findControl("Error");

                    message.Visible = true;
                    error.Visible = false;

                    DataModel model = context.getModel<DataModel>("PluginDetails");
                    String storeUniqueName = model.getValue("StoreUniqueName");
                    String pluginUniqueName = model.getValue("PluginUniqueName");
                    String title = model.getValue("Title");
                    String ownsCopyright = model.getValue("CopyrightAgree");
                    String mode = model.getValue("Mode");

                    if (ownsCopyright != "True")
                    {
                        submitToServer = false;
                        errorMessage = "You must assert that you own the copyright to all content in this plugin.";
                    }

                    if (String.IsNullOrWhiteSpace(title))
                    {
                        submitToServer = false;
                        errorMessage = "You must enter a title for this plugin.";
                    }

                    CredentialServerConnection serverConnection = null;
                    bool savePluginUniqueName = false;
                    switch (mode)
                    {
                        case "Create":
                            serverConnection = new CredentialServerConnection(StoreManagerConfig.CreatePluginUrl(storeUniqueName), licenseManager.User, licenseManager.MachinePassword);
                            serverConnection.addArgument("Title", title);
                            serverConnection.addArgument("OwnsCopyright", "True");
                            serverConnection.addArgument("Tool", tool.ToString());
                            savePluginUniqueName = true;
                            break;

                        case "Update":
                            serverConnection = new CredentialServerConnection(StoreManagerConfig.UpdatePluginUrl(storeUniqueName), licenseManager.User, licenseManager.MachinePassword);
                            serverConnection.addArgument("Title", title);
                            serverConnection.addArgument("OwnsCopyright", "True");
                            serverConnection.addArgument("Tool", tool.ToString());
                            serverConnection.addArgument("PluginUniqueName", pluginUniqueName);
                            break;

                        default:
                            submitToServer = false;
                            errorMessage = "Unsupported mode";
                            break;
                    }

                    if (submitToServer)
                    {
                        ThreadPool.QueueUserWorkItem(arg =>
                            {
                                ResponseModel response = null;
                                try
                                {
                                    response = serverConnection.makeRequestSaveableResponse() as ResponseModel;
                                }
                                catch (Exception ex)
                                {
                                    Logging.Log.Error("{0} occured when trying to get the list of user stores. Message: {1}", ex.GetType().Name, ex.Message);
                                }
                                ThreadManager.invoke(() =>
                                    {
                                        //Need to check that ui is still active
                                        if (response != null)
                                        {
                                            if (response.Success)
                                            {
                                                if (savePluginUniqueName)
                                                {
                                                    model.setValue("PluginUniqueName", response.Message);
                                                }
                                                message.Visible = false;
                                                error.Visible = false;
                                                allowSavePluginDetails = true;
                                                context.runAction("EditPluginDetails/FinishedSaving");
                                            }
                                            else
                                            {
                                                setError(executingContext, response.Message, message, error);
                                            }
                                        }
                                        else
                                        {
                                            setError(executingContext, "", message, error);
                                        }
                                    });
                            });
                    }
                    else
                    {
                        setError(executingContext, errorMessage, message, error);
                    }
                }
            }));
        }

        private static void setImage(ResourceProvider resourceProvider, AnomalousMvcContext executingContext)
        {
            ViewHostControl imageDiv = executingContext.RunningActionViewHost.findControl("ImageDiv");
            if (resourceProvider.exists(IconSourceFile))
            {
                imageDiv.Value = "";
                //Delaying this by a frame forces it to reload
                ThreadManager.invoke(() =>
                {
                    imageDiv.Value = String.Format("<img id={0} src=\"{1}\"/>", Guid.NewGuid().ToString(), IconSourceFile);
                });
            }
        }

        private void setError(AnomalousMvcContext executingContext, String errorMessage, ViewHostControl message, ViewHostControl error)
        {
            message.Visible = false;
            error.Visible = true;
            ViewHostControl errorOutput = executingContext.RunningActionViewHost.findControl("ErrorOutput");
            errorOutput.Value = errorMessage;
            allowSavePluginDetails = true;
        }
    }
}
