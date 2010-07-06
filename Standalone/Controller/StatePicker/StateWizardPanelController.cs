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
        //private ImageRenderer imageRenderer;
        //private ImageList imageList;
        private MovementSequenceController movementSequenceController;
        private XmlSaver saver = new XmlSaver();
        private SceneViewController sceneViewController;
        private NotesPanel notesPanel;
        private TemporaryStateBlender stateBlender;
        //private MeasurementGrid measurementGrid;

        public StateWizardPanelController(Gui gui, MedicalController medicalController, MedicalStateController stateController, NavigationController navigationController, LayerController layerController, SceneViewController sceneViewController, TemporaryStateBlender stateBlender, MovementSequenceController movementSequenceController/*, ImageRenderer imageRenderer, MeasurementGrid measurementGrid*/)
        {
            this.stateBlender = stateBlender;
            this.medicalController = medicalController;
            this.stateController = stateController;
            this.navigationController = navigationController;
            this.layerController = layerController;
            this.sceneViewController = sceneViewController;
            //this.imageRenderer = imageRenderer;
            this.movementSequenceController = movementSequenceController;
            //this.measurementGrid = measurementGrid;

            notesPanel = new NotesPanel("DistortionPanels/NotesPanel.layout", this);
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
                panel.setToDefault();
                panel.sceneChanged(medicalController, simScene);
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
            LeftCondylarGrowthPanel leftCondyle = new LeftCondylarGrowthPanel("DistortionPanels/LeftCondylarGrowthPanel.layout", this);
            leftCondyle.LayerState = "MandibleSliderSizeLayers";
            leftCondyle.NavigationState = "WizardGrowthLeftCameraAngle";
            leftCondyle.TextLine1 = "Left Condyle";
            leftCondyle.TextLine2 = "Growth";
            return leftCondyle;
        }

        private StateWizardPanel createLeftCondylarDegeneration()
        {
            LeftCondylarDegenerationPanel leftCondyle = new LeftCondylarDegenerationPanel("DistortionPanels/LeftCondyleDegenerationPanel.layout", this);
            leftCondyle.LayerState = "MandibleSliderSizeLayers";
            leftCondyle.NavigationState = "WizardDegenerationLeftCameraAngle";
            leftCondyle.TextLine1 = "Left Condyle";
            leftCondyle.TextLine2 = "Degeneration";
            return leftCondyle;
        }

        private StateWizardPanel createRightCondylarGrowth()
        {
            RightCondylarGrowthPanel rightCondyle = new RightCondylarGrowthPanel("DistortionPanels/RightCondylarGrowthPanel.layout", this);
            rightCondyle.LayerState = "MandibleSliderSizeLayers";
            rightCondyle.NavigationState = "WizardGrowthRightCameraAngle";
            rightCondyle.TextLine1 = "Right Condyle";
            rightCondyle.TextLine2 = "Growth";
            return rightCondyle;
        }

        private StateWizardPanel createRightCondylarDegeneration()
        {
            RightCondylarDegenerationPanel rightCondyle = new RightCondylarDegenerationPanel("DistortionPanels/RightCondyleDegenerationPanel.layout", this);
            rightCondyle.LayerState = "MandibleSliderSizeLayers";
            rightCondyle.NavigationState = "WizardDegenerationRightCameraAngle";
            rightCondyle.TextLine1 = "Right Condyle";
            rightCondyle.TextLine2 = "Degeneration";
            return rightCondyle;
        }

        private StateWizardPanel createLeftFossaPanel()
        {
            FossaPanel leftFossaPanel = new FossaPanel("LeftFossa", "DistortionPanels/LeftFossaPanel.layout", this);
            //leftFossaPanel.NormalImage = Resources.LeftFossaNormal;
            //leftFossaPanel.DistortedImage = Resources.LeftFossaFlat;
            leftFossaPanel.NavigationState = "WizardLeftTMJ";
            leftFossaPanel.LayerState = "FossaLayers";
            leftFossaPanel.TextLine1 = "Left Fossa";
            //leftFossaPanel.LargeIcon = Resources.LeftFossaFlatness;
            return leftFossaPanel;
        }

        private StateWizardPanel createRightFossaPanel()
        {
            FossaPanel rightFossaPanel = new FossaPanel("RightFossa", "DistortionPanels/RightFossaPanel.layout", this);
            //rightFossaPanel.NormalImage = Resources.RightFossaNormal;
            //rightFossaPanel.DistortedImage = Resources.RightFossaFlat;
            rightFossaPanel.NavigationState = "WizardRightTMJ";
            rightFossaPanel.LayerState = "FossaLayers";
            rightFossaPanel.TextLine1 = "Right Fossa";
            //rightFossaPanel.LargeIcon = Resources.RightFossaFlatness;
            return rightFossaPanel;
        }

        private StateWizardPanel createLeftDopplerPanel()
        {
            DopplerPanel leftDopplerPanel = new DopplerPanel("DistortionPanels/DopplerPanel.layout", this, "LeftDoppler", "WizardLeftTMJ", "WizardLeftTMJSuperior");
            leftDopplerPanel.NavigationState = "WizardBothTMJSuperior";
            leftDopplerPanel.LayerState = "JointMenuLayers";
            leftDopplerPanel.TextLine1 = "Left TMJ";
            leftDopplerPanel.TextLine2 = "Doppler";
            return leftDopplerPanel;
        }

        private StateWizardPanel createRightDopplerPanel()
        {
            DopplerPanel rightDopplerPanel = new DopplerPanel("DistortionPanels/DopplerPanel.layout", this, "RightDoppler", "WizardRightTMJ", "WizardRightTMJSuperior");
            rightDopplerPanel.NavigationState = "WizardBothTMJSuperior";
            rightDopplerPanel.LayerState = "JointMenuLayers";
            rightDopplerPanel.TextLine1 = "Right TMJ";
            rightDopplerPanel.TextLine2 = "Doppler";
            return rightDopplerPanel;
        }

        private StateWizardPanel createProfileDistortionPanel()
        {
            ProfileDistortionPanel profileDistortionPicker = new ProfileDistortionPanel("DistortionPanels/ProfileDistortionPanel.layout", this);
            profileDistortionPicker.NavigationState = "WizardRightLateral";
            profileDistortionPicker.LayerState = "ProfileLayers";
            profileDistortionPicker.TextLine1 = "Profile";
            //profileDistortionPicker.LargeIcon = Resources.ProfileIcon;
            return profileDistortionPicker;
        }

        private StateWizardPanel createBottomTeethRemovalPanel()
        {
            ToothPanel panel = new ToothPanel("DistortionPanels/BottomTeethRemovalPanel.layout", this);
            panel.LayerState = "BottomTeethLayers";
            panel.NavigationState = "WizardBottomTeeth";
            panel.TextLine1 = "Remove";
            panel.TextLine2 = "Mandibular Teeth";
            return panel;
        }

        private StateWizardPanel createTopTeethRemovalPanel()
        {
            ToothPanel panel = new ToothPanel("DistortionPanels/TopTeethRemovalPanel.layout", this);
            panel.LayerState = "TopTeethLayers";
            panel.NavigationState = "WizardTopTeeth";
            panel.TextLine1 = "Remove";
            panel.TextLine2 = "Maxillary Teeth";
            return panel;
        }

        private StateWizardPanel createTeethHeightAdaptationPanel()
        {
            TeethHeightAdaptationPanel teethHeightAdaptation = new TeethHeightAdaptationPanel("DistortionPanels/TeethHeightAdaptationPanel.layout", this);
            teethHeightAdaptation.NavigationState = "WizardTeethMidlineAnterior";
            teethHeightAdaptation.LayerState = "TeethLayers";
            teethHeightAdaptation.TextLine1 = "Teeth";
            //teethHeightAdaptation.LargeIcon = Resources.AdaptationIcon;
            return teethHeightAdaptation;
        }

        private StateWizardPanel createLeftDiscSpacePanel()
        {
            DiscSpacePanel leftDiscPanel = new DiscSpacePanel("FossaLayers", "LeftDiscSpace", "DistortionPanels/DiscSpacePanel.layout", this);
            leftDiscPanel.NavigationState = "WizardLeftTMJ";
            leftDiscPanel.LayerState = "FossaLayers";
            leftDiscPanel.TextLine1 = "Left Disc";
            leftDiscPanel.TextLine2 = "Space";
            //leftDiscPanel.LargeIcon = Resources.LeftDiscSpace;
            return leftDiscPanel;
        }

        private StateWizardPanel createRightDiscSpacePanel()
        {
            DiscSpacePanel rightDiscPanel = new DiscSpacePanel("FossaLayers", "RightDiscSpace", "DistortionPanels/DiscSpacePanel.layout", this);
            rightDiscPanel.NavigationState = "WizardRightTMJ";
            rightDiscPanel.LayerState = "FossaLayers";
            rightDiscPanel.TextLine1 = "Right Disc";
            rightDiscPanel.TextLine2 = "Space";
            //rightDiscPanel.LargeIcon = Resources.RightDiscSpace;
            return rightDiscPanel;
        }

        private StateWizardPanel createLeftDiscClockFacePanel()
        {
            PresetStatePanel leftDiscPanel = new PresetStatePanel("LeftDisc", "DistortionPanels/PresetStatePanel.layout", this);
            //leftDiscPanel.Text = "Left Disc";
            leftDiscPanel.NavigationState = "WizardLeftTMJ";
            leftDiscPanel.LayerState = "DiscLayers";
            leftDiscPanel.TextLine1 = "Left TMJ";
            //leftDiscPanel.LargeIcon = Resources.LeftDiscPosition;
            return leftDiscPanel;
        }

        private StateWizardPanel createRightDiscClockFacePanel()
        {
            PresetStatePanel rightDiscPanel = new PresetStatePanel("RightDisc", "DistortionPanels/PresetStatePanel.layout", this);
            //rightDiscPanel.Text = "Right Disc";
            rightDiscPanel.NavigationState = "WizardRightTMJ";
            rightDiscPanel.LayerState = "DiscLayers";
            rightDiscPanel.TextLine1 = "Right TMJ";
            //rightDiscPanel.LargeIcon = Resources.RightDiscPosition;
            return rightDiscPanel;
        }

        private StateWizardPanel createDisclaimerPanel()
        {
            StateWizardPanel panel = new StateWizardPanel("DistortionPanels/DisclaimerPanel.layout", this);
            panel.TextLine1 = "Disclaimer";
            return panel;
        }

        private StateWizardPanel createTeethAdaptationPanel()
        {
            TeethAdaptationPanel teethPanel = new TeethAdaptationPanel("DistortionPanels/TeethAdaptationPanel.layout", this);
            teethPanel.LayerState = "TeethLayers";
            teethPanel.NavigationState = "WizardTeethMidlineAnterior";
            teethPanel.TextLine1 = "Teeth";
            teethPanel.TextLine2 = "Adaptation";
            return teethPanel;
        }

        #endregion
    }
}
