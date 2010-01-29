using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical.Properties;
using Engine.ObjectManagement;
using System.Drawing;

namespace Medical.Controller
{
    public class ProfileStatePicker : DistortionWizard
    {
        private StatePickerWizard statePicker;

        public ProfileStatePicker(StatePickerPanelController panelController)
        {
            statePicker = new StatePickerWizard(Name, panelController.UiHost, panelController.StateBlender, panelController.NavigationController, panelController.LayerController);
            statePicker.StateCreated += statePicker_StateCreated;
            statePicker.Finished += statePicker_Finished;

            statePicker.addStatePanel(panelController.getPanel(WizardPanels.ProfileDistortionPanel));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.NotesPanel));
        }

        public override void Dispose()
        {
            if (statePicker != null)
            {
                statePicker.Dispose();
            }
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
                return "Profile Wizard";
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

        public override String TextLine1
        {
            get
            {
                return "Profile Wizard";
            }
        }

        public override String TextLine2
        {
            get
            {
                return "";
            }
        }

        public override Image ImageLarge
        {
            get
            {
                return Resources.ProfileIcon;
            }
        }
    }
}
