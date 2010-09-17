using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Standalone;
using Engine.ObjectManagement;

namespace Medical.GUI
{
    class MandibleMovementTaskbarItem : TaskbarItem
    {
        private MandibleMovementDialog mandibleDialog;

        public MandibleMovementTaskbarItem(StandaloneController standaloneController)
            :base("Manual Movement", "Joint")
        {
            standaloneController.SceneLoaded += new SceneEvent(standaloneController_SceneLoaded);
            standaloneController.SceneUnloading += new SceneEvent(standaloneController_SceneUnloading);

            mandibleDialog = new MandibleMovementDialog(standaloneController.MedicalController, standaloneController.MovementSequenceController);
            mandibleDialog.Shown += new EventHandler(mandibleDialog_Shown);
            mandibleDialog.Closed += new EventHandler(mandibleDialog_Closed);
        }

        public override void clicked(Widget source, EventArgs e)
        {
            mandibleDialog.Visible = !mandibleDialog.Visible;
        }

        public void closeDialog()
        {
            if (mandibleDialog.Visible)
            {
                mandibleDialog.close();
            }
        }

        void standaloneController_SceneUnloading(SimScene scene)
        {
            mandibleDialog.sceneUnloading(scene);
        }

        void standaloneController_SceneLoaded(SimScene scene)
        {
            mandibleDialog.sceneLoaded(scene);
        }

        void mandibleDialog_Closed(object sender, EventArgs e)
        {
            taskbarButton.StateCheck = false;
        }

        void mandibleDialog_Shown(object sender, EventArgs e)
        {
            taskbarButton.StateCheck = true;
        }
    }
}
