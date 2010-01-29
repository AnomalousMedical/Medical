using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Engine.ObjectManagement;
using System.Drawing;
using Medical.Properties;
using Medical.Controller;

namespace Medical
{
    public class ClinicStatePicker : DistortionWizard
    {
        private StatePickerWizard statePicker;

        public ClinicStatePicker(StatePickerPanelController panelController)
        {
            statePicker = new StatePickerWizard(Name, panelController.UiHost, panelController.StateBlender, panelController.NavigationController, panelController.LayerController);
            statePicker.StateCreated += statePicker_StateCreated;
            statePicker.Finished += statePicker_Finished;

            statePicker.addStatePanel(panelController.getPanel(WizardPanels.LeftDopplerPanel));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.RightDopplerPanel));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.ProfileDistortionPanel));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.TopTeethRemovalPanel));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
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
                return "Clinical Exam Wizard";
            }
        }

        public override string TextLine1
        {
            get
            {
                return "Clinical Exam";
            }
        }

        public override string TextLine2
        {
            get
            {
                return "Wizard";
            }
        }

        public override Image ImageLarge
        {
            get
            {
                return Resources.ClinicalIcon;
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
