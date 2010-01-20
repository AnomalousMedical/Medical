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
        private TemporaryStateBlender temporaryStateBlender;
        private StatePickerWizard statePicker;
        private DopplerPanel leftDopplerPanel;
        private DopplerPanel rightDopplerPanel;
        private ProfileDistortionPanel profileDistortionPicker;
        private TeethHeightAdaptationPanel teethHeightAdaptation;
        private NotesPanel notesPanel;

        public ClinicStatePicker(MovementSequenceController movementController, StatePickerUIHost uiHost, MedicalController medicalController, MedicalStateController stateController, NavigationController navigationController, LayerController layerController, ImageRenderer imageRenderer)
        {
            temporaryStateBlender = new TemporaryStateBlender(medicalController.MainTimer, stateController);
            statePicker = new StatePickerWizard(Name, uiHost, temporaryStateBlender, navigationController, layerController);
            statePicker.StateCreated += statePicker_StateCreated;
            statePicker.Finished += statePicker_Finished;

            leftDopplerPanel = new DopplerPanel("LeftDoppler", "Left TMJ", movementController);
            leftDopplerPanel.NavigationState = "Left TMJ";
            leftDopplerPanel.LayerState = "DiscLayers";
            leftDopplerPanel.Text = "Left Fossa";
            leftDopplerPanel.TextLine1 = "Left TMJ";
            leftDopplerPanel.TextLine2 = "Doppler";
            leftDopplerPanel.LargeIcon = Resources.LeftDiscSpace;
            statePicker.addStatePanel(leftDopplerPanel);

            rightDopplerPanel = new DopplerPanel("RightDoppler", "Right TMJ", movementController);
            rightDopplerPanel.NavigationState = "Right TMJ";
            rightDopplerPanel.LayerState = "DiscLayers";
            rightDopplerPanel.Text = "Right Fossa";
            rightDopplerPanel.TextLine1 = "Right TMJ";
            rightDopplerPanel.TextLine2 = "Doppler";
            rightDopplerPanel.LargeIcon = Resources.RightDiscSpace;
            statePicker.addStatePanel(rightDopplerPanel);

            profileDistortionPicker = new ProfileDistortionPanel();
            profileDistortionPicker.Text = "Profile";
            profileDistortionPicker.NavigationState = "Right Lateral";
            profileDistortionPicker.LayerState = "ProfileLayers";
            profileDistortionPicker.TextLine1 = "Profile";
            profileDistortionPicker.LargeIcon = Resources.ProfileIcon;
            statePicker.addStatePanel(profileDistortionPicker);

            statePicker.addStatePanel(new BottomTeethRemovalPanel());
            statePicker.addStatePanel(new TopTeethRemovalPanel());

            teethHeightAdaptation = new TeethHeightAdaptationPanel();
            teethHeightAdaptation.Text = "Teeth";
            teethHeightAdaptation.NavigationState = "Teeth Midline Anterior";
            teethHeightAdaptation.LayerState = "TeethLayers";
            teethHeightAdaptation.TextLine1 = "Teeth";
            teethHeightAdaptation.LargeIcon = Resources.AdaptationIcon;
            statePicker.addStatePanel(teethHeightAdaptation);

            notesPanel = new NotesPanel("Clinical Exam", imageRenderer);
            statePicker.addStatePanel(notesPanel);
         }

        public override void Dispose()
        {
            leftDopplerPanel.Dispose();
            rightDopplerPanel.Dispose();
            profileDistortionPicker.Dispose();
            notesPanel.Dispose();
            teethHeightAdaptation.Dispose();
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

            rightDopplerPanel.sceneChanged(presetDirectory);
            leftDopplerPanel.sceneChanged(presetDirectory);
            profileDistortionPicker.sceneChanged();
            teethHeightAdaptation.sceneChanged();
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
