using Logging;
using Medical;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Anomalous.Medical.StoreManager.Controller
{
    class UploadPluginController
    {
        private StandaloneController controller;
        AnomalousMvcContext context;
        EditPluginDetailsController editPluginDetails;
        DDAtlasPlugin plugin;

        public UploadPluginController(StandaloneController standaloneController, DDAtlasPlugin plugin)
        {
            this.controller = standaloneController;
            this.plugin = plugin;
        }

        public void showContext()
        {
            EmbeddedResourceProvider embeddedResourceProvider = new EmbeddedResourceProvider(Assembly.GetExecutingAssembly(), "Anomalous.Medical.StoreManager.MvcContexts.UploadPlugin.");
            controller.TimelineController.setResourceProvider(embeddedResourceProvider);

            //Load and run the mvc context
            context = controller.MvcCore.loadContext(embeddedResourceProvider.openFile("MvcContext.mvc"));
            context.RuntimeName = "UploadPlugin";
            context.setResourceProvider(embeddedResourceProvider);
            //DataModel pluginDetailsModel = (DataModel)context.Models["PluginDetails"];

            editPluginDetails = new EditPluginDetailsController(context, plugin);

            controller.MvcCore.startRunningContext(context);
        }
    }
}
