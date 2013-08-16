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
                ViewHost viewHost = executingContext.RunningActionViewHost;
                ViewHostControl progressMessage = viewHost.findControl("ProgressMessage");
                ViewHostControl pluginSelection = viewHost.findControl("PluginSelection");
                ViewHostControl errorMessage = viewHost.findControl("ErrorMessage");
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
                        plugins = serverConnection.makeRequestSaveableResponse() as StorePluginsModel;
                    }
                    catch (Exception ex)
                    {
                        Logging.Log.Error("{0} occured when trying to get the list of user stores. Message: {1}", ex.GetType().Name, ex.Message);
                    }
                    ThreadManager.invoke(() =>
                    {
                        if (viewHost.Open)
                        {
                            if (plugins != null)
                            {
                                progressMessage.Visible = false;
                                pluginSelection.Visible = true;
                                ViewHostControl formElement = viewHost.findControl("PluginSelectionForms");
                                String formatString = formElement.Value;
                                StringBuilder sb = new StringBuilder();
                                foreach (var plugin in plugins.Plugins)
                                {
                                    sb.AppendFormat(formatString, plugin.UniqueName, plugin.Name, plugin.Version);
                                }
                                formElement.Value = sb.ToString();
                            }
                            else
                            {
                                progressMessage.Visible = false;
                                errorMessage.Visible = true;
                            }
                        }

                        //Set fetch allow back to true.
                        allowGet = true;
                    });
                });
            }
        }
    }
}
