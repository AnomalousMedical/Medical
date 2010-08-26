using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Engine.Saving.XMLSaver;
using Engine;
using Medical.Muscles;

namespace Medical.GUI
{
    public enum WizardPanels
    {
        LeftCondylarDegeneration,
        RightCondylarDegeneration,
        LeftCondylarGrowth,
        RightCondylarGrowth,
        LeftFossa,
        RightFossa,
        LeftDopplerPanel,
        RightDopplerPanel,
        ProfileDistortionPanel,
        BottomTeethRemovalPanel,
        TopTeethRemovalPanel,
        TeethHeightAdaptationPanel,
        NotesPanel,
        LeftDiscSpacePanel,
        RightDiscSpacePanel,
        LeftDiscClockFacePanel,
        RightDiscClockFacePanel,
        TeethAdaptationPanel,
        DisclaimerPanel,
    }

    class StateWizardPanelController : IDisposable
    {
        private delegate StateWizardPanel CreatePanel();

        private Dictionary<WizardPanels, StateWizardPanel> panelDictionary = new Dictionary<WizardPanels, StateWizardPanel>();
        private Dictionary<WizardPanels, CreatePanel> panelCreationFunctions = new Dictionary<WizardPanels, CreatePanel>();
        private MedicalController medicalController;
        private MedicalStateController stateController;
        private NavigationController navigationController;
        private LayerController layerController;
        private ImageRenderer imageRenderer;
        private MovementSequenceController movementSequenceController;
        private XmlSaver saver = new XmlSaver();
        private SceneViewController sceneViewController;
        private NotesPanel notesPanel;
        private TemporaryStateBlender stateBlender;
        private MeasurementGrid measurementGrid;

        public StateWizardPanelController(Gui gui, MedicalController medicalController, MedicalStateController stateController, NavigationController navigationController, LayerController layerController, SceneViewController sceneViewController, TemporaryStateBlender stateBlender, MovementSequenceController movementSequenceController, ImageRenderer imageRenderer, MeasurementGrid measurementGrid)
        {
            this.stateBlender = stateBlender;
            this.medicalController = medicalController;
            this.stateController = stateController;
            this.navigationController = navigationController;
            this.layerController = layerController;
            this.sceneViewController = sceneViewController;
            this.imageRenderer = imageRenderer;
            this.movementSequenceController = movementSequenceController;
            this.measurementGrid = measurementGrid;

            notesPanel = new NotesPanel(this);
            notesPanel.ImageKey = "DistortionPanelIcons/Notes";
            panelDictionary.Add(WizardPanels.NotesPanel, notesPanel);

            panelCreationFunctions.Add(WizardPanels.LeftCondylarGrowth, createLeftCondylarGrowth);
            panelCreationFunctions.Add(WizardPanels.LeftCondylarDegeneration, createLeftCondylarDegeneration);
            panelCreationFunctions.Add(WizardPanels.RightCondylarGrowth, createRightCondylarGrowth);
            panelCreationFunctions.Add(WizardPanels.RightCondylarDegeneration, createRightCondylarDegeneration);
            panelCreationFunctions.Add(WizardPanels.LeftFossa, createLeftFossaPanel);
            panelCreationFunctions.Add(WizardPanels.RightFossa, createRightFossaPanel);
            panelCreationFunctions.Add(WizardPanels.LeftDopplerPanel, createLeftDopplerPanel);
            panelCreationFunctions.Add(WizardPanels.RightDopplerPanel, createRightDopplerPanel);
            panelCreationFunctions.Add(WizardPanels.ProfileDistortionPanel, createProfileDistortionPanel);
            panelCreationFunctions.Add(WizardPanels.BottomTeethRemovalPanel, createBottomTeethRemovalPanel);
            panelCreationFunctions.Add(WizardPanels.TopTeethRemovalPanel, createTopTeethRemovalPanel);
            panelCreationFunctions.Add(WizardPanels.TeethHeightAdaptationPanel, createTeethHeightAdaptationPanel);
            panelCreationFunctions.Add(WizardPanels.LeftDiscSpacePanel, createLeftDiscSpacePanel);
            panelCreationFunctions.Add(WizardPanels.RightDiscSpacePanel, createRightDiscSpacePanel);
            panelCreationFunctions.Add(WizardPanels.LeftDiscClockFacePanel, createLeftDiscClockFacePanel);
            panelCreationFunctions.Add(WizardPanels.RightDiscClockFacePanel, createRightDiscClockFacePanel);
            panelCreationFunctions.Add(WizardPanels.TeethAdaptationPanel, createTeethAdaptationPanel);
            panelCreationFunctions.Add(WizardPanels.DisclaimerPanel, createDisclaimerPanel);
        }

