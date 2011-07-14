using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class CloneWindowTaskMenuItem : TaskMenuItem
    {
        private StandaloneController standaloneController;
        private CloneWindowDialog cloneWindowDialog;

        public CloneWindowTaskMenuItem(StandaloneController standaloneController, CloneWindowDialog cloneWindowDialog)
            : base("Clone Window", "CloneWindowIcon", TaskMenuCategories.System)
        {
            this.standaloneController = standaloneController;
            this.cloneWindowDialog = cloneWindowDialog;

            cloneWindowDialog.CreateCloneWindow += new EventHandler(cloneWindowDialog_CreateCloneWindow);
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
    }
}
