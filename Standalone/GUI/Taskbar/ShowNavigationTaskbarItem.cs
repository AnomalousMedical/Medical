using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;

namespace Medical.GUI
{
    class ShowNavigationTaskbarItem : TaskbarItem
    {
        private NavigationController navigationController;

        public ShowNavigationTaskbarItem(NavigationController controller)
            :base("Show Navigation", "NavIconLarge")
        {
            this.navigationController = controller;
            navigationController.ShowOverlaysChanged += new NavigationControllerEvent(navigationController_ShowOverlaysChanged);
        }

        public override void clicked(Widget source, EventArgs e)
        {
            navigationController.ShowOverlays = !navigationController.ShowOverlays;
        }

        void navigationController_ShowOverlaysChanged(NavigationController controller)
        {
            taskbarButton.StateCheck = navigationController.ShowOverlays;
        }
    }
}
