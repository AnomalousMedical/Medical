using Engine;
using libRocketPlugin;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public class BuyScreen : MDIDialog
    {
        ImageBox rmlImage;
        RocketWidget rocketWidget;
        ResourceProvider resourceProvider;
        ResourceProviderRocketFSExtension resourceProviderRocketFSExtension;

        public BuyScreen(ResourceProvider resourceProvider)
            : base("Medical.GUI.BuyScreen.BuyScreen.layout")
        {
            this.resourceProvider = resourceProvider;

            resourceProviderRocketFSExtension = new ResourceProviderRocketFSExtension(resourceProvider);
            RocketInterface.Instance.SystemInterface.AddRootPath(resourceProvider.BackingLocation);
            RocketInterface.Instance.FileInterface.addExtension(resourceProviderRocketFSExtension);

            rmlImage = (ImageBox)window.findWidget("RmlImage");
            rocketWidget = new RocketWidget(rmlImage, false);

            window.WindowChangedCoord += window_WindowChangedCoord;
        }

        public override void Dispose()
        {
            RocketInterface.Instance.FileInterface.removeExtension(resourceProviderRocketFSExtension);
            RocketInterface.Instance.SystemInterface.RemoveRootPath(resourceProvider.BackingLocation);
            rocketWidget.Dispose();
            base.Dispose();
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

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            rocketWidget.resized();
        }
    }
}
