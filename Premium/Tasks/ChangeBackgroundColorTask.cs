﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical.Controller;
using Engine;
using Anomalous.GuiFramework;

namespace Medical
{
    class ChangeBackgroundColorTask : Task
    {
        private SceneViewController sceneViewController;

        public ChangeBackgroundColorTask(SceneViewController sceneViewController)
            : base("Medical.ChangeBackgroundColor", "Background Color", "PremiumFeatures/BackgroundColorIcon", TaskMenuCategories.System)
        {
            this.sceneViewController = sceneViewController;
        }

        public override void clicked(TaskPositioner positioner)
        {
            IntVector2 location = positioner.findGoodWindowPosition(0, 0);
            ColorMenu colorMenu = ColorMenu.ShowColorMenu(location.x, location.y, delegate(Color color)
            {
                sceneViewController.ActiveWindow.BackColor = color;
            });
            colorMenu.Hiding += (sender, e) =>
                {
                    fireItemClosed();
                };
        }

        public override bool Active
        {
            get { return false; }
        }
    }
}
