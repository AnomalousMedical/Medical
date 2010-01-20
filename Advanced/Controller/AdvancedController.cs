using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.Windows.Forms;
using Engine;
using System.Threading;
using System.IO;
using Engine.Resources;
using System.Xml;
using Engine.Saving.XMLSaver;
using Medical.Muscles;
using Medical.Properties;
using Engine.ObjectManagement;
using Logging;
using OgrePlugin;

namespace Medical.Controller
{
    public delegate void SceneEvent(SimScene scene);

    /// <summary>
    /// This is the main controller for the Advanced program.
    /// </summary>
    public class AdvancedController : MedicalFormController, IDisposable
    {
        public event SceneEvent SceneLoaded;
        public event SceneEvent SceneUnloading;

        private MedicalController medicalController;
        private DrawingWindowController drawingWindowController;
        private AdvancedForm advancedForm;
        private GUIElementController guiElements;
        private MedicalStateController stateController;
        private MedicalStateGUI stateGUI;
        private XmlSaver saver = new XmlSaver();
        private ImageRenderer imageRenderer;
        private MovementStateControl movementState;
        private ScenePicker scenePicker;
        private Options options = null;
        private DockProvider dockProvider;
        private TemporaryStateBlender tempBlender;

        private WatermarkController watermarkController;
        private Watermark watermark;
        private BackgroundController backgroundController;
        private ViewportBackground background;

        //Ribbon controllers
        //LayerGUIController layerGUIController;

