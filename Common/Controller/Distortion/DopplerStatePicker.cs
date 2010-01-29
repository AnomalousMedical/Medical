using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Medical.Properties;
using Medical.GUI;
using Engine.ObjectManagement;
using Medical.Controller;

namespace Medical
{
    public class DopplerStatePicker : DistortionWizard
    {
        private StatePickerWizard statePicker;

        public DopplerStatePicker(StatePickerPanelController panelController)
        {
            statePicker = new StatePickerWizard(Name, panelController.UiHost, panelController.StateBlender, panelController.NavigationController, panelController.LayerController);
            statePicker.StateCreated += statePicker_StateCreated;
            statePicker.Finished += statePicker_Finished;

            statePicker.addStatePanel(panelController.getPanel(WizardPanels.LeftDopplerPanel));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.RightDopplerPanel));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.NotesPanel));
         }

        public override void Dispose()
        {
            statePicker.Dispose();
        }

        public override void setToDefault()
        {
            statePicker.setToDefault();
        }

        public override void sceneChanged(SimScene scene, string presetDirectory)
        {
            if (statePicker.Visible)
            {
                statePicker.closeForSceneChange();
            }
        }

        public override void startWizard(DrawingWindow displayWindow)
        {
            statePicker.startWizard(displayWindow);
            statePicker.show();
        }

        public override string Name
        {
            get 
            {
                return "Doppler Wizard";
            }
        }

        public override string TextLine1
        {
            get
            {
                return "Doppler Wizard";
            }
        }

        public override string TextLine2
        {
            get
            {
                return null;
            }
        }

        public override Image ImageLarge
        {
            get
            {
                return Resources.DopplerWizardLarge;
            }
        }

        void statePicker_StateCreated(MedicalState state)
        {
            alertStateCreated(state);
        }

        void statePicker_Finished()
        {
            alertWizardFinished();
        }
    }
}
