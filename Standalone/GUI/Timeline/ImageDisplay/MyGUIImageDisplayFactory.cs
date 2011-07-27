using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical.Controller;
using Logging;

namespace Medical.GUI
{
    class MyGUIImageDisplayFactory : IImageDisplayFactory, OSWindowListener
    {
        private List<MyGUIImageDisplay> displays = new List<MyGUIImageDisplay>();
        private SceneViewController sceneViewController;

        public MyGUIImageDisplayFactory(SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
        }

        public IImageDisplay createImageDisplay(String cameraName)
        {
            SceneViewWindow sceneWindow = sceneViewController.findWindow(cameraName);
            if (sceneWindow == null)
            {
                sceneWindow = sceneViewController.ActiveWindow;
                Log.Warning("Could not find SceneViewWindow {0}. Using active window {1} instead.", cameraName, sceneWindow.Name);
            }
            MyGUIImageDisplay display = new MyGUIImageDisplay(this, sceneWindow);
            displays.Add(display);
            return display;
        }

        internal void displayDisposed(MyGUIImageDisplay display)
        {
            displays.Remove(display);
        }

        #region OSWindowListener Members

        public void closed(OSWindow window)
        {
            
        }

        public void closing(OSWindow window)
        {
            
        }

        public void focusChanged(OSWindow window)
        {
            
        }

        public void moved(OSWindow window)
        {
            
        }

        public void resized(OSWindow window)
        {
            foreach (MyGUIImageDisplay display in displays)
            {
                display.screenResized(window.WindowWidth, window.WindowHeight);
            }
        }

        #endregion
    }
}
