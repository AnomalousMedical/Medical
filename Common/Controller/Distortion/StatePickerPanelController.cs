using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical.Properties;
using System.Windows.Forms;
using System.Drawing;
using Medical.Controller;
using Engine.Saving.XMLSaver;

namespace Medical
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

    public class StatePickerPanelController : IDisposable
    {
        private delegate StatePickerPanel CreatePanel();

        private Dictionary<WizardPanels, StatePickerPanel> panelDictionary = new Dictionary<WizardPanels, StatePickerPanel>();
        private Dictionary<WizardPanels, CreatePanel> panelCreationFunctions = new Dictionary<WizardPanels, CreatePanel>();
        private StatePickerUIHost uiHost;
        private MedicalController medicalController;
        private MedicalStateController stateController;
        private NavigationController navigationController;
        private LayerController layerController;
        private ImageRenderer imageRenderer;
        private ImageList imageList;
        private MovementSequenceController movementSequenceController;
        private TemporaryStateBlender stateBlender;
        private XmlSaver saver = new XmlSaver();
        private DrawingWindowController drawingWindowController;
        private NotesPanel notesPanel;
        private MeasurementGrid measurementGrid;

        public StatePickerPanelController(StatePickerUIHost uiHost, MedicalController medicalController, MedicalStateController stateController, NavigationController navigationController, LayerController layerController, ImageRenderer imageRenderer, MovementSequenceController movementSequenceController, DrawingWindowController drawingWindowController, MeasurementGrid measurementGrid)
        {
            this.uiHost = uiHost;
            this.medicalController = medicalController;
            this.stateController = stateController;
            this.navigationController = navigationController;
            this.layerController = layerController;
            this.imageRenderer = imageRenderer;
            this.movementSequenceController = movementSequenceController;
            this.drawingWindowController = drawingWindowController;
            this.measurementGrid = measurementGrid;

            imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(100, 100);

            this.stateBlender = new TemporaryStateBlender(medicalController.MainTimer, stateController);

            notesPanel = new NotesPanel("", imageRenderer, this);
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
            foreach (StatePickerPanel panel in panelDictionary.Values)
            {
                panel.Dispose();
            }
        }

        public void applyNotes(MedicalState state)
        {
            notesPanel.applyToState(state);
        }

        public void setCurrentWizard(DistortionWizard wizard)
        {
            notesPanel.DistortionWizardText = wizard.Name;
        }

        public StatePickerPanel getPanel(WizardPanels key)
        {
            StatePickerPanel panel;
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
            foreach (StatePickerPanel panel in panelDictionary.Values)
            {
                panel.sceneChanged(medicalController, simScene);
            }
            IntPtr handle = imageList.Handle;
        }

        internal void setNavigationState(string name)
        {
            navigationController.setNavigationState(name, drawingWindowController.getActiveWindow().DrawingWindow);
        }

        internal void setLayerState(string name)
        {
            layerController.applyLayerState(name);
        }

        internal void showChanges(StatePickerPanel panel, bool immediate)
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

        public StatePickerUIHost UiHost
        {
            get
            {
                return uiHost;
            }
        }

        public NavigationController NavigationController
        {
            get
            {
                return navigationController;
            }
        }

        public LayerController LayerController
        {
            get
            {
                return layerController;
            }
        }

        public ImageRenderer ImageRenderer
        {
            get
            {
                return imageRenderer;
            }
        }

        public ImageList ImageList
        {
            get
            {
                return imageList;
            }
        }

        public TemporaryStateBlender StateBlender
        {
            get
            {
                return stateBlender;
            }
        }

        public XmlSaver XmlSaver
        {
            get
            {
                return saver;
            }
        }

        public MeasurementGrid MeasurementGrid
        {
            get
            {
                return measurementGrid;
            }
        }

        private StatePickerPanel createLeftCondylarGrowth()
        {
            return new LeftCondylarGrowthPanel(this);
        }

        private StatePickerPanel createLeftCondylarDegeneration()
        {
            return new LeftCondylarDegenrationPanel(this);
        }

        private StatePickerPanel createRightCondylarGrowth()
        {
            return new RightCondylarGrowthPanel(this);
        }

        private StatePickerPanel createRightCondylarDegeneration()
        {
            return new RightCondylarDegenerationPanel(this);
        }

        private StatePickerPanel createLeftFossaPanel()
        {
            FossaPanel leftFossaPanel = new FossaPanel("LeftFossa", this);
            leftFossaPanel.NormalImage = Resources.LeftFossaNormal;
            leftFossaPanel.DistortedImage = Resources.LeftFossaFlat;
            leftFossaPanel.NavigationState = "WizardLeftTMJ";
            leftFossaPanel.LayerState = "FossaLayers";
            leftFossaPanel.Text = "Left Fossa";
            leftFossaPanel.TextLine1 = "Left Fossa";
            leftFossaPanel.LargeIcon = Resources.LeftFossaFlatness;
            return leftFossaPanel;
        }

        private StatePickerPanel createRightFossaPanel()
        {
            FossaPanel rightFossaPanel = new FossaPanel("RightFossa", this);
            rightFossaPanel.NormalImage = Resources.RightFossaNormal;
            rightFossaPanel.DistortedImage = Resources.RightFossaFlat;
            rightFossaPanel.NavigationState = "WizardRightTMJ";
            rightFossaPanel.LayerState = "FossaLayers";
            rightFossaPanel.Text = "Right Fossa";
            rightFossaPanel.TextLine1 = "Right Fossa";
            rightFossaPanel.LargeIcon = Resources.RightFossaFlatness;
            return rightFossaPanel;
        }

        private StatePickerPanel createLeftDopplerPanel()
        {
            DopplerPanel leftDopplerPanel = new DopplerPanel("LeftDoppler", "WizardLeftTMJ", "WizardLeftTMJSuperior", movementSequenceController, this);
            leftDopplerPanel.NavigationState = "WizardBothTMJSuperior";
            leftDopplerPanel.LayerState = "JointMenuLayers";
            leftDopplerPanel.Text = "Left Fossa";
            leftDopplerPanel.TextLine1 = "Left TMJ";
            leftDopplerPanel.TextLine2 = "Doppler";
            leftDopplerPanel.LargeIcon = Resources.LeftDiscSpace;
            return leftDopplerPanel;
        }

        private StatePickerPanel createRightDopplerPanel()
        {
            DopplerPanel rightDopplerPanel = new DopplerPanel("RightDoppler", "WizardRightTMJ", "WizardRightTMJSuperior", movementSequenceController, this);
            rightDopplerPanel.NavigationState = "WizardBothTMJSuperior";
            rightDopplerPanel.LayerState = "JointMenuLayers";
            rightDopplerPanel.Text = "Right Fossa";
            rightDopplerPanel.TextLine1 = "Right TMJ";
            rightDopplerPanel.TextLine2 = "Doppler";
            rightDopplerPanel.LargeIcon = Resources.RightDiscSpace;
            return rightDopplerPanel;
        }

        private StatePickerPanel createProfileDistortionPanel()
        {
            ProfileDistortionPanel profileDistortionPicker = new ProfileDistortionPanel(this);
            profileDistortionPicker.Text = "Profile";
            profileDistortionPicker.NavigationState = "WizardRightLateral";
            profileDistortionPicker.LayerState = "ProfileLayers";
            profileDistortionPicker.TextLine1 = "Profile";
            profileDistortionPicker.LargeIcon = Resources.ProfileIcon;
            return profileDistortionPicker;
        }

        private StatePickerPanel createBottomTeethRemovalPanel()
        {
            return new BottomTeethRemovalPanel(this);
        }

        private StatePickerPanel createTopTeethRemovalPanel()
        {
            return new TopTeethRemovalPanel(this);
        }

        private StatePickerPanel createTeethHeightAdaptationPanel()
        {
            TeethHeightAdaptationPanel teethHeightAdaptation = new TeethHeightAdaptationPanel(this);
            teethHeightAdaptation.Text = "Teeth";
            teethHeightAdaptation.NavigationState = "WizardTeethMidlineAnterior";
            teethHeightAdaptation.LayerState = "TeethLayers";
            teethHeightAdaptation.TextLine1 = "Teeth";
            teethHeightAdaptation.LargeIcon = Resources.AdaptationIcon;
            return teethHeightAdaptation;
        }

        private StatePickerPanel createLeftDiscSpacePanel()
        {
            DiscSpacePanel leftDiscPanel = new DiscSpacePanel("FossaLayers", "LeftDiscSpace", this);
            leftDiscPanel.NavigationState = "WizardLeftTMJ";
            leftDiscPanel.LayerState = "FossaLayers";
            leftDiscPanel.Text = "Left Disc Space";
            leftDiscPanel.TextLine1 = "Left Disc";
            leftDiscPanel.TextLine2 = "Space";
            leftDiscPanel.LargeIcon = Resources.LeftDiscSpace;
            return leftDiscPanel;
        }

        private StatePickerPanel createRightDiscSpacePanel()
        {
            DiscSpacePanel rightDiscPanel = new DiscSpacePanel("FossaLayers", "RightDiscSpace", this);
            rightDiscPanel.NavigationState = "WizardRightTMJ";
            rightDiscPanel.LayerState = "FossaLayers";
            rightDiscPanel.Text = "Right Disc Space";
            rightDiscPanel.TextLine1 = "Right Disc";
            rightDiscPanel.TextLine2 = "Space";
            rightDiscPanel.LargeIcon = Resources.RightDiscSpace;
            return rightDiscPanel;
        }

        private StatePickerPanel createLeftDiscClockFacePanel()
        {
            PresetStatePanel leftDiscPanel = new PresetStatePanel("LeftDisc", this);
            leftDiscPanel.Text = "Left Disc";
            leftDiscPanel.NavigationState = "WizardLeftTMJ";
            leftDiscPanel.LayerState = "DiscLayers";
            leftDiscPanel.TextLine1 = "Left TMJ";
            leftDiscPanel.LargeIcon = Resources.LeftDiscPosition;
            return leftDiscPanel;
        }

        private StatePickerPanel createRightDiscClockFacePanel()
        {
            PresetStatePanel rightDiscPanel = new PresetStatePanel("RightDisc", this);
            rightDiscPanel.Text = "Right Disc";
            rightDiscPanel.NavigationState = "WizardRightTMJ";
            rightDiscPanel.LayerState = "DiscLayers";
            rightDiscPanel.TextLine1 = "Right TMJ";
            rightDiscPanel.LargeIcon = Resources.RightDiscPosition;
            return rightDiscPanel;
        }

        private StatePickerPanel createDisclaimerPanel()
        {
            DisclaimerPanel disclaimerPanel = new DisclaimerPanel(this);
            return disclaimerPanel;
        }

        private StatePickerPanel createTeethAdaptationPanel()
        {
            return new TeethAdaptationPanel(this);
        }
    }
}