        public void Dispose()
        {
            foreach (StateWizardPanel panel in panelDictionary.Values)
            {
                panel.Dispose();
            }
            panelDictionary.Clear();
            notesPanel = null;
        }

        public StateWizardPanel getPanel(WizardPanels key)
        {
            StateWizardPanel panel;
            panelDictionary.TryGetValue(key, out panel);
            if (panel == null)
            {
                panel = panelCreationFunctions[key].Invoke();
                panelDictionary.Add(key, panel);
            }
            return panel;
        }

        public void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            foreach (StateWizardPanel panel in panelDictionary.Values)
            {
                panel.sceneChanged(medicalController, simScene);
                panel.setToDefault();
            }
        }

        public SceneViewWindow CurrentSceneView { get; set; }

        public XmlSaver XmlSaver
        {
            get
            {
                return saver;
            }
        }

        public TemporaryStateBlender StateBlender
        {
            get
            {
                return stateBlender;
            }
        }

        public ImageRenderer ImageRenderer
        {
            get
            {
                return imageRenderer;
            }
        }

        public MeasurementGrid MeasurementGrid
        {
            get
            {
                return measurementGrid;
            }
        }

        internal void setNavigationState(string name)
        {
            navigationController.setNavigationState(name, CurrentSceneView);
        }

        internal void setLayerState(string name)
        {
            layerController.applyLayerState(name);
        }

        /// <summary>
        /// Change the current movement sequence. Will return the movement sequence that was playing.
        /// </summary>
        /// <param name="sequence">The sequence to play.</param>
        /// <returns>The sequence that was playing.</returns>
        internal MovementSequence changeAndPlaySequence(MovementSequence sequence)
        {
            MovementSequence retVal = movementSequenceController.CurrentSequence;
            movementSequenceController.CurrentSequence = sequence;
            movementSequenceController.playCurrentSequence();
            return retVal;
        }

        /// <summary>
        /// Stop the playback of the sequence and set the current sequence to sequence.
        /// </summary>
        /// <param name="sequence">The sequence to set but not play.</param>
        internal void resetAndStopMovementSequence(MovementSequence sequence)
        {
            movementSequenceController.stopPlayback();
            movementSequenceController.CurrentSequence = sequence;
        }

        internal MovementSequence loadSequence(String filename)
        {
            return movementSequenceController.loadSequence(filename);
        }

        internal void showChanges(StateWizardPanel panel, bool immediate)
        {
            MedicalState createdState;
            createdState = stateBlender.createBaselineState();
            panel.applyToState(createdState);
            if (immediate)
            {
                createdState.blend(1.0f, createdState);
            }
            else
            {
                stateBlender.startTemporaryBlend(createdState);
            }
        }

        #region Creation Functions

        private StateWizardPanel createLeftCondylarGrowth()
        {
            LeftCondylarGrowthPanel leftCondyle = new LeftCondylarGrowthPanel(this);
            leftCondyle.LayerState = "MandibleSliderSizeLayers";
            leftCondyle.NavigationState = "WizardGrowthLeftCameraAngle";
            leftCondyle.TextLine1 = "Left Condyle";
            leftCondyle.TextLine2 = "Growth";
            leftCondyle.ImageKey = "DistortionPanelIcons/LeftCondyleGrowth";
            return leftCondyle;
        }

        private StateWizardPanel createLeftCondylarDegeneration()
        {
            LeftCondylarDegenerationPanel leftCondyle = new LeftCondylarDegenerationPanel(this);
            leftCondyle.LayerState = "MandibleSliderSizeLayers";
            leftCondyle.NavigationState = "WizardDegenerationLeftCameraAngle";
            leftCondyle.TextLine1 = "Left Condyle";
            leftCondyle.TextLine2 = "Degeneration";
            leftCondyle.ImageKey = "DistortionPanelIcons/LeftCondyleDegeneration";
            return leftCondyle;
        }

