using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Standalone;

namespace Medical.GUI
{
    class WindowGUIController : IDisposable
    {
        private ColorMenu colorMenu;
        private SceneViewController sceneViewController;
        private StandaloneController standaloneController;
        private OptionsDialog options;
        private CloneWindowDialog cloneWindowDialog;

        public WindowGUIController(Widget ribbonWidget, StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            this.sceneViewController = standaloneController.SceneViewController;

            Button colorButton = ribbonWidget.findWidget("WindowTab/BackgroundButton") as Button;
            colorButton.MouseButtonClick += new MyGUIEvent(colorButton_MouseButtonClick);

            Button showStatsButton = ribbonWidget.findWidget("WindowTab/ShowStatsButton") as Button;
            showStatsButton.MouseButtonClick += new MyGUIEvent(showStatsButton_MouseButtonClick);

            colorMenu = new ColorMenu("ColorMenu.layout");
            colorMenu.ColorChanged += new EventHandler(colorMenu_ColorChanged);

            options = new OptionsDialog("Options.layout");
            options.OptionsChanged += new EventHandler(options_OptionsChanged);

            cloneWindowDialog = new CloneWindowDialog("CloneWindowProperties.layout");
            cloneWindowDialog.CreateCloneWindow += new EventHandler(cloneWindowDialog_CreateCloneWindow);

            Button optionsButton = ribbonWidget.findWidget("WindowTab/Options") as Button;
            optionsButton.MouseButtonClick += new MyGUIEvent(optionsButton_MouseButtonClick);

            Button cloneButton = ribbonWidget.findWidget("WindowTab/CloneButton") as Button;
            cloneButton.MouseButtonClick += new MyGUIEvent(cloneButton_MouseButtonClick);
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

        void showStatsButton_MouseButtonClick(Widget source, EventArgs e)
        {
            sceneViewController.ActiveWindow.ShowStats = !sceneViewController.ActiveWindow.ShowStats;
        }

        void optionsButton_MouseButtonClick(Widget source, EventArgs e)
        {
            options.Visible = true;
        }

        void options_OptionsChanged(object sender, EventArgs e)
        {
            standaloneController.recreateMainWindow();
        }

        void cloneButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (sceneViewController.HasCloneWindow)
            {
                sceneViewController.destroyCloneWindow();
            }
            else
            {
                cloneWindowDialog.open(true);
            }
        }

        void cloneWindowDialog_CreateCloneWindow(object sender, EventArgs e)
        {
            standaloneController.SceneViewController.createCloneWindow(cloneWindowDialog.createWindowInfo());
        }
    }
}
