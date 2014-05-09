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
                controller.GUIManager.removeManagedDialog(screen);
                screen.Dispose();
            }
            openScreens.Clear();
        }

        public void showScreen(BuyScreens screen)
        {
            EmbeddedResourceProvider embeddedResourceProvider = new EmbeddedResourceProvider(Assembly.GetExecutingAssembly(), "Medical.MvcContexts.BuyScreens.");

            BuyScreen buyScreen = new BuyScreen(embeddedResourceProvider);
            buyScreen.setFile(String.Format("{0}/Index.rml", screen));
            buyScreen.Closed += (sender, e) =>
                {
                    openScreens.Add(buyScreen);
                    controller.GUIManager.removeManagedDialog(buyScreen);
                    buyScreen.Dispose();
                };
            controller.GUIManager.addManagedDialog(buyScreen);
            buyScreen.Visible = true;
            openScreens.Add(buyScreen);
        }
    }
}
