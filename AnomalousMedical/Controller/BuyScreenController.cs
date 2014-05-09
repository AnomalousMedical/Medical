using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Medical.Controller.AnomalousMvc;
using System.Threading;
using Engine;
using Logging;
using MyGUIPlugin;
using System.Net;
using libRocketPlugin;
using Medical.GUI;

namespace Medical.Controller
{
    enum BuyScreens
    {
        AnatomyFinder,
        Bookmarks,
        SelectionMode
    }

    class BuyScreenController : IDisposable
    {
        StandaloneController controller;

        private List<BuyScreen> openScreens = new List<BuyScreen>();

        public BuyScreenController(StandaloneController controller)
        {
            this.controller = controller;
        }

        public void Dispose()
        {
            foreach(BuyScreen screen in openScreens)
            {
                screen.Dispose();
            }
            openScreens.Clear();
        }

        public void showScreen(BuyScreens screen)
        {
            VirtualFilesystemResourceProvider embeddedResourceProvider = new VirtualFilesystemResourceProvider("BuyScreens");

            BuyScreen buyScreen = new BuyScreen(embeddedResourceProvider, controller.GUIManager);
            buyScreen.setFile(String.Format("{0}/Index.rml", screen));
            buyScreen.Hidden += (sender, e) =>
                {
                    openScreens.Remove(buyScreen);
                    buyScreen.Dispose();
                };
            buyScreen.show(100, 100);
            openScreens.Add(buyScreen);
        }
    }
}
