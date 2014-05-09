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
        ImageBox rmlImage;
        RocketWidget rocketWidget;
        ResourceProvider resourceProvider;
        ResourceProviderRocketFSExtension resourceProviderRocketFSExtension;

        public BuyScreen(ResourceProvider resourceProvider, GUIManager guiManager)
            : base("Medical.GUI.BuyScreen.BuyScreen.layout", guiManager)
        {
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

            if (rocketWidget.Context.ZoomLevel != newScale)
            {
                rocketWidget.Context.ZoomLevel = newScale;
                foreach (ElementDocument document in rocketWidget.Context.Documents)
                {
                    document.MakeDirtyForScaleChange();
                }
                rocketWidget.renderOnNextFrame();
            }

            base.layoutUpdated();
        }

        public void setFile(String file)
        {
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
        }
    }
}
