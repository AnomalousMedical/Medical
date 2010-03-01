using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Engine;
using System.Windows.Forms;
using System.IO;
using Engine.Resources;
using Engine.Saving;
using Engine.Saving.XMLSaver;
using System.Xml;
using Medical.Properties;
using Logging;
using Engine.ObjectManagement;
using System.Drawing;
using OgrePlugin;
using Engine.Platform;

namespace Medical.Controller
{
    public delegate void SceneEvent(SimScene scene);

    public class BasicController : IDisposable
    {
        public String featureLevelString;

        public event SceneEvent SceneLoaded;
        public event SceneEvent SceneUnloading;

        private MedicalController medicalController;
        private DrawingWindowController drawingWindowController;
        private BasicForm basicForm;
        private MedicalStateController stateController;
        private XmlSaver saver = new XmlSaver();
        private Options options = null;
        private ImageRenderer imageRenderer;
        private NavigationController navigationController;
        private ScenePicker scenePicker;
        private LayerController layerController;
        private DistortionController distortionController;
        private DrawingWindowPresetController windowPresetController;
        private ShortcutController shortcutController;
        private MovementSequenceController movementSequenceController;
        private MeasurementGrid measurementGrid;

        //Wizards
        private StatePickerPanelController statePickerPanelController;

        private Watermark watermark;
        private BackgroundController backgroundController;
        private ViewportBackground background;

        private SimObjectMover teethMover;
        private DockProvider dockProvider;

        private const double checkInterval = 30;
        private double checkTime = 0.0;

