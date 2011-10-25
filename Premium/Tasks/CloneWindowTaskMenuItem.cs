using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class CloneWindowTask : Task
    {
        private StandaloneController standaloneController;
        private CloneWindowDialog cloneWindowDialog;

        public CloneWindowTask(StandaloneController standaloneController, CloneWindowDialog cloneWindowDialog)
            : base("Medical.CloneWindow", "Clone Window", "CloneWindowIcon", TaskMenuCategories.Tools)
        {
            this.standaloneController = standaloneController;
            this.cloneWindowDialog = cloneWindowDialog;
            this.ShowOnTimelineTaskbar = true;

            cloneWindowDialog.CreateCloneWindow += new EventHandler(cloneWindowDialog_CreateCloneWindow);
            cloneWindowDialog.Closed += new EventHandler(cloneWindowDialog_Closed);
        }

        public override void clicked(TaskPositioner positioner)
        {
            if (standaloneController.SceneViewController.HasCloneWindow)
            {
                standaloneController.SceneViewController.destroyCloneWindow();
                fireItemClosed();
            }
            else
            {
                cloneWindowDialog.open(true);
            }
        }

        void cloneWindowDialog_CreateCloneWindow(object sender, EventArgs e)
        {
            standaloneController.SceneViewController.createCloneWindow(cloneWindowDialog.createWindowInfo(), cloneWindowDialog.FloatOnParent);
        }

        void cloneWindowDialog_Closed(object sender, EventArgs e)
        {
            if (!standaloneController.SceneViewController.HasCloneWindow)
            {
                fireItemClosed();
            }
        }

        public override bool Active
        {
            get { return standaloneController.SceneViewController.HasCloneWindow; }
        }
    }
}
