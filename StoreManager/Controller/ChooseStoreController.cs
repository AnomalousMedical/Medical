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
    class ChooseStoreController
    {
        public ChooseStoreController(AnomalousMvcContext context, LicenseManager licenseManager)
        {
            MvcController controller = context.Controllers["ChooseStore"];

            ((RunCommandsAction)controller.Actions["Opening"]).addCommand(new CallbackCommand((executingContext) =>
            {
                //messageControl = executingContext.RunningActionViewHost.findControl("Message");
                //errorControl = executingContext.RunningActionViewHost.findControl("Error");
                ThreadPool.QueueUserWorkItem((stateInfo) =>
                    {
                        CredentialServerConnection serverConnection = new CredentialServerConnection(StoreManagerConfig.StoreListUrl, licenseManager.User, licenseManager.MachinePassword);
                        UserStoresModel userStores = null;
                        serverConnection.makeRequestSaveableResponse(saveable =>
                            {
                                userStores = saveable as UserStoresModel;
                            });
                        ThreadManager.invoke(() =>
                            {
                                //Need to check that this context is still active
                                if (userStores != null)
                                {
                                    ViewHostControl progressMessage = executingContext.RunningActionViewHost.findControl("ProgressMessage");
                                    progressMessage.Visible = false;
                                    ViewHostControl storeSelection = executingContext.RunningActionViewHost.findControl("StoreSelection");
                                    storeSelection.Visible = true;
                                    ViewHostControl formElement = executingContext.RunningActionViewHost.findControl("StoreSelectionForms");
                                    String formatString = formElement.Value;
                                    StringBuilder sb = new StringBuilder();
                                    foreach (var store in userStores.Stores)
                                    {
                                        sb.AppendFormat(formatString, store.UniqueName, store.Name);
                                    }
                                    formElement.Value = sb.ToString();
                                }
                                else
                                {
                                    ViewHostControl progressMessage = executingContext.RunningActionViewHost.findControl("ProgressMessage");
                                    progressMessage.Value = "TEMPORARY, could not contact server";
                                }
                            });
                    });
            }));
        }
    }
}
