using Anomalous.Medical.StoreManager.Config;
using Anomalous.Medical.StoreManager.Models;
using Medical;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Anomalous.Medical.StoreManager.Controller
{
    class ChoosePluginController
    {
        private LicenseManager licenseManager;
        private bool allowGet = true;

        public ChoosePluginController(AnomalousMvcContext context, LicenseManager licenseManager)
        {
            MvcController controller = context.Controllers["ChoosePlugin"];
            this.licenseManager = licenseManager;

            ((RunCommandsAction)controller.Actions["GetPlugins"]).addCommand(new CallbackCommand((executingContext) =>
            {
                getPlugins(executingContext);
            }));
        }

        private void getPlugins(AnomalousMvcContext executingContext)
        {
            if (allowGet)
            {
                allowGet = false;
                ViewHostControl progressMessage = executingContext.RunningActionViewHost.findControl("ProgressMessage");
                ViewHostControl pluginSelection = executingContext.RunningActionViewHost.findControl("PluginSelection");
                ViewHostControl errorMessage = executingContext.RunningActionViewHost.findControl("ErrorMessage");
                progressMessage.Visible = true;
                pluginSelection.Visible = false;
                errorMessage.Visible = false;

                DataModel model = executingContext.getModel<DataModel>("PluginDetails");
                String storeUniqueName = model.getValue("StoreUniqueName");

                ThreadPool.QueueUserWorkItem((stateInfo) =>
                {
                    StorePluginsModel plugins = null;
                    try
                    {
                        CredentialServerConnection serverConnection = new CredentialServerConnection(StoreManagerConfig.GetStorePluginsUrl(storeUniqueName), licenseManager.User, licenseManager.MachinePassword);
                        serverConnection.makeRequestSaveableResponse(saveable =>
                        {
                            plugins = saveable as StorePluginsModel;
                        });
                    }
                    catch (Exception ex)
                    {
                        Logging.Log.Error("{0} occured when trying to get the list of user stores. Message: {1}", ex.GetType().Name, ex.Message);
                    }
                    ThreadManager.invoke(() =>
                    {
                        //Need to check that this context is still active
                        if (plugins != null)
                        {
                            progressMessage.Visible = false;
                            pluginSelection.Visible = true;
                            ViewHostControl formElement = executingContext.RunningActionViewHost.findControl("PluginSelectionForms");
                            String formatString = formElement.Value;
                            StringBuilder sb = new StringBuilder();
                            foreach (var plugin in plugins.Plugins)
                            {
                                sb.AppendFormat(formatString, plugin.UniqueName, plugin.Name);
                            }
                            formElement.Value = sb.ToString();
                        }
                        else
                        {
                            progressMessage.Visible = false;
                            errorMessage.Visible = true;
                        }

                        //Set fetch allow back to true.
                        allowGet = true;
                    });
                });
            }
        }
    }
}