        private bool closeOnWindowUpdate = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BasicController()
        {
            MedicalConfig config = new MedicalConfig(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Anomalous Medical/PiperJBO");
            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_GRAPHICS))
            {
                featureLevelString = "Graphics Edition";
            }
            else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_MRI))
            {
                featureLevelString = "MRI Edition";
            }
            else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_RADIOGRAPHY_CT))
            {
                featureLevelString = "Radiography and CT Edition";
            }
            else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_CLINICAL))
            {
                featureLevelString = "Clinical Edition";
            }
            else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_DENTITION_PROFILE))
            {
                featureLevelString = "Dentition and Profile Edition";
            }
            else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_DOPPLER))
            {
                featureLevelString = "Doppler Edition";
            }
        }

        /// <summary>
        /// Dispose function.
        /// </summary>
        public void Dispose()
        {
            OgreWrapper.OgreResourceGroupManager.getInstance().destroyResourceGroup("__InternalMedical");
            if (watermark != null)
            {
                watermark.Dispose();
            }
            if (measurementGrid != null)
            {
                measurementGrid.Dispose();
            }
            if (drawingWindowController != null)
            {
                drawingWindowController.Dispose();
            }
            if (basicForm != null)
            {
                basicForm.Dispose();
            }
            if (options != null)
            {
                options.Dispose();
            }
            if (distortionController != null)
            {
                distortionController.Dispose();
            }
            if (statePickerPanelController != null)
            {
                statePickerPanelController.Dispose();
            }
            if (medicalController != null)
            {
                medicalController.Dispose();
            }
            if (scenePicker != null)
            {
                scenePicker.Dispose();
            }
        }

        /// <summary>
        /// Start running the program.
        /// </summary>
        public void go()
        {
            ProgressDialog splashScreen = new ProgressDialog(Resources.ArticulometricsSplash);
            splashScreen.VersionString = featureLevelString;
            splashScreen.fadeIn();
            splashScreen.ProgressMaximum = 100;
            try
            {
                shortcutController = new ShortcutController();
                basicForm = new BasicForm(shortcutController, featureLevelString);
                medicalController = new MedicalController();
                medicalController.intialize(basicForm);
                medicalController.PumpMessage += new PumpMessage(medicalController_PumpMessage);
                medicalController.FixedLoopUpdate += new LoopUpdate(medicalController_FixedLoopUpdate);

                splashScreen.stepProgress(10);

                dockProvider = new KryptonDockProvider(basicForm.DockingManager, basicForm.DockableWorkspace);
                drawingWindowController = new DrawingWindowController(dockProvider);
                drawingWindowController.AllowRotation = false;
                drawingWindowController.AllowZoom = false;
                drawingWindowController.initialize(medicalController.EventManager, PluginManager.Instance.RendererPlugin, MedicalConfig.ConfigFile);
                windowPresetController = new DrawingWindowPresetController(drawingWindowController);
                createWindowPresets();

                navigationController = new NavigationController(drawingWindowController, medicalController.EventManager, medicalController.MainTimer);
                layerController = new LayerController();
                movementSequenceController = new MovementSequenceController(medicalController);

                OgreWrapper.OgreResourceGroupManager.getInstance().addResourceLocation(Engine.Resources.Resource.ResourceRoot + "/Watermark", "EngineArchive", "Watermark", false);
                OgreWrapper.OgreResourceGroupManager.getInstance().createResourceGroup("__InternalMedical");
                OgreWrapper.OgreResourceGroupManager.getInstance().initializeAllResourceGroups();
                watermark = new SideLogoWatermark("AnomalousMedicalWatermark", "AnomalousMedical", 150, 44, 4, 4);

                if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_GRAPHICS))
                {
                    background = new ViewportBackground("SourceBackground", "PiperJBOGraphicsBackground", 900, 500, 500, 5, 5);
                }
                else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_MRI))
                {
                    background = new ViewportBackground("SourceBackground", "PiperJBOMRIBackground", 900, 500, 500, 5, 5);
                }
                else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_RADIOGRAPHY_CT))
                {
                    background = new ViewportBackground("SourceBackground", "PiperJBORadiographyBackground", 900, 500, 500, 5, 5);
                }
                else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_CLINICAL))
                {
                    background = new ViewportBackground("SourceBackground", "PiperJBOClinicalBackground", 900, 500, 500, 5, 5);
                }
                else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_DENTITION_PROFILE))
                {
                    background = new ViewportBackground("SourceBackground", "PiperJBODentitionProfileBackground", 900, 500, 500, 5, 5);
                }
                else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_DOPPLER))
                {
                    background = new ViewportBackground("SourceBackground", "PiperJBODopplerBackground", 900, 500, 500, 5, 5);
                }
                backgroundController = new BackgroundController(background, drawingWindowController);

                teethMover = new SimObjectMover("Teeth", medicalController.PluginManager, medicalController.EventManager);
                this.SceneLoaded += teethMover.sceneLoaded;
                this.SceneUnloading += teethMover.sceneUnloading;
                TeethController.TeethMover = teethMover;
                medicalController.FixedLoopUpdate += teethMover.update;

                measurementGrid = new MeasurementGrid("Measurement", medicalController, drawingWindowController);
                this.SceneLoaded += measurementGrid.sceneLoaded;
                this.SceneUnloading += measurementGrid.sceneUnloading;

                imageRenderer = new ImageRenderer(medicalController, drawingWindowController, layerController, navigationController);
                imageRenderer.Watermark = watermark;
                imageRenderer.Background = background;
                imageRenderer.ImageRenderStarted += TeethController.ScreenshotRenderStarted;
                imageRenderer.ImageRenderCompleted += TeethController.ScreenshotRenderCompleted;
                imageRenderer.ImageRenderStarted += measurementGrid.ScreenshotRenderStarted;
                imageRenderer.ImageRenderCompleted += measurementGrid.ScreenshotRenderCompleted;

                stateController = new MedicalStateController(imageRenderer, medicalController);

                scenePicker = new ScenePicker();
                scenePicker.initialize();

                //Configure distort mode
                distortionController = new DistortionController();
                distortionController.Finished += new StatePickerFinished(statePicker_Finished);
                distortionController.StateCreated += new MedicalStateCreated(statePicker_StateCreated);
                createWizardPanels();
                basicForm.createDistortionMenu(distortionController.Wizards);

                basicForm.initialize(this);

                splashScreen.stepProgress(70);

                openDefaultScene();

                //if (!viewMode.restoreWindowFile(MedicalConfig.WindowsFile, getDockContent))
                {
                    windowPresetController.setPresetSet("Primary");
                }

                options = new Options();

                splashScreen.stepProgress(20);

                basicForm.Show();
                basicForm.Activate();
                basicForm.Invalidate(true);

                splashScreen.fadeAway();

                medicalController.start();
            }
            finally
            {
                splashScreen.fadeAway();
            }
        }

        private void createWizardPanels()
        {
            //Create panels
            statePickerPanelController = new StatePickerPanelController(basicForm.StateWizardHost, medicalController, stateController, navigationController, layerController, imageRenderer, movementSequenceController, drawingWindowController, measurementGrid);

            //Create wizards

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_DOPPLER))
            {
                //Doppler
                DistortionWizard dopplerWizard = new DistortionWizard("Doppler", "Single Distortion", statePickerPanelController);
                dopplerWizard.TextLine1 = "Doppler";
                dopplerWizard.ImageLarge = Resources.DopplerWizardLarge;
                dopplerWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.DisclaimerPanel));
                dopplerWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftDopplerPanel));
                dopplerWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightDopplerPanel));
                dopplerWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(dopplerWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_DENTITION))
            {
                //Teeth
                DistortionWizard teethWizard = new DistortionWizard("Dentition", "Single Distortion", statePickerPanelController);
                teethWizard.TextLine1 = "Dentition";
                teethWizard.ImageLarge = Resources.TeethWizardIcon;
                teethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.DisclaimerPanel));
                teethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                teethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                teethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                teethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(teethWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_CEPHALOMETRIC))
            {
                //Profile
                DistortionWizard profileWizard = new DistortionWizard("Cephalometric", "Single Distortion", statePickerPanelController);
                profileWizard.TextLine1 = "Cephalometric";
                profileWizard.ImageLarge = Resources.ProfileIcon;
                profileWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.DisclaimerPanel));
                profileWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                profileWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(profileWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_CEPHALOMETRIC_DENTITION))
            {
                //Profile + Teeth
                DistortionWizard profileTeethWizard = new DistortionWizard("Cephalometric and Dentition", "Combination Distortion", statePickerPanelController);
                profileTeethWizard.TextLine1 = "Cephalometric";
                profileTeethWizard.TextLine2 = "and Dentition";
                profileTeethWizard.ImageLarge = Resources.ProfileAndTeethWizardLarge;
                profileTeethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.DisclaimerPanel));
                profileTeethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                profileTeethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                profileTeethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                profileTeethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                profileTeethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(profileTeethWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_MANDIBLE))
            {
                //Bone
                DistortionWizard boneWizard = new DistortionWizard("Mandible", "Single Distortion", statePickerPanelController);
                boneWizard.TextLine1 = "Mandible";
                boneWizard.ImageLarge = Resources.BoneWizardLarge;
                boneWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.DisclaimerPanel));
                boneWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                boneWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                boneWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                boneWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                boneWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(boneWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_CLINICAL_DOPPLER))
            {
                //Clinical
                DistortionWizard clinicalWizard = new DistortionWizard("Clinical and Doppler", "Combination Distortion", statePickerPanelController);
                clinicalWizard.TextLine1 = "Clinical";
                clinicalWizard.TextLine2 = "and Doppler";
                clinicalWizard.ImageLarge = Resources.ClinicalIcon;
                clinicalWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.DisclaimerPanel));
                clinicalWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftDopplerPanel));
                clinicalWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightDopplerPanel));
                clinicalWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                clinicalWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                clinicalWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                clinicalWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                clinicalWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(clinicalWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_RADIOGRAPHY))
            {
                //CT/Radiography Wizard
                DistortionWizard ctWizard = new DistortionWizard("Clinical and Radiography", "Combination Distortion", statePickerPanelController);
                ctWizard.TextLine1 = "Clinical and";
                ctWizard.TextLine2 = "Radiography";
                ctWizard.ImageLarge = Resources.CTWizardLarge;
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.DisclaimerPanel));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftDiscSpacePanel));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftFossa));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightDiscSpacePanel));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightFossa));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TeethAdaptationPanel));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(ctWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_DISC_SPACE))
            {
                //Disc
                DistortionWizard discWizard = new DistortionWizard("Disc Space", "Single Distortion", statePickerPanelController);
                discWizard.TextLine1 = "Disc Space";
                discWizard.ImageLarge = Resources.DiscSpaceWizardIcon;
                discWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.DisclaimerPanel));
                discWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftDiscSpacePanel));
                discWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightDiscSpacePanel));
                discWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(discWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_DISC_CLOCK))
            {
                //Disc
                DistortionWizard discClockWizard = new DistortionWizard("Disc Clock Face", "Single Distortion", statePickerPanelController);
                discClockWizard.TextLine1 = "Disc";
                discClockWizard.TextLine2 = "Clock Face";
                discClockWizard.ImageLarge = Resources.DiscWizardLarge;
                discClockWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.DisclaimerPanel));
                discClockWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftDiscClockFacePanel));
                discClockWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightDiscClockFacePanel));
                discClockWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(discClockWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_MRI))
            {
                //MRI Wizard
                DistortionWizard mriWizard = new DistortionWizard("Clinical and MRI", "Combination Distortion", statePickerPanelController);
                mriWizard.TextLine1 = "Clinical";
                mriWizard.TextLine2 = "and MRI";
                mriWizard.ImageLarge = Resources.MRIWizardLarge;
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.DisclaimerPanel));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftDiscClockFacePanel));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftFossa));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightDiscClockFacePanel));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightFossa));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TeethAdaptationPanel));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(mriWizard);
            }
        }

        private void createWindowPresets()
        {
            windowPresetController.clearPresetSets();
            DrawingWindowPresetSet primary = new DrawingWindowPresetSet("Primary");
            DrawingWindowPreset preset = new DrawingWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            primary.addPreset(preset);
            primary.Hidden = true;
            windowPresetController.addPresetSet(primary);

            DrawingWindowPresetSet oneWindow = new DrawingWindowPresetSet("One Window");
            oneWindow.Image = Resources.OneWindowLayout;
            preset = new DrawingWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            oneWindow.addPreset(preset);
            windowPresetController.addPresetSet(oneWindow);

            if(UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_CLINICAL) ||
                UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_RADIOGRAPHY_CT) ||
                UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_MRI) ||
                UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_GRAPHICS))
            {
                DrawingWindowPresetSet twoWindows = new DrawingWindowPresetSet("Two Windows");
                twoWindows.Image = Resources.TwoWindowLayout;
                preset = new DrawingWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
                twoWindows.addPreset(preset);
                preset = new DrawingWindowPreset("Camera 2", new Vector3(0.0f, -5.0f, -170.0f), new Vector3(0.0f, -5.0f, 0.0f));
                preset.ParentWindow = "Camera 1";
                preset.WindowPosition = DrawingWindowPosition.Right;
                twoWindows.addPreset(preset);
                windowPresetController.addPresetSet(twoWindows);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_GRAPHICS))
            {
                DrawingWindowPresetSet threeWindows = new DrawingWindowPresetSet("Three Windows");
                threeWindows.Image = Resources.ThreeWindowLayout;
                preset = new DrawingWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
                threeWindows.addPreset(preset);
                preset = new DrawingWindowPreset("Camera 2", new Vector3(-170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
                preset.ParentWindow = "Camera 1";
                preset.WindowPosition = DrawingWindowPosition.Left;
                threeWindows.addPreset(preset);
                preset = new DrawingWindowPreset("Camera 3", new Vector3(170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
                preset.ParentWindow = "Camera 1";
                preset.WindowPosition = DrawingWindowPosition.Right;
                threeWindows.addPreset(preset);
                windowPresetController.addPresetSet(threeWindows);

                DrawingWindowPresetSet fourWindows = new DrawingWindowPresetSet("Four Windows");
                fourWindows.Image = Resources.FourWindowLayout;
                preset = new DrawingWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
                fourWindows.addPreset(preset);
                preset = new DrawingWindowPreset("Camera 2", new Vector3(0.0f, -5.0f, -170.0f), new Vector3(0.0f, -5.0f, 0.0f));
                preset.ParentWindow = "Camera 1";
                preset.WindowPosition = DrawingWindowPosition.Right;
                fourWindows.addPreset(preset);
                preset = new DrawingWindowPreset("Camera 3", new Vector3(-170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
                preset.ParentWindow = "Camera 1";
                preset.WindowPosition = DrawingWindowPosition.Bottom;
                fourWindows.addPreset(preset);
                preset = new DrawingWindowPreset("Camera 4", new Vector3(170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
                preset.ParentWindow = "Camera 3";
                preset.WindowPosition = DrawingWindowPosition.Right;
                fourWindows.addPreset(preset);
                windowPresetController.addPresetSet(fourWindows);
            }
        }

        void medicalController_FixedLoopUpdate(Clock time)
        {
            checkTime += time.Seconds;
            if (checkTime > checkInterval)
            {
                checkTime = 0;
                bool loop = true;
                while (loop)
                {
                    if (!UserPermissions.Instance.checkConnection())
                    {
                        DialogResult result = MessageBox.Show(basicForm, "Please reconnect your dongle.\nWarning, clicking cancel will close the program and all work will be lost.", "Dongle Connection Failure", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        if (result == DialogResult.Cancel)
                        {
                            loop = false;
                            closeOnWindowUpdate = true;
                        }
                    }
                    else
                    {
                        loop = false;
                    }
                }
            }
        }

        void medicalController_PumpMessage(ref Message msg)
        {
            if (!distortionController.Visible)
            {
                shortcutController.processShortcuts(ref msg);
            }
            if (closeOnWindowUpdate)
            {
                basicForm.Close();
            }
        }

        /// <summary>
        /// Stop the loop and begin shutdown procedures.
        /// </summary>
        public void stop()
        {
            //Only save windows if the state picker is not active.
            if (!distortionController.Visible)
            {
//                viewMode.saveWindowFile(MedicalConfig.WindowsFile);
            }
            drawingWindowController.destroyCameras();
            drawingWindowController.closeAllWindows();
            medicalController.shutdown();
        }

        /// <summary>
        /// Open the default scene.
        /// </summary>
        public void openDefaultScene()
        {
            changeScene(MedicalConfig.DefaultScene);
            stateController.clearStates();
        }

        public void newScene()
        {
            scenePicker.ShowDialog(basicForm);
            if (scenePicker.DialogResult == DialogResult.OK)
            {
                changeScene(scenePicker.SelectedFileName);
                stateController.clearStates();
            }
        }

        /// <summary>
        /// Used when restoring window positions. Return the window matching the
        /// persistString or null if no match is found.
        /// </summary>
        /// <param name="persistString">A string describing the window.</param>
        /// <returns>The matching DockContent or null if none is found.</returns>
        //public DockContent getDockContent(String persistString)
        //{
        //    DockContent ret = null;
        //    ret = viewMode.restoreWindow(persistString);
        //    if (ret == null)
        //    {
        //        ret = drawingWindowController.restoreFromString(persistString);
        //    }
        //    return ret;
        //}

        public void showOptions()
        {
            if (options.ShowDialog(basicForm) == DialogResult.OK)
            {
                StatusController.SetStatus("Applying options...");
                drawingWindowController.recreateAllWindows();
                medicalController.MainTimer.FramerateCap = MedicalConfig.EngineConfig.MaxFPS;
                StatusController.TaskCompleted();
            }
        }

        public void saveMedicalState(PatientDataFile patientData)
        {
            if (stateController.getNumStates() == 0)
            {
                stateController.createNormalStateFromScene();
            }
            patientData.SavedStates = stateController.getSavedState(medicalController.CurrentSceneFile);
            patientData.save();
        }

        private void oldSave(String filename)
        {
            XmlTextWriter textWriter = null;
            try
            {
                String saveFolder = Path.GetDirectoryName(filename);
                if (!Directory.Exists(saveFolder))
                {
                    Directory.CreateDirectory(saveFolder);
                }
                textWriter = new XmlTextWriter(filename, Encoding.Default);
                textWriter.Formatting = Formatting.Indented;
                SavedMedicalStates states = stateController.getSavedState(medicalController.CurrentSceneFile);
                saver.saveObject(states, textWriter);
            }
            catch (Exception e)
            {
                MessageBox.Show(basicForm, String.Format("Error saving file {0}.\n{1}", Path.GetFileNameWithoutExtension(filename), e.Message), "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (textWriter != null)
                {
                    textWriter.Close();
                }
            }
        }

        /// <summary>
        /// Open the specified file and change the scene.
        /// </summary>
        /// <param name="filename"></param>
        public void openStates(PatientDataFile dataFile)
        {
            if (dataFile.loadData())
            {
                SavedMedicalStates states = dataFile.SavedStates;
                if (states != null)
                {
                    changeScene(MedicalConfig.SceneDirectory + "/" + states.SceneName);
                    stateController.setStates(states);
                    stateController.blend(0.0f);
                }
                dataFile.closeData();
            }
            else
            {
                MessageBox.Show(basicForm, String.Format("Error loading file {0}.", dataFile.BackingFile), "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void oldLoad(String filename)
        {
            XmlTextReader textReader = null;
            try
            {
                textReader = new XmlTextReader(filename);
                SavedMedicalStates states = saver.restoreObject(textReader) as SavedMedicalStates;
                if (states != null)
                {
                    changeScene(MedicalConfig.SceneDirectory + "/" + states.SceneName);
                    stateController.setStates(states);
                    stateController.blend(0.0f);
                }
            }
            finally
            {
                if (textReader != null)
                {
                    textReader.Close();
                }
            }
        }

        public bool ShowNavigation
        {
            get
            {
                return navigationController.ShowOverlays;
            }
            set
            {
                navigationController.ShowOverlays = value;
            }
        }

        /// <summary>
        /// Change the scene to the specified filename.
        /// </summary>
        /// <param name="filename"></param>
        private bool changeScene(String file)
        {
            drawingWindowController.resetAllCameraPositions();
            navigationController.recalculateClosestNonHiddenStates();
            StatusController.SetStatus(String.Format("Opening scene {0}...", FileSystem.GetFileName(file)));
            if (movementSequenceController.Playing)
            {
                movementSequenceController.stopPlayback();
            }
            //distortionController.setToDefault();
            if (SceneUnloading != null && medicalController.CurrentScene != null)
            {
                SceneUnloading.Invoke(medicalController.CurrentScene);
            }
            drawingWindowController.destroyCameras();
            background.destroyBackground();
            backgroundController.sceneUnloading();
            if (medicalController.openScene(file))
            {
                SimSubScene defaultScene = medicalController.CurrentScene.getDefaultSubScene();
                if (defaultScene != null)
                {
                    OgreSceneManager ogreScene = defaultScene.getSimElementManager<OgreSceneManager>();
                    backgroundController.sceneLoaded(ogreScene);
                    background.createBackground(ogreScene);

                    drawingWindowController.createCameras(medicalController.MainTimer, medicalController.CurrentScene, medicalController.CurrentSceneDirectory);
                    SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();

                    String layersFile = medicalController.CurrentSceneDirectory + "/" + medicalScene.LayersFileDirectory;
                    String cameraFile = medicalController.CurrentSceneDirectory + "/" + medicalScene.CameraFileDirectory;
                    String sequenceDirectory = medicalController.CurrentSceneDirectory + "/" + medicalScene.SequenceDirectory;
                    if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_GRAPHICS))
                    {
                        movementSequenceController.loadSequenceDirectories(sequenceDirectory + "/Graphics", 
                            sequenceDirectory + "/MRI", 
                            sequenceDirectory + "/RadiographyCT",
                            sequenceDirectory + "/Clinical",
                            sequenceDirectory + "/DentitionProfile",
                            sequenceDirectory + "/Doppler");
                        cameraFile += "/GraphicsCameras.cam";
                        layersFile += "/GraphicsLayers.lay";
                    }
                    else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_MRI))
                    {
                        movementSequenceController.loadSequenceDirectories(sequenceDirectory + "/MRI",
                            sequenceDirectory + "/RadiographyCT",
                            sequenceDirectory + "/Clinical",
                            sequenceDirectory + "/DentitionProfile",
                            sequenceDirectory + "/Doppler");
                        cameraFile += "/MRICameras.cam";
                        layersFile += "/MRILayers.lay";
                    }
                    else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_RADIOGRAPHY_CT))
                    {
                        movementSequenceController.loadSequenceDirectories(sequenceDirectory + "/RadiographyCT",
                            sequenceDirectory + "/Clinical",
                            sequenceDirectory + "/DentitionProfile",
                            sequenceDirectory + "/Doppler");
                        cameraFile += "/RadiographyCameras.cam";
                        layersFile += "/RadiographyLayers.lay";
                    }
                    else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_CLINICAL))
                    {
                        movementSequenceController.loadSequenceDirectories(sequenceDirectory + "/Clinical",
                            sequenceDirectory + "/DentitionProfile",
                            sequenceDirectory + "/Doppler");
                        cameraFile += "/ClinicalCameras.cam";
                        layersFile += "/ClinicalLayers.lay";
                    }
                    else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_DENTITION_PROFILE))
                    {
                        movementSequenceController.loadSequenceDirectories(sequenceDirectory + "/DentitionProfile",
                            sequenceDirectory + "/Doppler");
                        cameraFile += "/DentitionProfileCameras.cam";
                        layersFile += "/DentitionProfileLayers.lay";
                    }
                    else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_DOPPLER))
                    {
                        movementSequenceController.loadSequenceDirectories(sequenceDirectory + "/Doppler");
                        cameraFile += "/DopplerCameras.cam";
                        layersFile += "/DopplerLayers.lay";
                    }
                    layerController.loadLayerStateSet(layersFile);
                    //Load camera file, merge baseline cameras if the cameras changed
                    if (navigationController.loadNavigationSetIfDifferent(cameraFile))
                    {
                        navigationController.mergeNavigationSet(medicalController.CurrentSceneDirectory + "/" + medicalScene.CameraFileDirectory + "/RequiredCameras.cam");
                    }
                    distortionController.sceneChanged(medicalController.CurrentScene, medicalController.CurrentSceneDirectory + "/" + medicalScene.PresetDirectory);
                    statePickerPanelController.sceneChanged(medicalController, medicalScene);
                    if (SceneLoaded != null)
                    {
                        SceneLoaded.Invoke(medicalController.CurrentScene);
                    }
                }
                distortionController.setToDefault();
                StatusController.TaskCompleted();
                return true;
            }
            else
            {
                StatusController.TaskCompleted();
                return false;
            }
        }

        internal void showStatePicker(String pickerName)
        {
            if (!distortionController.Visible)
            {
                if (stateController.getNumStates() == 0)
                {
                    stateController.createNormalStateFromScene();
                }
                basicForm.SuspendLayout();
                movementSequenceController.stopPlayback();
                distortionController.startWizard(pickerName, drawingWindowController.getActiveWindow().DrawingWindow);
                basicForm.enableViewMode(false);
                basicForm.ResumeLayout();
            }
        }

        void statePicker_StateCreated(MedicalState state)
        {
            stateController.addState(state);
            stateController.blend(stateController.getNumStates() - 1);
        }

        void statePicker_Finished()
        {
            //since this does not process when the state controller is visible just reset buttons.
            shortcutController.resetButtons();
            basicForm.SuspendLayout();
            basicForm.enableViewMode(true);
            basicForm.ResumeLayout();
            //Force focus back to either the active window or the form to make navigation easier to understand and
            //to prevent user inputs to closed wizards in normal mode.
            DrawingWindowHost activeWindow = drawingWindowController.getActiveWindow();
            if (activeWindow != null)
            {
                activeWindow.DrawingWindow.Focus();
            }
            else
            {
                basicForm.Focus();
            }
        }

        public DrawingWindowPresetController PresetWindows
        {
            get
            {
                return windowPresetController;
            }
        }

        public DrawingWindowController DrawingWindowController
        {
            get
            {
                return drawingWindowController;
            }
        }

        public ImageRenderer ImageRenderer
        {
            get
            {
                return imageRenderer;
            }
        }

        public LayerController LayerController
        {
            get
            {
                return layerController;
            }
        }

        public NavigationController NavigationController
        {
            get
            {
                return navigationController;
            }
        }

        public MedicalController MedicalController
        {
            get
            {
                return medicalController;
            }
        }

        public MovementSequenceController MovementSequenceController
        {
            get
            {
                return movementSequenceController;
            }
        }

        public MedicalStateController MedicalStateController
        {
            get
            {
                return stateController;
            }
        }

        public ShortcutController ShortcutController
        {
            get
            {
                return shortcutController;
            }
        }
    }
}