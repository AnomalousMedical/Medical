using Anomalous.Medical.StoreManager.Config;
using Anomalous.Medical.StoreManager.Models;
using Medical;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Anomalous.Medical.StoreManager.Controller
{
    class EditPluginDetailsController
    {
        private bool allowSavePluginDetails = true;

        public EditPluginDetailsController(AnomalousMvcContext context, LicenseManager licenseManager)
        {
            ((RunCommandsAction)context.Controllers["EditPluginDetails"].Actions["SavePluginDetails"]).addCommand(new CallbackCommand((executingContext) =>
            {
                if (allowSavePluginDetails)
                {
                    allowSavePluginDetails = false;
                    bool submitToServer = true;
                    String errorMessage = "";

                    ViewHostControl message = executingContext.RunningActionViewHost.findControl("Message");
                    ViewHostControl error = executingContext.RunningActionViewHost.findControl("Error");

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
                            savePluginUniqueName = true;
                            break;

                        case "Update":
                            serverConnection = new CredentialServerConnection(StoreManagerConfig.UpdatePluginUrl(storeUniqueName), licenseManager.User, licenseManager.MachinePassword);
                            serverConnection.addArgument("Title", title);
                            serverConnection.addArgument("OwnsCopyright", "True");
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
