using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Engine;
using OgrePlugin;
using OgreWrapper;

namespace Medical.GUI
{
    public class WindowLayout : FixedSizeDialog
    {
        private SceneViewController sceneViewController;
        private WindowLayoutMenu windowLayoutMenu;

        public WindowLayout(StandaloneController standaloneController)
            :base("Medical.GUI.WindowLayout.WindowLayout.layout")
        {
            this.sceneViewController = standaloneController.SceneViewController;

            Button windowLayout = window.findWidget("WindowLayout") as Button;
            windowLayout.MouseButtonClick += new MyGUIEvent(windowLayout_MouseButtonClick);

            Button backgroundColor = window.findWidget("BackgroundColor") as Button;
            backgroundColor.MouseButtonClick += new MyGUIEvent(backgroundColor_MouseButtonClick);

            windowLayoutMenu = new WindowLayoutMenu(standaloneController);
        }

        public override void Dispose()
        {
            windowLayoutMenu.Dispose();
            base.Dispose();
        }

        void windowLayout_MouseButtonClick(Widget source, EventArgs e)
        {
            windowLayoutMenu.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
        }

        void backgroundColor_MouseButtonClick(Widget source, EventArgs e)
        {
            ColorMenu.ShowColorMenu(source.AbsoluteLeft, source.AbsoluteTop + source.Height, delegate(Color color)
            {
                sceneViewController.ActiveWindow.BackColor = color;
            });
        }
    }
}