        private StateWizardPanel createRightCondylarGrowth()
        {
            RightCondylarGrowthPanel rightCondyle = new RightCondylarGrowthPanel(this);
            rightCondyle.LayerState = "MandibleSliderSizeLayers";
            rightCondyle.NavigationState = "WizardGrowthRightCameraAngle";
            rightCondyle.TextLine1 = "Right Condyle";
            rightCondyle.TextLine2 = "Growth";
            rightCondyle.ImageKey = "DistortionPanelIcons/RightCondyleGrowth";
            return rightCondyle;
        }

        private StateWizardPanel createRightCondylarDegeneration()
        {
            RightCondylarDegenerationPanel rightCondyle = new RightCondylarDegenerationPanel(this);
            rightCondyle.LayerState = "MandibleSliderSizeLayers";
            rightCondyle.NavigationState = "WizardDegenerationRightCameraAngle";
            rightCondyle.TextLine1 = "Right Condyle";
            rightCondyle.TextLine2 = "Degeneration";
            rightCondyle.ImageKey = "DistortionPanelIcons/RightCondyleDegeneration";
            return rightCondyle;
        }

        private StateWizardPanel createLeftFossaPanel()
        {
            FossaPanel leftFossaPanel = new FossaPanel("LeftFossa", "Medical.Controller.StatePicker.Panels.Fossa.FossaPanelLeft.layout", this);
            leftFossaPanel.NavigationState = "WizardLeftTMJ";
            leftFossaPanel.LayerState = "FossaLayers";
            leftFossaPanel.TextLine1 = "Left Fossa";
            leftFossaPanel.ImageKey = "DistortionPanelIcons/LeftFossa";
            return leftFossaPanel;
        }

        private StateWizardPanel createRightFossaPanel()
        {
            FossaPanel rightFossaPanel = new FossaPanel("RightFossa", "Medical.Controller.StatePicker.Panels.Fossa.FossaPanelRight.layout", this);
            rightFossaPanel.NavigationState = "WizardRightTMJ";
            rightFossaPanel.LayerState = "FossaLayers";
            rightFossaPanel.TextLine1 = "Right Fossa";
            rightFossaPanel.ImageKey = "DistortionPanelIcons/RightFossa";
            return rightFossaPanel;
        }

        private StateWizardPanel createLeftDopplerPanel()
        {
            DopplerPanel leftDopplerPanel = new DopplerPanel(this, "LeftDoppler", "WizardLeftTMJ", "WizardLeftTMJSuperior");
            leftDopplerPanel.NavigationState = "WizardBothTMJSuperior";
            leftDopplerPanel.LayerState = "JointMenuLayers";
            leftDopplerPanel.TextLine1 = "Left TMJ";
            leftDopplerPanel.TextLine2 = "Doppler";
            leftDopplerPanel.ImageKey = "DistortionPanelIcons/LeftDiscSpace";
            return leftDopplerPanel;
        }

        private StateWizardPanel createRightDopplerPanel()
        {
            DopplerPanel rightDopplerPanel = new DopplerPanel(this, "RightDoppler", "WizardRightTMJ", "WizardRightTMJSuperior");
            rightDopplerPanel.NavigationState = "WizardBothTMJSuperior";
            rightDopplerPanel.LayerState = "JointMenuLayers";
            rightDopplerPanel.TextLine1 = "Right TMJ";
            rightDopplerPanel.TextLine2 = "Doppler";
            rightDopplerPanel.ImageKey = "DistortionPanelIcons/RightDiscSpace";
            return rightDopplerPanel;
        }

        private StateWizardPanel createProfileDistortionPanel()
        {
            ProfileDistortionPanel profileDistortionPicker = new ProfileDistortionPanel(this);
            profileDistortionPicker.NavigationState = "WizardRightLateral";
            profileDistortionPicker.LayerState = "ProfileLayers";
            profileDistortionPicker.TextLine1 = "Profile";
            profileDistortionPicker.ImageKey = "DistortionPanelIcons/Profile";
            return profileDistortionPicker;
        }

        private StateWizardPanel createBottomTeethRemovalPanel()
        {
            ToothRemovalPanel panel = new ToothRemovalPanel("Medical.Controller.StatePicker.Panels.TeethPanels.ToothRemovalPanelBottom.layout", this);
            panel.LayerState = "BottomTeethLayers";
            panel.NavigationState = "WizardBottomTeeth";
            panel.TextLine1 = "Remove";
            panel.TextLine2 = "Mandibular Teeth";
            panel.ImageKey = "DistortionPanelIcons/BottomTeeth";
            return panel;
        }

