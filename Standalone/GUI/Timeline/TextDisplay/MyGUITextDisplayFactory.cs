using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical.Controller;
using Logging;
using MyGUIPlugin;

namespace Medical.GUI
{
    class MyGUITextDisplayFactory : ITextDisplayFactory
    {
        private List<MyGUITextDisplay> displays = new List<MyGUITextDisplay>();
        private SceneViewController sceneViewController;

        public MyGUITextDisplayFactory(SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
        }

        public ITextDisplay createTextDisplay(String cameraName)
        {
            SceneViewWindow sceneWindow = sceneViewController.findWindow(cameraName);
            if (sceneWindow == null)
            {
                sceneWindow = sceneViewController.ActiveWindow;
                Log.Warning("Could not find SceneViewWindow {0}. Using active window {1} instead.", cameraName, sceneWindow.Name);
            }
            MyGUITextDisplay display = new MyGUITextDisplay(this, sceneWindow);
            displays.Add(display);
            return display;
        }

        public List<String> FontNames
        {
            get
            {
                return new List<string>(new String[]{"TimelineText"});
            }
        }

        internal void textDisposed(MyGUITextDisplay display)
        {
            displays.Remove(display);
        }
    }
}
