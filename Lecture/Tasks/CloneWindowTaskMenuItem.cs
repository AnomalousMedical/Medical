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
            : base("Medical.CloneWindow", "Clone Window", "CloneWindowIcon", TaskMenuCategories.System)
        {
            this.standaloneController = standaloneController;
            this.cloneWindowDialog = cloneWindowDialog;

            cloneWindowDialog.CreateCloneWindow += new EventHandler(cloneWindowDialog_CreateCloneWindow);
            cloneWindowDialog.Closed += new EventHandler(cloneWindowDialog_Closed);
        }

        public override void clicked()
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
            standaloneController.SceneViewController.createCloneWindow(cloneWindowDialog.createWindowInfo());
        }

        void cloneWindowDialog_Closed(object sender, EventArgs e)
        {
            if (!standaloneController.SceneViewController.HasCloneWindow)
            {
                fireItemClosed();
            }
        }
    }
}
