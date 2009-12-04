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
        private TemporaryStateBlender temporaryStateBlender;
        private StatePickerWizard statePicker;
        private String lastPresetDirectory;

        private FullDistortionPicker profileDistortionPicker;
        private NotesPanel notesPanel;

        public ProfileStatePicker(StatePickerUIHost uiHost, MedicalController medicalController, MedicalStateController stateController, NavigationController navigationController, LayerController layerController, ImageRenderer imageRenderer)
        {
            temporaryStateBlender = new TemporaryStateBlender(medicalController.MainTimer, stateController);
            statePicker = new StatePickerWizard(uiHost, temporaryStateBlender, navigationController, layerController);
            statePicker.StateCreated += statePicker_StateCreated;
            statePicker.Finished += statePicker_Finished;

            profileDistortionPicker = new FullDistortionPicker();
            profileDistortionPicker.Text = "Profile";
            profileDistortionPicker.NavigationState = "Right Lateral";
            profileDistortionPicker.LayerState = "ProfileLayers";
            profileDistortionPicker.TextLine1 = "Profile";
            profileDistortionPicker.LargeIcon = Resources.AdaptationIcon;
            statePicker.addStatePanel(profileDistortionPicker);

            notesPanel = new NotesPanel(this.Name, imageRenderer);
            statePicker.addStatePanel(notesPanel);
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
            if (presetDirectory != lastPresetDirectory)
            {
                lastPresetDirectory = presetDirectory;
                profileDistortionPicker.initialize(presetDirectory + "/ProfilePresets");
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
                return null;
            }
        }
    }
}
