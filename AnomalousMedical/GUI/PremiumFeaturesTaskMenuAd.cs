using Engine;
using libRocketPlugin;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class PremiumFeaturesTaskMenuAd : TaskMenuAdProvider, IDisposable
    {
        TaskMenu taskMenu;
        ImageBox adImage;
        RocketWidget rocketWidget;

        private static readonly int AdWidth = ScaleHelper.Scaled(230);
        private static readonly int AdHeight = ScaleHelper.Scaled(460);

        public PremiumFeaturesTaskMenuAd(TaskMenu taskMenu)
        {
            this.taskMenu = taskMenu;
            taskMenu.AdProvider = this;
            taskMenu.Showing += taskMenu_Showing;
        }

        public void Dispose()
        {
            taskMenu.Showing -= taskMenu_Showing;
            if (adImage != null)
            {
                rocketWidget.Dispose();
                fireAdDestroyed();
                Gui.Instance.destroyWidget(adImage);
            }
            if (taskMenu.AdProvider == this)
            {
                taskMenu.AdProvider = null;
            }
        }

        void taskMenu_Showing(object sender, EventArgs e)
        {
            if (adImage == null)
            {
                adImage = (ImageBox)ParentWidget.createWidgetT("ImageBox", "ImageBox", 2, taskMenu.AdTop, AdWidth, AdHeight, Align.Left | Align.Top, "");
                rocketWidget = new RocketWidget(adImage, false);
                openRml();
                Right = adImage.Right;
                fireAdCreated();
                taskMenu.Showing -= taskMenu_Showing;
            }
        }

        private void openRml()
        {
            VirtualFilesystemResourceProvider resourceProvider = new VirtualFilesystemResourceProvider("BuyScreens");
            String file = "TaskMenuAd/Index.rml";
            if (resourceProvider.exists(file))
            {
                ResourceProviderRocketFSExtension resourceProviderRocketFSExtension = new ResourceProviderRocketFSExtension(resourceProvider);
                RocketInterface.Instance.SystemInterface.AddRootPath(resourceProvider.BackingLocation);
                RocketInterface.Instance.FileInterface.addExtension(resourceProviderRocketFSExtension);

                DelegateRocketEventController eventController = new DelegateRocketEventController();
                eventController.addHandler("visitAnomalousPage", visitAnomalousPage);
                RocketEventListenerInstancer.setEventController(eventController);

                RocketGuiManager.clearAllCaches();
                rocketWidget.Context.UnloadAllDocuments();

                using (ElementDocument document = rocketWidget.Context.LoadDocument(resourceProvider.getFullFilePath(file)))
                {
                    if (document != null)
                    {
                        document.Show();
                        rocketWidget.removeFocus();
                        rocketWidget.renderOnNextFrame();
                    }
                }

                RocketEventListenerInstancer.resetEventController();
                RocketInterface.Instance.FileInterface.removeExtension(resourceProviderRocketFSExtension);
                RocketInterface.Instance.SystemInterface.RemoveRootPath(resourceProvider.BackingLocation);
            }
        }

        private static void visitAnomalousPage(Event evt)
        {
            Variant url = evt.TargetElement.GetAttribute("url");
            if (url != null)
            {
                OtherProcessManager.openUrlInBrowser(String.Format("{0}/{1}", MedicalConfig.WebsiteHostUrl, url.StringValue));
            }
        }
    }
}
