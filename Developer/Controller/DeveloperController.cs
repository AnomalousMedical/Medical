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

namespace Medical.Controller
{
    public delegate void SceneEvent(SimScene scene);

    /// <summary>
    /// This is the main controller for the Advanced program.
    /// </summary>
    public class DeveloperController : MedicalFormController, IDisposable
    {
        public event SceneEvent SceneLoaded;
        public event SceneEvent SceneUnloading;

        private MedicalController medicalController;
        private DrawingWindowController drawingWindowController;
        private AdvancedForm developerForm;
        private GUIElementController guiElements;
        private MedicalStateController stateController;
        private MedicalStateGUI stateGUI;
        private XmlSaver saver = new XmlSaver();
        private ImageRenderer imageRenderer;
        private MovementStateControl movementState;
        private ScenePicker scenePicker = new ScenePicker();
        private LayerController layerController;
        private Options options = null;
        private DockProvider dockProvider;
        private TemporaryStateBlender tempBlender;

        private NavigationController navigationController;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DeveloperController()
        {
            MedicalConfig config = new MedicalConfig(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Anomalous Software/Articulometrics/Developer");
        }

        /// <summary>
        /// Dispose function.
        /// </summary>
        public void Dispose()
        {
            OgreWrapper.OgreResourceGroupManager.getInstance().destroyResourceGroup("__InternalMedical");
            if (drawingWindowController != null)
            {
                drawingWindowController.Dispose();
            }
            if (medicalController != null)
            {
                medicalController.Dispose();
            }
            if (developerForm != null)
            {
                developerForm.Dispose();
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
            ProgressDialog splashScreen = new ProgressDialog(Resources.articulometricsdeveloper);
            splashScreen.fadeIn();
            splashScreen.ProgressMaximum = 100;
            try
            {

                developerForm = new AdvancedForm();
                developerForm.Text = "Articulometrics Developer";
                developerForm.Icon = Resources.skull;
                developerForm.initialize(this);
                medicalController = new MedicalController();
                medicalController.intialize(developerForm);

                splashScreen.stepProgress(10);

                dockProvider = new KryptonDockProvider(developerForm.DrawingWindowDockingManager, developerForm.DrawingWindowWorkspace);
                drawingWindowController = new DrawingWindowController(dockProvider);
                drawingWindowController.initialize(medicalController.EventManager, PluginManager.Instance.RendererPlugin, MedicalConfig.ConfigFile);

                navigationController = new NavigationController(drawingWindowController, medicalController.EventManager, medicalController.MainTimer);

                OgreWrapper.OgreResourceGroupManager.getInstance().createResourceGroup("__InternalMedical");

                imageRenderer = new ImageRenderer(medicalController, drawingWindowController, layerController, navigationController);
                stateController = new MedicalStateController(imageRenderer, medicalController);
                layerController = new LayerController();

                guiElements = new GUIElementController(developerForm.DockingManager, developerForm.HomeTab, medicalController);
                guiElements.EnableToolbars = true;

                //Add common gui elements
                LayersControl layersControl = new LayersControl();
                guiElements.addGUIElement(layersControl);

                PictureControl pictureControl = new PictureControl();
                pictureControl.initialize(imageRenderer, drawingWindowController, developerForm.DockingManager);
                guiElements.addGUIElement(pictureControl);

                stateGUI = new MedicalStateGUI();
                stateGUI.initialize(stateController);
                stateGUI.CreateStateCallback = createStateCallback;
                guiElements.addGUIElement(stateGUI);

                SavedCameraGUI savedCameraGUI = new SavedCameraGUI();
                savedCameraGUI.initialize(drawingWindowController, MedicalConfig.CamerasFile, navigationController);
                guiElements.addGUIElement(savedCameraGUI);

                scenePicker.initialize();

                //Add specific gui elements
                DiskControl discControl = new DiskControl();
                guiElements.addGUIElement(discControl);

                MandibleOffsetControl mandibleOffset = new MandibleOffsetControl();
                guiElements.addGUIElement(mandibleOffset);

                MandibleSizeControl mandibleSize = new MandibleSizeControl();
                mandibleSize.initialize(medicalController);
                guiElements.addGUIElement(mandibleSize);

                MuscleControl muscleControl = new MuscleControl();
                guiElements.addGUIElement(muscleControl);

                TeethControl teethControl = new TeethControl();
                teethControl.initialize(medicalController);
                guiElements.addGUIElement(teethControl);

                FossaControl fossaControl = new FossaControl();
                guiElements.addGUIElement(fossaControl);

                movementState = new MovementStateControl();
                guiElements.addGUIElement(movementState);

                MeasurementGUI measurement = new MeasurementGUI();
                guiElements.addGUIElement(measurement);

                //Editor
                BonePresetSaveWindow bonePresetSaver = new BonePresetSaveWindow();
                bonePresetSaver.initialize(imageRenderer, stateController);
                guiElements.addGUIElement(bonePresetSaver);

                PresetLayerEditor presetLayers = new PresetLayerEditor();
                presetLayers.initialize(layerController, imageRenderer);
                guiElements.addGUIElement(presetLayers);

                DiscPresetEditor discPresetEditor = new DiscPresetEditor();
                discPresetEditor.initialize(imageRenderer, stateController);
                guiElements.addGUIElement(discPresetEditor);

                FossaPresetEditor fossaPresetEditor = new FossaPresetEditor();
                fossaPresetEditor.initialize(imageRenderer, stateController);
                guiElements.addGUIElement(fossaPresetEditor);

                NavigationEditor navEditor = new NavigationEditor(navigationController, drawingWindowController);
                guiElements.addGUIElement(navEditor);

                NavigationMenuEditor menuEditor = new NavigationMenuEditor(navigationController, imageRenderer, navEditor, drawingWindowController);
                guiElements.addGUIElement(menuEditor);

                StateDistortionEditor stateEditor = new StateDistortionEditor(stateController, imageRenderer);
                guiElements.addGUIElement(stateEditor);

                options = new Options();

                SimObjectMover teethMover = new SimObjectMover("Teeth", medicalController.PluginManager, medicalController.EventManager);
                this.SceneLoaded += teethMover.sceneLoaded;
                this.SceneUnloading += teethMover.sceneUnloading;
                TeethController.TeethMover = teethMover;
                medicalController.FixedLoopUpdate += teethMover.update;

                tempBlender = new TemporaryStateBlender(medicalController.MainTimer, stateController);

                splashScreen.stepProgress(70);

                loadDefaultScene();

                if (!guiElements.restoreWindowFile(MedicalConfig.WindowsFile))
                {
                    setOneWindowLayout();
                }

                createNewSequence();

                splashScreen.stepProgress(20);

                developerForm.Show();
                developerForm.Activate();
                developerForm.Invalidate(true);

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
            scenePicker.ShowDialog(developerForm);
            if (scenePicker.DialogResult == DialogResult.OK)
            {
                changeScene(scenePicker.SelectedFileName);
                stateController.clearStates();
            }
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
            movementState.Filename = null;
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
                        movementState.Filename = filename;
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
                movementState.Filename = filename;
            }
        }

        public void showOptions()
        {
            if (options.ShowDialog(developerForm) == DialogResult.OK)
            {
                drawingWindowController.recreateAllWindows();
                medicalController.MainTimer.FramerateCap = MedicalConfig.EngineConfig.MaxFPS;
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
                    SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();
                    String layersFile = medicalController.CurrentSceneDirectory + "/" + medicalScene.GraphicsLayersFile;
                    layerController.loadLayerStateSet(layersFile);
                    String cameraFile = medicalController.CurrentSceneDirectory + "/" + medicalScene.GraphicsCameraFile;
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

        public void cloneActiveWindow()
        {
            drawingWindowController.cloneActiveWindow();
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
    }
}
