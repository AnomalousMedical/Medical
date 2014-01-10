using Anomalous.Medical.StoreManager.Models;
using libRocketPlugin;
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

        public UploadPluginController(StandaloneController standaloneController)
        {
            this.controller = standaloneController;
        }

        public void showContext(ResourceProvider projectResourceProvider, PluginCreationTool tool)
        {
            EmbeddedResourceProvider embeddedResources = new EmbeddedResourceProvider(Assembly.GetExecutingAssembly(), "Anomalous.Medical.StoreManager.MvcContexts.UploadPlugin.");

            ResourceProviderRocketFSExtension rocketEmbeddedResources = new ResourceProviderRocketFSExtension(embeddedResources);
            ResourceProviderRocketFSExtension rocketProjectResources = new ResourceProviderRocketFSExtension(projectResourceProvider);

            //Load and run the mvc context
            context = controller.MvcCore.loadContext(embeddedResources.openFile("MvcContext.mvc"));
            context.RuntimeName = "UploadPlugin";
            context.setResourceProvider(embeddedResources);
            controller.TimelineController.setResourceProvider(embeddedResources);

            context.Started += (ctx) =>
            {
                RocketInterface.Instance.FileInterface.addExtension(rocketEmbeddedResources);
                RocketInterface.Instance.FileInterface.addExtension(rocketProjectResources);
            };

            context.RemovedFromStack += (ctx) =>
            {
                RocketInterface.Instance.FileInterface.removeExtension(rocketEmbeddedResources);
                RocketInterface.Instance.FileInterface.removeExtension(rocketProjectResources);
            };

            indexController = new IndexController(context, controller.App.LicenseManager);
            chooseStore = new ChooseStoreController(context, controller.App.LicenseManager);
            editPluginDetails = new EditPluginDetailsController(context, controller.App.LicenseManager, tool, projectResourceProvider);
            choosePlugin = new ChoosePluginController(context, controller.App.LicenseManager);
            transmitFile = new TransmitFileController(context, controller.App.LicenseManager, projectResourceProvider);
            uploadComplete = new UploadCompleteController(context);

            controller.MvcCore.startRunningContext(context);
        }
    }
}
