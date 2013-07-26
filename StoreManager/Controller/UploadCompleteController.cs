using Anomalous.Medical.StoreManager.Config;
using Medical;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anomalous.Medical.StoreManager.Controller
{
    class UploadCompleteController
    {
        public UploadCompleteController(AnomalousMvcContext context)
        {
            MvcController controller = context.Controllers["UploadComplete"];

            ((RunCommandsAction)controller.Actions["VisitDashboard"]).addCommand(new CallbackCommand((executingContext) =>
            {
                DataModel model = context.getModel<DataModel>("PluginDetails");
                String storeUniqueName = model.getValue("StoreUniqueName");

                OtherProcessManager.openUrlInBrowser(StoreManagerConfig.GetStoreDashboardPluginsUrl(storeUniqueName));
            }));
        }
    }
}
