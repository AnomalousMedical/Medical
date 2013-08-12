using Anomalous.Medical.StoreManager.Models;
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
        IndexController indexController;
        EditPluginDetailsController editPluginDetails;
        ChooseStoreController chooseStore;
        ChoosePluginController choosePlugin;
        TransmitFileController transmitFile;
        UploadCompleteController uploadComplete;
        DDAtlasPlugin plugin;

        public UploadPluginController(StandaloneController standaloneController, DDAtlasPlugin plugin)
        {
            this.controller = standaloneController;
            this.plugin = plugin;
        }

        public void showContext(String pluginSourcePath, PluginCreationTool tool)
        {
            EmbeddedResourceProvider embeddedResourceProvider = new EmbeddedResourceProvider(Assembly.GetExecutingAssembly(), "Anomalous.Medical.StoreManager.MvcContexts.UploadPlugin.");
            controller.TimelineController.setResourceProvider(embeddedResourceProvider);

            //Load and run the mvc context
            context = controller.MvcCore.loadContext(embeddedResourceProvider.openFile("MvcContext.mvc"));
            context.RuntimeName = "UploadPlugin";
            context.setResourceProvider(embeddedResourceProvider);

            indexController = new IndexController(context, controller.App.LicenseManager);
            chooseStore = new ChooseStoreController(context, controller.App.LicenseManager);
            editPluginDetails = new EditPluginDetailsController(context, controller.App.LicenseManager, tool);
            choosePlugin = new ChoosePluginController(context, controller.App.LicenseManager);
            transmitFile = new TransmitFileController(context, controller.App.LicenseManager, pluginSourcePath);
            uploadComplete = new UploadCompleteController(context);

            controller.MvcCore.startRunningContext(context);
        }
    }
}
