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
        private String licenseText;

        public LicenseDisplayManager()
        {
            
        }

        public void setLicenseText(String text)
        {
            this.licenseText = text;
            foreach (var display in activeLicenseDisplays)
            {
                display.LicenseText = licenseText;
            }
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
                licenseDisplay.LicenseText = licenseText;
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
