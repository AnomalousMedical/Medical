using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;

namespace Medical.GUI
{
    class ShowStatsTaskbarItem : TaskbarItem
    {
        private SceneViewController sceneViewController;

        public ShowStatsTaskbarItem(SceneViewController sceneViewController)
            :base("Show Statistics", "StatsIconLarge")
        {
            this.sceneViewController = sceneViewController;
        }

        public override void clicked(Widget source, EventArgs e)
        {
            sceneViewController.ActiveWindow.ShowStats = !sceneViewController.ActiveWindow.ShowStats;
        }
    }
}
