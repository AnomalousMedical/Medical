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

        //Wizards
        private StatePickerPanelController statePickerPanelController;

        private WatermarkController watermarkController;
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
            MedicalConfig config = new MedicalConfig(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Anomalous Software/Articulometrics/Clinical");
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
            splashScreen.fadeIn();
            splashScreen.ProgressMaximum = 100;
            try
            {
                shortcutController = new ShortcutController();
                basicForm = new BasicForm(shortcutController);
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
                watermark = new SideLogoWatermark("SourceWatermark", "AnomalousMedical", 150, 44, 4, 4);
                //watermark = new TiledWatermark("SourceWatermark", "PiperClinicBg", 150, 60);
                //watermark = new TextWatermark("SourceWatermark", "Copyright 2009 Piper Clinic", 50);
                watermark.createOverlays();
                watermarkController = new WatermarkController(watermark, drawingWindowController);

                //background = new ViewportBackground("SourceBackground", "PiperClinicBg", 900, 500, 250, 30, 30);
                background = new ViewportBackground("SourceBackground", "PiperClinicBg2", 900, 500, 500, 5, 5);
                backgroundController = new BackgroundController(background, drawingWindowController);

                teethMover = new SimObjectMover("Teeth", medicalController.PluginManager, medicalController.EventManager);
                this.SceneLoaded += teethMover.sceneLoaded;
                this.SceneUnloading += teethMover.sceneUnloading;
                TeethController.TeethMover = teethMover;
                medicalController.FixedLoopUpdate += teethMover.update;

                imageRenderer = new ImageRenderer(medicalController, drawingWindowController, layerController, navigationController);
                imageRenderer.Watermark = watermark;
                imageRenderer.Background = background;
                imageRenderer.ImageRenderStarted += TeethController.ScreenshotRenderStarted;
                imageRenderer.ImageRenderCompleted += TeethController.ScreenshotRenderCompleted;

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
            statePickerPanelController = new StatePickerPanelController(basicForm.StateWizardHost, medicalController, stateController, navigationController, layerController, imageRenderer, movementSequenceController, drawingWindowController);

            //Create wizards

            if (UserPermissions.Instance.allowFeature(Features.WIZARD_PIPER_JBO_DOPPLER))
            {
                //Doppler
                DistortionWizard dopplerWizard = new DistortionWizard("Doppler Wizard", statePickerPanelController);
                dopplerWizard.TextLine1 = "Doppler Wizard";
                dopplerWizard.ImageLarge = Resources.DopplerWizardLarge;
                dopplerWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftDopplerPanel));
                dopplerWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightDopplerPanel));
                dopplerWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(dopplerWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.WIZARD_PIPER_JBO_TEETH))
            {
                //Teeth
                DistortionWizard teethWizard = new DistortionWizard("Teeth Wizard", statePickerPanelController);
                teethWizard.TextLine1 = "Teeth Wizard";
                teethWizard.ImageLarge = Resources.TeethWizardIcon;
                teethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                teethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                teethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                teethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(teethWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.WIZARD_PIPER_JBO_PROFILE))
            {
                //Profile
                DistortionWizard profileWizard = new DistortionWizard("Profile Wizard", statePickerPanelController);
                profileWizard.TextLine1 = "Profile Wizard";
                profileWizard.ImageLarge = Resources.ProfileIcon;
                profileWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                profileWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(profileWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.WIZARD_PIPER_JBO_PROFILE_TEETH))
            {
                //Profile + Teeth
                DistortionWizard profileTeethWizard = new DistortionWizard("Profile and Teeth Wizard", statePickerPanelController);
                profileTeethWizard.TextLine1 = "Profile and Teeth";
                profileTeethWizard.TextLine2 = "Wizard";
                profileTeethWizard.ImageLarge = Resources.ProfileAndTeethWizardLarge;
                profileTeethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                profileTeethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                profileTeethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                profileTeethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                profileTeethWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(profileTeethWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.WIZARD_PIPER_JBO_BONE))
            {
                //Bone
                DistortionWizard boneWizard = new DistortionWizard("Bone Wizard", statePickerPanelController);
                boneWizard.TextLine1 = "Bone Wizard";
                boneWizard.ImageLarge = Resources.BoneWizardLarge;
                boneWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                boneWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                boneWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                boneWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                boneWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(boneWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.WIZARD_PIPER_JBO_CLINICAL))
            {
                //Clinical
                DistortionWizard clinicalWizard = new DistortionWizard("Clinical Exam Wizard", statePickerPanelController);
                clinicalWizard.TextLine1 = "Clinical Exam";
                clinicalWizard.TextLine2 = "Wizard";
                clinicalWizard.ImageLarge = Resources.ClinicalIcon;
                clinicalWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftDopplerPanel));
                clinicalWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightDopplerPanel));
                clinicalWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                clinicalWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                clinicalWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                clinicalWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                clinicalWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(clinicalWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.WIZARD_PIPER_JBO_CT))
            {
                //CT/Radiography Wizard
                DistortionWizard ctWizard = new DistortionWizard("CT/Radiography Wizard", statePickerPanelController);
                ctWizard.TextLine1 = "CT/Radiography";
                ctWizard.TextLine2 = "Wizard";
                ctWizard.ImageLarge = Resources.CTWizardLarge;
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftFossa));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftDiscSpacePanel));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightFossa));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightDiscSpacePanel));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.TeethAdaptationPanel));
                ctWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(ctWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.WIZARD_PIPER_JBO_DISC))
            {
                //Disc
                DistortionWizard discWizard = new DistortionWizard("Disc Wizard", statePickerPanelController);
                discWizard.TextLine1 = "Disc Wizard";
                discWizard.ImageLarge = Resources.DiscWizardLarge;
                discWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftDiscClockFacePanel));
                discWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightDiscClockFacePanel));
                discWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.NotesPanel));
                distortionController.addDistortionWizard(discWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.WIZARD_PIPER_JBO_MRI))
            {
                //MRI Wizard
                DistortionWizard mriWizard = new DistortionWizard("MRI Wizard", statePickerPanelController);
                mriWizard.TextLine1 = "MRI Wizard";
                mriWizard.ImageLarge = Resources.MRIWizardLarge;
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftDiscClockFacePanel));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.LeftFossa));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                mriWizard.addStatePanel(statePickerPanelController.getPanel(WizardPanels.RightDiscClockFacePanel));
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
            preset = new DrawingWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            oneWindow.addPreset(preset);
            windowPresetController.addPresetSet(oneWindow);

            if(UserPermissions.Instance.allowFeature(Features.PIPER_JBO_STANDARD) ||
                UserPermissions.Instance.allowFeature(Features.PIPER_JBO_GRAPHICS))
            {
                DrawingWindowPresetSet twoWindows = new DrawingWindowPresetSet("Two Windows");
                preset = new DrawingWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
                twoWindows.addPreset(preset);
                preset = new DrawingWindowPreset("Camera 2", new Vector3(0.0f, -5.0f, -170.0f), new Vector3(0.0f, -5.0f, 0.0f));
                preset.ParentWindow = "Camera 1";
                preset.WindowPosition = DrawingWindowPosition.Right;
                twoWindows.addPreset(preset);
                windowPresetController.addPresetSet(twoWindows);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_GRAPHICS))
            {
                DrawingWindowPresetSet threeWindows = new DrawingWindowPresetSet("Three Windows");
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
            navigationController.recalculateClosestStates();
            StatusController.SetStatus(String.Format("Opening scene {0}...", FileSystem.GetFileName(file)));
            if (movementSequenceController.Playing)
            {
                movementSequenceController.stopPlayback();
            }
            //distortionController.setToDefault();
            if (SceneUnloading != null)
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
                    String layersFile = medicalController.CurrentSceneDirectory + "/" + medicalScene.LayersFile;
                    layerController.loadLayerStateSet(layersFile);
                    String sequenceDirectory = medicalController.CurrentSceneDirectory + "/" + medicalScene.SequenceDirectory;
                    if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_GRAPHICS))
                    {
                        movementSequenceController.loadSequenceDirectories(sequenceDirectory + "/Lite", sequenceDirectory + "/Standard", sequenceDirectory + "/Graphics");
                    }
                    else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_STANDARD))
                    {
                        movementSequenceController.loadSequenceDirectories(sequenceDirectory + "/Lite", sequenceDirectory + "/Standard");
                    }
                    else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_LITE))
                    {
                        movementSequenceController.loadSequenceDirectories(sequenceDirectory + "/Lite");
                    }
                    String cameraFile = medicalController.CurrentSceneDirectory + "/" + medicalScene.CameraFile;
                    navigationController.loadNavigationSet(cameraFile);
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
            basicForm.Focus();
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