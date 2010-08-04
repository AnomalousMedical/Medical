using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;

namespace Medical.GUI
{
    class WindowGUIController : IDisposable
    {
        private ColorMenu colorMenu;
        private SceneViewController sceneViewController;

        public WindowGUIController(Widget ribbonWidget, SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;

            Button colorButton = ribbonWidget.findWidget("WindowTab/BackgroundButton") as Button;
            colorButton.MouseButtonClick += new MyGUIEvent(colorButton_MouseButtonClick);

            colorMenu = new ColorMenu("ColorMenu.layout");
            colorMenu.ColorChanged += new EventHandler(colorMenu_ColorChanged);
        }

        public void Dispose()
        {

        }

        void colorButton_MouseButtonClick(Widget source, EventArgs e)
        {
            colorMenu.show(source.getAbsoluteLeft(), source.getAbsoluteTop() + source.getHeight());
        }

        void colorMenu_ColorChanged(object sender, EventArgs e)
        {
            sceneViewController.ActiveWindow.BackColor = colorMenu.SelectedColor;
        }
    }
}
