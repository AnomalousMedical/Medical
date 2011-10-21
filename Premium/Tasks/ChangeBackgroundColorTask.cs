using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical.Controller;
using Engine;

namespace Medical
{
    class ChangeBackgroundColorTask : Task
    {
        private SceneViewController sceneViewController;

        public ChangeBackgroundColorTask(SceneViewController sceneViewController)
            : base("Medical.ChangeBackgroundColor", "Background Color", "BackgroundColorIcon", TaskMenuCategories.System)
        {
            this.sceneViewController = sceneViewController;
        }

        public override void clicked(TaskPositioner positioner)
        {
            IntVector2 location = positioner.findGoodWindowPosition(0, 0);
            ColorMenu.ShowColorMenu(location.x, location.y, delegate(Color color)
            {
                sceneViewController.ActiveWindow.BackColor = color;
                fireItemClosed();
            });
        }

        public override bool Active
        {
            get { return false; }
        }
    }
}
