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
        private Dictionary<String, MyGUIDynamicFontManager> fonts = new Dictionary<string, MyGUIDynamicFontManager>();
        private String defaultFont;

        public MyGUITextDisplayFactory(SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;

            //Timeline Text Font
            defaultFont = "TimelineText";
            MyGUIDynamicFontManager timelineText = new MyGUIDynamicFontManager();
            timelineText.addFont("TimelineText.10", 10);
            timelineText.addFont("TimelineText.25", 25);
            timelineText.addFont("TimelineText.50", 50);
            timelineText.addFont("TimelineText.100", 100);
            fonts.Add(defaultFont, timelineText);
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
                return new List<string>(fonts.Keys);
            }
        }

        internal void textDisposed(MyGUITextDisplay display)
        {
            displays.Remove(display);
        }

        internal String getMyGUIFont(String font, int size)
        {
            MyGUIDynamicFontManager fontManager;
            if (!fonts.TryGetValue(font, out fontManager))
            {
                Log.Warning("Could not find font {0} using default font {1} instead.", font, defaultFont);
                fontManager = fonts[defaultFont];
            }
            return fontManager.getFont(size);
        }
    }
}
