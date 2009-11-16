using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using WeifenLuo.WinFormsUI.Docking;
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
        private WatermarkController watermarkController;
        private ScenePicker scenePicker;
        private LayerController layerController;
        private DistortionController distortionController;
        private SkullStatePicker mriWizard;
        private Watermark watermark;
        private DrawingWindowPresetController windowPresetController;
        private ShortcutController shortcutController;
        private MovementSequenceController movementSequenceController;

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
            if (mriWizard != null)
            {
                mriWizard.Dispose();
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

            shortcutController = new ShortcutController();
            basicForm = new BasicForm(shortcutController);
            medicalController = new MedicalController();
            medicalController.intialize(basicForm);
            medicalController.PumpMessage += new PumpMessage(medicalController_PumpMessage);

            splashScreen.stepProgress(10);

            drawingWindowController = new DrawingWindowController();
            drawingWindowController.AllowRotation = false;
            drawingWindowController.AllowZoom = false;
            drawingWindowController.initialize(basicForm.DockPanel, medicalController.EventManager, PluginManager.Instance.RendererPlugin, MedicalConfig.ConfigFile);
            windowPresetController = new DrawingWindowPresetController(drawingWindowController);

            navigationController = new NavigationController(drawingWindowController, medicalController.EventManager, medicalController.MainTimer);
            layerController = new LayerController();
            movementSequenceController = new MovementSequenceController(medicalController);

            OgreWrapper.OgreResourceGroupManager.getInstance().addResourceLocation(Engine.Resources.Resource.ResourceRoot + "/Watermark", "EngineArchive", "Watermark", false);
            OgreWrapper.OgreResourceGroupManager.getInstance().initializeAllResourceGroups();
            watermark = new SideLogoWatermark("Source" + "Watermark", "PiperClinic", 150, 60);
            //watermark = new TiledWatermark("SourceWatermark", "PiperClinic", 150, 60);
            watermark.createOverlays();
            watermarkController = new WatermarkController(watermark, drawingWindowController);

            imageRenderer = new ImageRenderer(medicalController, drawingWindowController, layerController, navigationController);
            imageRenderer.Watermark = watermark;
            stateController = new MedicalStateController(imageRenderer, medicalController);

            scenePicker = new ScenePicker();
            scenePicker.initialize();

            //Configure distort mode
            distortionController = new DistortionController();
            distortionController.Finished += new StatePickerFinished(statePicker_Finished);
            distortionController.StateCreated += new MedicalStateCreated(statePicker_StateCreated);
            mriWizard = new SkullStatePicker(basicForm.DockPanel, basicForm.ToolStrip, medicalController, stateController, navigationController, layerController);
            distortionController.addDistortionWizard(mriWizard);
            basicForm.createDistortionMenu(distortionController.Wizards);

            basicForm.initialize(this, medicalController.ToolStripImages);

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

        void medicalController_PumpMessage(ref Message msg)
        {
            if (!distortionController.Visible)
            {
                shortcutController.processShortcuts(ref msg);
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

        public void saveMedicalState(String filename)
        {
            String saveFolder = Path.GetDirectoryName(filename);
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }
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
            StatusController.SetStatus(String.Format("Opening scene {0}...", FileSystem.GetFileName(file)));
            distortionController.setToDefault();
            if (SceneUnloading != null)
            {
                SceneUnloading.Invoke(medicalController.CurrentScene);
            }
            drawingWindowController.destroyCameras();
            if (medicalController.openScene(file))
            {
                drawingWindowController.createCameras(medicalController.MainTimer, medicalController.CurrentScene, medicalController.CurrentSceneDirectory);
                if (SceneLoaded != null)
                {
                    SceneLoaded.Invoke(medicalController.CurrentScene);
                }

                SimSubScene defaultScene = medicalController.CurrentScene.getDefaultSubScene();
                if (defaultScene != null)
                {
                    SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();
                    String layersFile = medicalController.CurrentSceneDirectory + "/" + medicalScene.LayersFile;
                    layerController.loadLayerStateSet(layersFile);
                    String cameraFile = medicalController.CurrentSceneDirectory + "/" + medicalScene.CameraFile;
                    navigationController.loadNavigationSet(cameraFile);
                    distortionController.updateStatePicker(medicalController.CurrentSceneDirectory + "/" + medicalScene.PresetDirectory);
                    windowPresetController.loadPresetSet();
                    String sequenceDirectory = medicalController.CurrentSceneDirectory + "/" + medicalScene.SequenceDirectory;
                    movementSequenceController.loadSequenceSet(sequenceDirectory);
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
                basicForm.ResumeLayout();
            }
        }

        void statePicker_StateCreated(MedicalState state)
        {
            stateController.addState(state);
        }

        void statePicker_Finished()
        {
            //since this does not process when the state controller is visible just reset buttons.
            shortcutController.resetButtons();
            basicForm.SuspendLayout();
            basicForm.ResumeLayout();
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
    }
}