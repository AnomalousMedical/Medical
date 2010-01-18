﻿using System;
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
        private TemporaryStateBlender temporaryStateBlender;
        private StatePickerWizard statePicker;
        private DopplerPanel leftDopplerPanel;
        private DopplerPanel rightDopplerPanel;
        private NotesPanel notesPanel;

        public DopplerStatePicker(MovementSequenceController movementController, StatePickerUIHost uiHost, MedicalController medicalController, MedicalStateController stateController, NavigationController navigationController, LayerController layerController, ImageRenderer imageRenderer)
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

            notesPanel = new NotesPanel("Doppler", imageRenderer);
            statePicker.addStatePanel(notesPanel);
         }

        public override void Dispose()
        {
            leftDopplerPanel.Dispose();
            rightDopplerPanel.Dispose();
            notesPanel.Dispose();
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
