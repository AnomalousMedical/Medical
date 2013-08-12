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

        public UploadPluginController(StandaloneController standaloneController)
        {
            this.controller = standaloneController;
        }

        public void showContext(String pluginSourcePath, PluginCreationTool tool)
        {
            ResourceProvider resourceProvider = new StoreManagerResourceProvider(new EmbeddedResourceProvider(Assembly.GetExecutingAssembly(), "Anomalous.Medical.StoreManager.MvcContexts.UploadPlugin."), new FilesystemResourceProvider(pluginSourcePath));
            controller.TimelineController.setResourceProvider(resourceProvider);

            //Load and run the mvc context
            context = controller.MvcCore.loadContext(resourceProvider.openFile("MvcContext.mvc"));
            context.RuntimeName = "UploadPlugin";
            context.setResourceProvider(resourceProvider);

            indexController = new IndexController(context, controller.App.LicenseManager);
            chooseStore = new ChooseStoreController(context, controller.App.LicenseManager);
            editPluginDetails = new EditPluginDetailsController(context, controller.App.LicenseManager, tool, resourceProvider);
            choosePlugin = new ChoosePluginController(context, controller.App.LicenseManager);
            transmitFile = new TransmitFileController(context, controller.App.LicenseManager, pluginSourcePath);
            uploadComplete = new UploadCompleteController(context);

            controller.MvcCore.startRunningContext(context);
        }
    }
}
