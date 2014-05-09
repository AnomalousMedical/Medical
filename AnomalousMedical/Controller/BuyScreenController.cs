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
            VirtualFilesystemResourceProvider resourceProvider = new VirtualFilesystemResourceProvider("BuyScreens");

            String file = String.Format("{0}/Index.rml", screen);
            if (resourceProvider.exists(file))
            {
                BuyScreen buyScreen = new BuyScreen(resourceProvider, controller.GUIManager);
                buyScreen.setFile(file);
                buyScreen.Hidden += (sender, e) =>
                    {
                        openScreens.Remove(buyScreen);
                        buyScreen.Dispose();
                    };
                buyScreen.show(0, 0);
                openScreens.Add(buyScreen);
            }
            else
            {
                MessageBox.show(String.Format("Cannot find buy screen '{0}'", file), "Missing Buy Screen File", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }
    }
}
