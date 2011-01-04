using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class CloneWindowTaskbarItem : TaskbarItem
    {
        private StandaloneController standaloneController;
        private CloneWindowDialog cloneWindowDialog;

        public CloneWindowTaskbarItem(StandaloneController standaloneController)
            :base("Clone Window", "CloneWindowLarge")
        {
            this.standaloneController = standaloneController;

            cloneWindowDialog = new CloneWindowDialog();
            cloneWindowDialog.CreateCloneWindow += new EventHandler(cloneWindowDialog_CreateCloneWindow);
        }

        public override void Dispose()
        {
            cloneWindowDialog.Dispose();
            base.Dispose();
        }

        public override void clicked(Widget source, EventArgs e)
        {
            if (standaloneController.SceneViewController.HasCloneWindow)
            {
                standaloneController.SceneViewController.destroyCloneWindow();
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
