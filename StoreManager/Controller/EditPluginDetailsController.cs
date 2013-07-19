using Medical;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anomalous.Medical.StoreManager.Controller
{
    class EditPluginDetailsController
    {
        private bool beginningUpload = false;
        private ViewHostControl messageControl;
        private ViewHostControl errorControl;

        public EditPluginDetailsController(AnomalousMvcContext context, DDAtlasPlugin plugin)
        {
            ((RunCommandsAction)context.Controllers["EditPluginDetails"].Actions["BeginUpload"]).addCommand(new CallbackCommand((executingContext) =>
            {
                if (!beginningUpload)
                {
                    beginningUpload = true;
                    DataModel model = context.getModel<DataModel>("PluginDetails");
                    if (model.getValue("CopyrightAgree") == "True")
                    {
                        messageControl.Value = "Connecting to server.";
                        errorControl.Value = "";
                        //ThreadPool.QueueUserWorkItem(new WaitCallback(getLicense), new Pair<String, String>(model.getValue("User"), model.getValue("Pass")));
                    }
                    else
                    {
                        beginningUpload = false;
                        messageControl.Value = "";
                        errorControl.Value = "You must assert that you own the copyright to the content you are uploading.";
                    }
                }
            }));

            ((RunCommandsAction)context.Controllers["EditPluginDetails"].Actions["Opening"]).addCommand(new CallbackCommand((executingContext) =>
            {
                messageControl = executingContext.RunningActionViewHost.findControl("Message");
                errorControl = executingContext.RunningActionViewHost.findControl("Error");
            }));
        }
    }
}