        private NavigationController navigationController;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AdvancedController()
        {
            MedicalConfig config = new MedicalConfig(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Anomalous Software/Articulometrics/Research");
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
            if (medicalController != null)
            {
                medicalController.Dispose();
            }
            if (advancedForm != null)
            {
                advancedForm.Dispose();
            }
            if (options != null)
            {
                options.Dispose();
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
            ProgressDialog splashScreen = new ProgressDialog(Resources.articulometricsresearch);
            splashScreen.fadeIn();
            splashScreen.ProgressMaximum = 100;
            try
            {

                advancedForm = new AdvancedForm();
                advancedForm.initialize(this);
                medicalController = new MedicalController();
                medicalController.intialize(advancedForm);

                splashScreen.stepProgress(10);

                dockProvider = new KryptonDockProvider(advancedForm.DrawingWindowDockingManager, advancedForm.DrawingWindowWorkspace);
                drawingWindowController = new DrawingWindowController(dockProvider);
                drawingWindowController.initialize(medicalController.EventManager, PluginManager.Instance.RendererPlugin, MedicalConfig.ConfigFile);

                navigationController = new NavigationController(drawingWindowController, medicalController.EventManager, medicalController.MainTimer);

                imageRenderer = new ImageRenderer(medicalController, drawingWindowController, null, null);
                stateController = new MedicalStateController(imageRenderer, medicalController);

                guiElements = new GUIElementController(advancedForm.DockingManager, advancedForm.HomeTab, medicalController);
                guiElements.EnableToolbars = true;

                //Add common gui elements
                LayersControl layersControl = new LayersControl();
                guiElements.addGUIElement(layersControl);

                PictureControl pictureControl = new PictureControl();
                pictureControl.initialize(imageRenderer, drawingWindowController, advancedForm.DockingManager);
                guiElements.addGUIElement(pictureControl);

                stateGUI = new MedicalStateGUI();
                stateGUI.initialize(stateController);
                stateGUI.CreateStateCallback = createStateCallback;
                guiElements.addGUIElement(stateGUI);

                SavedCameraGUI savedCameraGUI = new SavedCameraGUI();
                savedCameraGUI.initialize(drawingWindowController, MedicalConfig.CamerasFile, navigationController);
                guiElements.addGUIElement(savedCameraGUI);

                MeasurementGUI measurement = new MeasurementGUI();
                guiElements.addGUIElement(measurement);

                scenePicker = new ScenePicker();
                scenePicker.initialize();

                //Add specific gui elements
                DiskControl discControl = new DiskControl();
                guiElements.addGUIElement(discControl);

                MandibleOffsetControl mandibleOffset = new MandibleOffsetControl();
                guiElements.addGUIElement(mandibleOffset);

                MandibleSizeControl mandibleSize = new MandibleSizeControl();
                mandibleSize.initialize(medicalController);
                guiElements.addGUIElement(mandibleSize);

                //MuscleControl muscleControl = new MuscleControl();
                //guiElements.addGUIElement(muscleControl);

                TeethControl teethControl = new TeethControl();
                teethControl.initialize(medicalController);
                guiElements.addGUIElement(teethControl);

                FossaControl fossaControl = new FossaControl();
                guiElements.addGUIElement(fossaControl);

                movementState = new MovementStateControl();
                guiElements.addGUIElement(movementState);

                OgreWrapper.OgreResourceGroupManager.getInstance().addResourceLocation(Engine.Resources.Resource.ResourceRoot + "/Watermark", "EngineArchive", "Watermark", false);
                OgreWrapper.OgreResourceGroupManager.getInstance().initializeAllResourceGroups();
                OgreWrapper.OgreResourceGroupManager.getInstance().createResourceGroup("__InternalMedical");
                watermark = new SideLogoWatermark("SourceWatermark", "AnomalousMedical", 150, 44, 4, 4);
                //watermark = new TiledWatermark("SourceWatermark", "PiperClinicBg", 150, 60);
                //watermark = new TextWatermark("SourceWatermark", "Copyright 2009 Piper Clinic", 50);
                watermark.createOverlays();
                watermarkController = new WatermarkController(watermark, drawingWindowController);

                //background = new ViewportBackground("SourceBackground", "PiperClinicBg", 900, 500, 250, 30, 30);
                background = new ViewportBackground("SourceBackground", "PiperClinicBg2", 900, 500, 500, 5, 5);
                backgroundController = new BackgroundController(background, drawingWindowController);

                SimObjectMover teethMover = new SimObjectMover("Teeth", medicalController.PluginManager, medicalController.EventManager);
                this.SceneLoaded += teethMover.sceneLoaded;
                this.SceneUnloading += teethMover.sceneUnloading;
                TeethController.TeethMover = teethMover;
                medicalController.FixedLoopUpdate += teethMover.update;

                tempBlender = new TemporaryStateBlender(medicalController.MainTimer, stateController);

                //Setup the ribbon
                //layerGUIController = new LayerGUIController(advancedForm);
                //this.SceneLoaded += layerGUIController.sceneLoaded;
                //this.SceneUnloading += layerGUIController.sceneUnloading;

                splashScreen.stepProgress(70);

                loadDefaultScene();

                if (!guiElements.restoreWindowFile(MedicalConfig.WindowsFile))
                {
                    setOneWindowLayout();
                }

                createNewSequence();

                options = new Options();

                splashScreen.stepProgress(20);

                advancedForm.Show();
                advancedForm.Activate();
                advancedForm.Invalidate(true);

                splashScreen.fadeAway();
                medicalController.start();
            }
            finally
            {
                splashScreen.fadeAway();
            }
        }

        /// <summary>
        /// Stop the loop and begin shutdown procedures.
        /// </summary>
        public void stop()
        {
            guiElements.saveWindowFile(MedicalConfig.WindowsFile);
            drawingWindowController.destroyCameras();
            drawingWindowController.closeAllWindows();
            medicalController.shutdown();
        }

        /// <summary>
        /// Open the specified file and change the scene.
        /// </summary>
        /// <param name="filename"></param>
        public void open(String filename)
        {
            changeScene(filename);
            stateController.clearStates();
        }

        public void newScene()
        {
            scenePicker.ShowDialog(advancedForm);
            if (scenePicker.DialogResult == DialogResult.OK)
            {
                changeScene(scenePicker.SelectedFileName);
                stateController.clearStates();
            }
        }

        public void cloneActiveWindow()
        {
            drawingWindowController.cloneActiveWindow();
        }

        public void setOneWindowLayout()
        {
            DrawingWindowPresetSet oneWindow = new DrawingWindowPresetSet();
            DrawingWindowPreset preset = new DrawingWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            oneWindow.addPreset(preset);

            drawingWindowController.createFromPresets(oneWindow);
        }

        public void setTwoWindowLayout()
        {
            DrawingWindowPresetSet twoWindows = new DrawingWindowPresetSet();
            DrawingWindowPreset preset = new DrawingWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            twoWindows.addPreset(preset);

            preset = new DrawingWindowPreset("Camera 2", new Vector3(0.0f, -5.0f, -170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            preset.ParentWindow = "Camera 1";
            preset.WindowPosition = DrawingWindowPosition.Right;
            twoWindows.addPreset(preset);

            drawingWindowController.createFromPresets(twoWindows);
        }

        public void setThreeWindowLayout()
        {
            DrawingWindowPresetSet threeWindows = new DrawingWindowPresetSet();
            DrawingWindowPreset preset = new DrawingWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            threeWindows.addPreset(preset);

            preset = new DrawingWindowPreset("Camera 2", new Vector3(-170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
            preset.ParentWindow = "Camera 1";
            preset.WindowPosition = DrawingWindowPosition.Bottom;
            threeWindows.addPreset(preset);

            preset = new DrawingWindowPreset("Camera 3", new Vector3(170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
            preset.ParentWindow = "Camera 2";
            preset.WindowPosition = DrawingWindowPosition.Right;
            threeWindows.addPreset(preset);

            drawingWindowController.createFromPresets(threeWindows);
        }

        public void setFourWindowLayout()
        {
            DrawingWindowPresetSet fourWindows = new DrawingWindowPresetSet();
            DrawingWindowPreset preset = new DrawingWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
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
            preset.ParentWindow = "Camera 2";
            preset.WindowPosition = DrawingWindowPosition.Bottom;
            fourWindows.addPreset(preset);

            drawingWindowController.createFromPresets(fourWindows);
        }

        public void showOptions()
        {
            if (options.ShowDialog(advancedForm) == DialogResult.OK)
            {
                drawingWindowController.recreateAllWindows();
                medicalController.MainTimer.FramerateCap = MedicalConfig.EngineConfig.MaxFPS;
            }
        }

        public void createNewMedicalStates()
        {
            stateController.clearStates();
        }

        public void createMedicalState(string name)
        {
            stateController.createAndAddState(name);
            stateGUI.CurrentBlend = stateController.getNumStates() - 1;
        }

        public void saveMedicalState(String filename)
        {
            XmlTextWriter textWriter = null;
            try
            {
                textWriter = new XmlTextWriter(filename, Encoding.Default);
                textWriter.Formatting = Formatting.Indented;
                SavedMedicalStates states = stateController.getSavedState(medicalController.CurrentSceneFile);
                saver.saveObject(states, textWriter);
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
        public void openStates(String filename)
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

        public void createNewSequence()
        {
            MovementSequence sequence = new MovementSequence();
            sequence.Duration = 5.0f;
            movementState.Sequence = sequence;
        }

        public void loadSequence(String filename)
        {
            using (Archive archive = FileSystem.OpenArchive(filename))
            {
                using (Stream stream = archive.openStream(filename, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                {
                    MovementSequence sequence = saver.restoreObject(new XmlTextReader(stream)) as MovementSequence;
                    if (sequence != null)
                    {
                        movementState.Sequence = sequence;
                    }
                }
            }
        }

        public void saveSequence(String filename)
        {
            using (XmlTextWriter writer = new XmlTextWriter(filename, Encoding.Default))
            {
                writer.Formatting = Formatting.Indented;
                saver.saveObject(movementState.Sequence, writer);
            }
        }

        public void applyDistortion(String filename)
        {
            using (XmlTextReader reader = new XmlTextReader(filename))
            {
                PresetState state = saver.restoreObject(reader) as PresetState;
                if (state != null)
                {
                    MedicalState targetState = stateController.createState("FileOpenTemp");
                    state.applyToState(targetState);
                    tempBlender.startTemporaryBlend(targetState);
                }
                else
                {
                    Log.Error("Could not load a preset state out of {0}.", filename);
                }
            }
        }

        public DrawingWindowController DrawingWindowController
        {
            get
            {
                return drawingWindowController;
            }
        }

        /// <summary>
        /// Change the scene to the specified filename.
        /// </summary>
        /// <param name="filename"></param>
        private bool changeScene(String file)
        {
            guiElements.alertGUISceneUnloading();
            if (SceneUnloading != null)
            {
                SceneUnloading.Invoke(medicalController.CurrentScene);
            }
            drawingWindowController.destroyCameras();
            background.destroyBackground();
            backgroundController.sceneUnloading();
            if (medicalController.openScene(file))
            {
                drawingWindowController.createCameras(medicalController.MainTimer, medicalController.CurrentScene, medicalController.CurrentSceneDirectory);
                guiElements.alertGUISceneLoaded(medicalController.CurrentScene);
                if (SceneLoaded != null)
                {
                    SceneLoaded.Invoke(medicalController.CurrentScene);
                }

                SimSubScene defaultScene = medicalController.CurrentScene.getDefaultSubScene();
                if (defaultScene != null)
                {
                    OgreSceneManager ogreScene = defaultScene.getSimElementManager<OgreSceneManager>();
                    backgroundController.sceneLoaded(ogreScene);
                    background.createBackground(ogreScene);

                    SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();
                    //String layersFile = medicalController.CurrentSceneDirectory + "/" + medicalScene.LayersFile;
                    //layerController.loadLayerStateSet(layersFile);
                    String cameraFile = medicalController.CurrentSceneDirectory + "/" + medicalScene.CameraFile;
                    navigationController.loadNavigationSet(cameraFile);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private void loadDefaultScene()
        {
            String sceneFileName = MedicalConfig.DefaultScene;
            changeScene(sceneFileName);
            stateController.clearStates();
        }

        private MedicalState createStateCallback(int index)
        {
            return stateController.createAndInsertState(index, "Auto");
        }
    }
}