        private StateWizardPanel createTopTeethRemovalPanel()
        {
            ToothRemovalPanel panel = new ToothRemovalPanel("Medical.Controller.StatePicker.Panels.TeethPanels.ToothRemovalPanelTop.layout", this);
            panel.LayerState = "TopTeethLayers";
            panel.NavigationState = "WizardTopTeeth";
            panel.TextLine1 = "Remove";
            panel.TextLine2 = "Maxillary Teeth";
            panel.ImageKey = "DistortionPanelIcons/TopTeeth";
            return panel;
        }

        private StateWizardPanel createTeethHeightAdaptationPanel()
        {
            TeethHeightAdaptationPanel teethHeightAdaptation = new TeethHeightAdaptationPanel(this);
            teethHeightAdaptation.NavigationState = "WizardTeethMidlineAnterior";
            teethHeightAdaptation.LayerState = "TeethLayers";
            teethHeightAdaptation.TextLine1 = "Teeth";
            teethHeightAdaptation.ImageKey = "DistortionPanelIcons/TeethAdaptation";
            return teethHeightAdaptation;
        }

        private StateWizardPanel createLeftDiscSpacePanel()
        {
            DiscSpacePanel leftDiscPanel = new DiscSpacePanel("FossaLayers", "LeftDiscSpace", this);
            leftDiscPanel.NavigationState = "WizardLeftTMJ";
            leftDiscPanel.LayerState = "FossaLayers";
            leftDiscPanel.TextLine1 = "Left Disc";
            leftDiscPanel.TextLine2 = "Space";
            leftDiscPanel.ImageKey = "DistortionPanelIcons/LeftDiscSpace";
            return leftDiscPanel;
        }

        private StateWizardPanel createRightDiscSpacePanel()
        {
            DiscSpacePanel rightDiscPanel = new DiscSpacePanel("FossaLayers", "RightDiscSpace", this);
            rightDiscPanel.NavigationState = "WizardRightTMJ";
            rightDiscPanel.LayerState = "FossaLayers";
            rightDiscPanel.TextLine1 = "Right Disc";
            rightDiscPanel.TextLine2 = "Space";
            rightDiscPanel.ImageKey = "DistortionPanelIcons/RightDiscSpace";
            return rightDiscPanel;
        }

        private StateWizardPanel createLeftDiscClockFacePanel()
        {
            PresetStatePanel leftDiscPanel = new PresetStatePanel("LeftDisc", this);
            leftDiscPanel.NavigationState = "WizardLeftTMJ";
            leftDiscPanel.LayerState = "DiscLayers";
            leftDiscPanel.TextLine1 = "Left TMJ";
            leftDiscPanel.ImageKey = "DistortionPanelIcons/LeftDiscPosition";
            return leftDiscPanel;
        }

        private StateWizardPanel createRightDiscClockFacePanel()
        {
            PresetStatePanel rightDiscPanel = new PresetStatePanel("RightDisc", this);
            rightDiscPanel.NavigationState = "WizardRightTMJ";
            rightDiscPanel.LayerState = "DiscLayers";
            rightDiscPanel.TextLine1 = "Right TMJ";
            rightDiscPanel.ImageKey = "DistortionPanelIcons/RightDiscPosition";
            return rightDiscPanel;
        }

        private StateWizardPanel createDisclaimerPanel()
        {
            StateWizardPanel panel = new StateWizardPanel("Medical.Controller.StatePicker.Panels.Disclaimer.DisclaimerPanel.layout", this);
            panel.TextLine1 = "Disclaimer";
            panel.ImageKey = "DistortionPanelIcons/Disclaimer";
            return panel;
        }

        private StateWizardPanel createTeethAdaptationPanel()
        {
            TeethAdaptationPanel teethPanel = new TeethAdaptationPanel(this);
            teethPanel.LayerState = "TeethLayers";
            teethPanel.NavigationState = "WizardTeethMidlineAnterior";
            teethPanel.TextLine1 = "Teeth";
            teethPanel.TextLine2 = "Adaptation";
            teethPanel.ImageKey = "DistortionPanelIcons/TeethAdaptation";
            return teethPanel;
        }

        #endregion
    }
}
