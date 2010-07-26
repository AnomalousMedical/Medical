using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;

namespace Medical.GUI
{
    class NavigationGUIController
    {
        private NavigationController navigationController;
        private CheckButton showNavigationButton;

        public NavigationGUIController(Widget ribbonWidget, NavigationController navigationController)
        {
            this.navigationController = navigationController;

            showNavigationButton = new CheckButton(ribbonWidget.findWidget("Navigation/ShowNavigationButton") as Button);
            showNavigationButton.CheckedChanged += new MyGUIEvent(showNavigationButton_CheckedChanged);
        }

        void showNavigationButton_CheckedChanged(Widget source, EventArgs e)
        {
            navigationController.ShowOverlays = showNavigationButton.Checked;
        }


    }
}
