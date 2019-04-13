using Anomalous.GuiFramework.Cameras;
using Medical.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class LicenseDisplayManager
    {
        private List<LicenseDisplay> activeLicenseDisplays = new List<LicenseDisplay>();

        public LicenseDisplayManager()
        {
            
        }

        public void setSceneViewController(SceneViewController sceneViewController)
        {
            sceneViewController.WindowCreated += sceneViewController_WindowCreated;
        }

        void sceneViewController_WindowCreated(SceneViewWindow window)
        {
            MDISceneViewWindow mdiWindow = window as MDISceneViewWindow;
            if (mdiWindow != null)
            {
                LicenseDisplay licenseDisplay = new LicenseDisplay();
                activeLicenseDisplays.Add(licenseDisplay);
                mdiWindow.addChildContainer(licenseDisplay.LayoutContainer);
                mdiWindow.Disposed += (win) =>
                {
                    activeLicenseDisplays.Remove(licenseDisplay);
                    licenseDisplay.Dispose();
                };
            }
        }
    }
}
