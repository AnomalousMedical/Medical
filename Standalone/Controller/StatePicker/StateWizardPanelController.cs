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
                panel.sceneChanged(medicalController, simScene);
            }
        }

        public SceneViewWindow CurrentSceneView { get; set; }

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
            //FossaPanel leftFossaPanel = new FossaPanel("LeftFossa", this);
            //leftFossaPanel.NormalImage = Resources.LeftFossaNormal;
            //leftFossaPanel.DistortedImage = Resources.LeftFossaFlat;
            //leftFossaPanel.NavigationState = "WizardLeftTMJ";
            //leftFossaPanel.LayerState = "FossaLayers";
            //leftFossaPanel.Text = "Left Fossa";
            //leftFossaPanel.TextLine1 = "Left Fossa";
            //leftFossaPanel.LargeIcon = Resources.LeftFossaFlatness;
            //return leftFossaPanel;
            return new StateWizardPanel("DistortionPanels/LeftFossaPanel.layout", this);
        }

        private StateWizardPanel createRightFossaPanel()
        {
            //FossaPanel rightFossaPanel = new FossaPanel("RightFossa", this);
            //rightFossaPanel.NormalImage = Resources.RightFossaNormal;
            //rightFossaPanel.DistortedImage = Resources.RightFossaFlat;
            //rightFossaPanel.NavigationState = "WizardRightTMJ";
            //rightFossaPanel.LayerState = "FossaLayers";
            //rightFossaPanel.Text = "Right Fossa";
            //rightFossaPanel.TextLine1 = "Right Fossa";
            //rightFossaPanel.LargeIcon = Resources.RightFossaFlatness;
            //return rightFossaPanel;
            return new StateWizardPanel("DistortionPanels/RightFossaPanel.layout", this);
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
            //ProfileDistortionPanel profileDistortionPicker = new ProfileDistortionPanel(this);
            //profileDistortionPicker.Text = "Profile";
            //profileDistortionPicker.NavigationState = "WizardRightLateral";
            //profileDistortionPicker.LayerState = "ProfileLayers";
            //profileDistortionPicker.TextLine1 = "Profile";
            //profileDistortionPicker.LargeIcon = Resources.ProfileIcon;
            //return profileDistortionPicker;
            throw new NotImplementedException();
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
            //TeethHeightAdaptationPanel teethHeightAdaptation = new TeethHeightAdaptationPanel(this);
            //teethHeightAdaptation.Text = "Teeth";
            //teethHeightAdaptation.NavigationState = "WizardTeethMidlineAnterior";
            //teethHeightAdaptation.LayerState = "TeethLayers";
            //teethHeightAdaptation.TextLine1 = "Teeth";
            //teethHeightAdaptation.LargeIcon = Resources.AdaptationIcon;
            //return teethHeightAdaptation;
            throw new NotImplementedException();
        }

        private StateWizardPanel createLeftDiscSpacePanel()
        {
            //DiscSpacePanel leftDiscPanel = new DiscSpacePanel("FossaLayers", "LeftDiscSpace", this);
            //leftDiscPanel.NavigationState = "WizardLeftTMJ";
            //leftDiscPanel.LayerState = "FossaLayers";
            //leftDiscPanel.Text = "Left Disc Space";
            //leftDiscPanel.TextLine1 = "Left Disc";
            //leftDiscPanel.TextLine2 = "Space";
            //leftDiscPanel.LargeIcon = Resources.LeftDiscSpace;
            //return leftDiscPanel;
            return new StateWizardPanel("DistortionPanels/DiscSpacePanel.layout", this);
        }

        private StateWizardPanel createRightDiscSpacePanel()
        {
            //DiscSpacePanel rightDiscPanel = new DiscSpacePanel("FossaLayers", "RightDiscSpace", this);
            //rightDiscPanel.NavigationState = "WizardRightTMJ";
            //rightDiscPanel.LayerState = "FossaLayers";
            //rightDiscPanel.Text = "Right Disc Space";
            //rightDiscPanel.TextLine1 = "Right Disc";
            //rightDiscPanel.TextLine2 = "Space";
            //rightDiscPanel.LargeIcon = Resources.RightDiscSpace;
            //return rightDiscPanel;
            return new StateWizardPanel("DistortionPanels/DiscSpacePanel.layout", this);
        }

        private StateWizardPanel createLeftDiscClockFacePanel()
        {
            //PresetStatePanel leftDiscPanel = new PresetStatePanel("LeftDisc", this);
            //leftDiscPanel.Text = "Left Disc";
            //leftDiscPanel.NavigationState = "WizardLeftTMJ";
            //leftDiscPanel.LayerState = "DiscLayers";
            //leftDiscPanel.TextLine1 = "Left TMJ";
            //leftDiscPanel.LargeIcon = Resources.LeftDiscPosition;
            //return leftDiscPanel;
            throw new NotImplementedException();
        }

        private StateWizardPanel createRightDiscClockFacePanel()
        {
            //PresetStatePanel rightDiscPanel = new PresetStatePanel("RightDisc", this);
            //rightDiscPanel.Text = "Right Disc";
            //rightDiscPanel.NavigationState = "WizardRightTMJ";
            //rightDiscPanel.LayerState = "DiscLayers";
            //rightDiscPanel.TextLine1 = "Right TMJ";
            //rightDiscPanel.LargeIcon = Resources.RightDiscPosition;
            //return rightDiscPanel;
            throw new NotImplementedException();
        }

        private StateWizardPanel createDisclaimerPanel()
        {
            StateWizardPanel panel = new StateWizardPanel("DistortionPanels/DisclaimerPanel.layout", this);
            panel.TextLine1 = "Disclaimer";
            return panel;
        }

        private StateWizardPanel createTeethAdaptationPanel()
        {
            //return new TeethAdaptationPanel(this);
            throw new NotImplementedException();
        }

        #endregion
    }
}
