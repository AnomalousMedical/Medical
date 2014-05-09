using Engine;
using libRocketPlugin;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public class BuyScreen : AbstractFullscreenGUIPopup
    {
        private ImageBox rmlImage;
        private RocketWidget rocketWidget;
        private ResourceProvider resourceProvider;
        private ResourceProviderRocketFSExtension resourceProviderRocketFSExtension;
        private DelegateRocketEventController eventController;

        public BuyScreen(ResourceProvider resourceProvider, GUIManager guiManager)
            : base("Medical.GUI.BuyScreen.BuyScreen.layout", guiManager)
        {
            eventController = new DelegateRocketEventController();
            eventController.addHandler("close", evt => this.hide());
            eventController.addHandler("visitAnomalousPage", visitAnomalousPage);

            this.resourceProvider = resourceProvider;

            resourceProviderRocketFSExtension = new ResourceProviderRocketFSExtension(resourceProvider);
            RocketInterface.Instance.SystemInterface.AddRootPath(resourceProvider.BackingLocation);
            RocketInterface.Instance.FileInterface.addExtension(resourceProviderRocketFSExtension);

            rmlImage = (ImageBox)widget.findWidget("RmlImage");
            rocketWidget = new RocketWidget(rmlImage, false);
        }

        public override void Dispose()
        {
            RocketInterface.Instance.FileInterface.removeExtension(resourceProviderRocketFSExtension);
            RocketInterface.Instance.SystemInterface.RemoveRootPath(resourceProvider.BackingLocation);
            rocketWidget.Dispose();
            base.Dispose();
        }

        protected override void layoutUpdated()
        {
            rocketWidget.resized();

            float newScale = widget.Height / (float)Slideshow.BaseSlideScale;
            rocketWidget.setScale(newScale);

            base.layoutUpdated();
        }

        public void setFile(String file)
        {
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
