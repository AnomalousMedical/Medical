using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Standalone;
using System.Reflection;
using wx.Html.Help;

namespace Medical.GUI
{
    class UtilityGUIController : IDisposable
    {
        private ColorMenu colorMenu;
        private SceneViewController sceneViewController;
        private StandaloneController standaloneController;
        private PiperJBOGUI piperGUI;

        public UtilityGUIController(Widget ribbonWidget, PiperJBOGUI piperGUI, StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            this.sceneViewController = standaloneController.SceneViewController;
            this.piperGUI = piperGUI;

            Button colorButton = ribbonWidget.findWidget("UtilityTab/BackgroundButton") as Button;
            colorButton.MouseButtonClick += new MyGUIEvent(colorButton_MouseButtonClick);

            Button showStatsButton = ribbonWidget.findWidget("UtilityTab/ShowStatsButton") as Button;
            showStatsButton.MouseButtonClick += new MyGUIEvent(showStatsButton_MouseButtonClick);

            colorMenu = new ColorMenu();
            colorMenu.ColorChanged += new EventHandler(colorMenu_ColorChanged);

            Button cloneButton = ribbonWidget.findWidget("UtilityTab/CloneButton") as Button;
            cloneButton.MouseButtonClick += new MyGUIEvent(cloneButton_MouseButtonClick);
        }

        public void Dispose()
        {

        }

        void colorButton_MouseButtonClick(Widget source, EventArgs e)
        {
            colorMenu.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
        }

        void colorMenu_ColorChanged(object sender, EventArgs e)
        {
            sceneViewController.ActiveWindow.BackColor = colorMenu.SelectedColor;
        }

        void showStatsButton_MouseButtonClick(Widget source, EventArgs e)
        {
            //sceneViewController.ActiveWindow.ShowStats = !sceneViewController.ActiveWindow.ShowStats;
        }

        void cloneButton_MouseButtonClick(Widget source, EventArgs e)
        {
            piperGUI.toggleCloneWindow();
        }
    }
}
