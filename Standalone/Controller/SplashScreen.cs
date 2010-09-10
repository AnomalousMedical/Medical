using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgrePlugin;
using MyGUIPlugin;
using OgreWrapper;
using Engine;

namespace Medical.Controller
{
    class SplashScreen : PopupContainer, IDisposable
    {
        Layout layout;
        Widget mainWidget;
        Progress progressBar;
        OgreWindow ogreWindow;
        StaticText statusText;

        public SplashScreen(OgreWindow ogreWindow, uint progressRange)
        {
            this.ogreWindow = ogreWindow;

            Gui gui = Gui.Instance;
            OgreResourceGroupManager.getInstance().addResourceLocation("GUI/PiperJBO/SplashScreen", "EngineArchive", "MyGUI", true);
            gui.load("SplashScreen.xml");
            layout = LayoutManager.Instance.loadLayout("SplashScreen.layout");
            mainWidget = layout.getWidget(0);
            mainWidget.setPosition(0, 0);
            mainWidget.setSize(gui.getViewWidth(), gui.getViewHeight());

            progressBar = mainWidget.findWidget("SplashScreen/ProgressBar") as Progress;
            progressBar.Range = progressRange;

            statusText = mainWidget.findWidget("SplashScreen/Status") as StaticText;
            statusText.TextColor = Color.White;

            ogreWindow.OgreRenderWindow.update();

            initialize(mainWidget);

            this.Hidden += new EventHandler(SplashScreen_Hidden);
        }

        public void Dispose()
        {
            LayoutManager.Instance.unloadLayout(layout);
        }

        public void updateStatus(uint position, String status)
        {
            progressBar.Position = position;
            statusText.Caption = status;
            ogreWindow.OgreRenderWindow.update();
            WindowFunctions.pumpMessages();
        }

        void SplashScreen_Hidden(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
