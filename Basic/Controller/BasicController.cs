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

namespace Medical.Controller
{
    public class BasicController : IDisposable
    {
        private MedicalController medicalController;
        private DrawingWindowController drawingWindowController;
        private BasicForm basicForm;
        private GUIElementController viewMode;
        private GUIElementController distortMode;
        private MedicalStateController stateController = new MedicalStateController();
        private MedicalStateGUI stateGUI;
        private XmlSaver saver = new XmlSaver();
        private Options options = null;
        private StatePicker statePicker = new StatePicker();
        private ImageRenderer imageRenderer;
        private NavigationController navigationController;
        private WatermarkController watermarkController;
        private TemporaryStateBlender temporaryStateBlender;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BasicController()
        {
            MedicalConfig config = new MedicalConfig(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Anomalous Software/Basic");
        }

        /// <summary>
        /// Dispose function.
        /// </summary>
        public void Dispose()
        {
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
            if (statePicker != null)
            {
                statePicker.Dispose();
            }
            if (medicalController != null)
            {
                medicalController.Dispose();
            }
        }

        /// <summary>
        /// Start running the program.
        /// </summary>
        public void go()
        {
            ProgressDialog splashScreen = new ProgressDialog(Resources.articulometricsclinic);
            splashScreen.fadeIn();
            splashScreen.ProgressMaximum = 100;

            basicForm = new BasicForm();
            basicForm.initialize(this);
            medicalController = new MedicalController();
            medicalController.intialize(basicForm);

            splashScreen.stepProgress(10);

            drawingWindowController = new DrawingWindowController(MedicalConfig.CamerasFile);
            drawingWindowController.AllowRotation = false;
            drawingWindowController.initialize(basicForm.DockPanel, medicalController.EventManager, PluginManager.Instance.RendererPlugin, MedicalConfig.ConfigFile);

            navigationController = new NavigationController(drawingWindowController, medicalController.EventManager, medicalController.MainTimer);

            temporaryStateBlender = new TemporaryStateBlender(medicalController.MainTimer, stateController);

            OgreWrapper.OgreResourceGroupManager.getInstance().addResourceLocation(Engine.Resources.Resource.ResourceRoot + "/Watermark", "EngineArchive", "Watermark", false);
            OgreWrapper.OgreResourceGroupManager.getInstance().initializeAllResourceGroups();
            //Watermark watermark = new TiledWatermark("Source" + "Watermark", "Watermark", 150, 60);
            //Watermark watermark = new TextWatermark("Source" + "Watermark", "Piper Clinic Copyright 2009", 32);
            Watermark watermark = new SideLogoWatermark("Source" + "Watermark", "PiperClinic", 150, 60);
            //Watermark watermark = new CenteredWatermark("Source" + "Watermark", "PiperClinicAlpha", 1.0f, 0.4f);
            watermarkController = new WatermarkController(watermark, drawingWindowController);

            imageRenderer = new ImageRenderer(medicalController, drawingWindowController);

            //Configure view mode
            viewMode = new GUIElementController(basicForm.DockPanel, basicForm.ToolStrip, medicalController);

            PictureControl pictureControl = new PictureControl();
            pictureControl.initialize(imageRenderer, drawingWindowController);
            viewMode.addGUIElement(pictureControl);

            stateGUI = new MedicalStateGUI();
            stateGUI.initialize(stateController);
            viewMode.addGUIElement(stateGUI);

            SavedCameraGUI savedCameraGUI = new SavedCameraGUI();
            savedCameraGUI.initialize(drawingWindowController);
            viewMode.addGUIElement(savedCameraGUI);

            //Add specific gui elements
            MuscleControl muscleControl = new MuscleControl();
            viewMode.addGUIElement(muscleControl);

            SimpleLayerControl simpleLayer = new SimpleLayerControl();
            viewMode.addGUIElement(simpleLayer);

            viewMode.EnableToolbars = true;

            //Configure distort mode
            distortMode = new GUIElementController(basicForm.DockPanel, basicForm.ToolStrip, medicalController);
            constructStatePicker();

            splashScreen.stepProgress(70);

            openDefaultScene();

            if (!viewMode.restoreWindowFile(MedicalConfig.WindowsFile, getDockContent))
            {
                drawingWindowController.createOneWaySplit();
            }

            options = new Options();

            splashScreen.stepProgress(20);

            basicForm.Show();
            basicForm.Activate();
            splashScreen.fadeAway();
            medicalController.start();
        }

        /// <summary>
        /// Stop the loop and begin shutdown procedures.
        /// </summary>
        public void stop()
        {
            medicalController.shutdown();
            viewMode.saveWindowFile(MedicalConfig.WindowsFile);
            drawingWindowController.saveCameraFile();
            drawingWindowController.destroyCameras();
        }

        /// <summary>
        /// Open the default scene.
        /// </summary>
        public void openDefaultScene()
        {
            changeScene(MedicalConfig.DefaultScene);
            stateController.clearStates();
        }

        /// <summary>
        /// Used when restoring window positions. Return the window matching the
        /// persistString or null if no match is found.
        /// </summary>
        /// <param name="persistString">A string describing the window.</param>
        /// <returns>The matching DockContent or null if none is found.</returns>
        public DockContent getDockContent(String persistString)
        {
            DockContent ret = null;
            ret = viewMode.restoreWindow(persistString);
            if (ret == null)
            {
                String name;
                Vector3 translation, lookAt;
                if (DrawingWindowHost.RestoreFromString(persistString, out name, out translation, out lookAt))
                {
                    ret = drawingWindowController.createDrawingWindowHost(name, translation, lookAt);
                }
            }
            return ret;
        }

        public void showOptions()
        {
            if (options.ShowDialog(basicForm) == DialogResult.OK)
            {
                drawingWindowController.recreateAllWindows();
                medicalController.MainTimer.FramerateCap = MedicalConfig.EngineConfig.MaxFPS;
            }
        }

        public void setOneWindowLayout()
        {
            drawingWindowController.createOneWaySplit();
        }

        public void setTwoWindowLayout()
        {
            drawingWindowController.createTwoWaySplit();
        }

        public void setThreeWindowLayout()
        {
            drawingWindowController.createThreeWayUpperSplit();
        }

        public void setFourWindowLayout()
        {
            drawingWindowController.createFourWaySplit();
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
            openDefaultScene();
            XmlTextReader textReader = null;
            try
            {
                textReader = new XmlTextReader(filename);
                SavedMedicalStates states = saver.restoreObject(textReader) as SavedMedicalStates;
                if (states != null)
                {
                    stateController.setStates(states);
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
            statePicker.setToDefault();
            viewMode.alertGUISceneUnloading();
            drawingWindowController.destroyCameras();
            if (medicalController.openScene(file))
            {
                drawingWindowController.createCameras(medicalController.MainTimer, medicalController.CurrentScene, medicalController.CurrentSceneDirectory);
                viewMode.alertGUISceneLoaded(medicalController.CurrentScene);
                navigationController.NavigationSet = TEMP_createNavigationState(drawingWindowController.SceneCameras);
                return true;
            }
            else
            {
                return false;
            }
        }

        internal void showStatePicker()
        {
            if (!statePicker.Visible)
            {
                basicForm.SuspendLayout();
                if (stateController.getNumStates() == 0)
                {
                    stateController.createAndAddState("Normal");
                }
                statePicker.startWizard();
                viewMode.hideWindows();
                viewMode.EnableToolbars = false;
                statePicker.Show(distortMode.DockPanel);
                basicForm.ResumeLayout();
            }
        }

        void statePicker_StateCreated(MedicalState state)
        {
            stateController.addState(state);
            stateGUI.CurrentBlend = stateController.getNumStates() - 1;
        }

        void statePicker_Finished()
        {
            basicForm.SuspendLayout();
            distortMode.hideWindows();
            viewMode.EnableToolbars = true;
            viewMode.restoreHiddenWindows();
            basicForm.ResumeLayout();
        }

        private void constructStatePicker()
        {
            statePicker.initialize(temporaryStateBlender);
            statePicker.StateCreated += statePicker_StateCreated;
            statePicker.Finished += statePicker_Finished;

            PresetStateSet leftGrowth = new PresetStateSet("Left Condyle Growth", Resource.ResourceRoot + "/Presets/LeftGrowth");
            loadPresetSet(leftGrowth);
            statePicker.addPresetStateSet(leftGrowth);

            PresetStateSet rightGrowth = new PresetStateSet("Right Condyle Growth", Resource.ResourceRoot + "/Presets/RightGrowth");
            loadPresetSet(rightGrowth);
            statePicker.addPresetStateSet(rightGrowth);

            PresetStateSet leftDegeneration = new PresetStateSet("Left Condyle Degeneration", Resource.ResourceRoot + "/Presets/LeftDegeneration");
            loadPresetSet(leftDegeneration);
            statePicker.addPresetStateSet(leftDegeneration);

            PresetStateSet rightDegeneration = new PresetStateSet("Right Condyle Degeneration", Resource.ResourceRoot + "/Presets/RightDegeneration");
            loadPresetSet(rightDegeneration);
            statePicker.addPresetStateSet(rightDegeneration);

            statePicker.addStatePanel(new DiscSpacePanel());
            statePicker.addStatePanel(new FossaStatePanel());
            statePicker.addStatePanel(new TeethStatePanel());
            statePicker.setToDefault();

            distortMode.addGUIElement(statePicker);
        }

        private void loadPresetSet(PresetStateSet presetStateSet)
        {
            using (Archive archive = FileSystem.OpenArchive(presetStateSet.SourceDirectory))
            {
                String[] files = archive.listFiles(presetStateSet.SourceDirectory, "*.pre", false);
                foreach (String file in files)
                {
                    XmlTextReader reader = new XmlTextReader(archive.openStream(file, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read));
                    BoneManipulatorPresetState preset = saver.restoreObject(reader) as BoneManipulatorPresetState;
                    if (preset != null)
                    {
                        presetStateSet.addPresetState(preset);
                    }
                    else
                    {
                        Log.Error("Could not load preset from file {0}. Object was not a BoneManipulatorPresetState.", file);
                    }
                    reader.Close();
                }
            }
        }

        private static NavigationStateSet TEMP_createNavigationState(SavedCameraController sceneCameras)
        {
            NavigationStateSet navigationSet = new NavigationStateSet();
            foreach (SavedCameraDefinition def in sceneCameras.getSavedCameras())
            {
                NavigationState state = new NavigationState(def.Name, def.LookAt, def.Position);
                navigationSet.addState(state);
            }
            //setup adjacent states
            //outer shell
            NavigationState target = navigationSet.getState("Midline Anterior");
            target.addTwoWayAdjacentState(navigationSet.getState("Midline Anterosuperior"), NavigationButtons.Up);
            target.addTwoWayAdjacentState(navigationSet.getState("Midline Anteroinferior"), NavigationButtons.Down);
            target.addTwoWayAdjacentState(navigationSet.getState("Right Lateral"), NavigationButtons.Left);
            target.addTwoWayAdjacentState(navigationSet.getState("Left Lateral"), NavigationButtons.Right);
            target.addTwoWayAdjacentState(navigationSet.getState("Occlusion Joint Anterior"), NavigationButtons.ZoomIn);

            target = navigationSet.getState("Left Lateral");
            target.addTwoWayAdjacentState(navigationSet.getState("Left Laterosuperior"), NavigationButtons.Up);
            target.addTwoWayAdjacentState(navigationSet.getState("Left Lateroinferior"), NavigationButtons.Down);
            target.addTwoWayAdjacentState(navigationSet.getState("Occlusion Joint Left"), NavigationButtons.ZoomIn);
            target.addTwoWayAdjacentState(navigationSet.getState("Midline Posterior"), NavigationButtons.Right);
            target.addTwoWayAdjacentState(navigationSet.getState("Midline Anterior"), NavigationButtons.Left);
            target.addTwoWayAdjacentState(navigationSet.getState("Left TMJ"), NavigationButtons.ZoomIn);

            target = navigationSet.getState("Right Lateral");
            target.addTwoWayAdjacentState(navigationSet.getState("Right Laterosuperior"), NavigationButtons.Up);
            target.addTwoWayAdjacentState(navigationSet.getState("Right Lateroinferior"), NavigationButtons.Down);
            target.addTwoWayAdjacentState(navigationSet.getState("Occlusion Joint Right"), NavigationButtons.ZoomIn);
            target.addTwoWayAdjacentState(navigationSet.getState("Midline Posterior"), NavigationButtons.Left);
            target.addTwoWayAdjacentState(navigationSet.getState("Midline Anterior"), NavigationButtons.Right);
            target.addTwoWayAdjacentState(navigationSet.getState("Right TMJ"), NavigationButtons.ZoomIn);

            target = navigationSet.getState("Midline Anterosuperior");
            target.addTwoWayAdjacentState(navigationSet.getState("Midline Superior"), NavigationButtons.Up);
            target.addTwoWayAdjacentState(navigationSet.getState("Right Laterosuperior"), NavigationButtons.Left);
            target.addTwoWayAdjacentState(navigationSet.getState("Left Laterosuperior"), NavigationButtons.Right);

            target = navigationSet.getState("Midline Superior");
            target.addAdjacentState(navigationSet.getState("Midline Posterosuperior"), NavigationButtons.Up);

            target = navigationSet.getState("Midline Posterosuperior");
            target.addAdjacentState(navigationSet.getState("Midline Superior"), NavigationButtons.Up);
            target.addTwoWayAdjacentState(navigationSet.getState("Right Laterosuperior"), NavigationButtons.Right);
            target.addTwoWayAdjacentState(navigationSet.getState("Left Laterosuperior"), NavigationButtons.Left);
            target.addTwoWayAdjacentState(navigationSet.getState("Midline Posterior"), NavigationButtons.Down);

            target = navigationSet.getState("Midline Posterior");
            target.addTwoWayAdjacentState(navigationSet.getState("Midline Posteroinferior"), NavigationButtons.Down);

            target = navigationSet.getState("Midline Anteroinferior");
            target.addTwoWayAdjacentState(navigationSet.getState("Left Lateroinferior"), NavigationButtons.Right);
            target.addTwoWayAdjacentState(navigationSet.getState("Right Lateroinferior"), NavigationButtons.Left);
            target.addTwoWayAdjacentState(navigationSet.getState("Midline Submental"), NavigationButtons.Down);
            target.addAdjacentState(navigationSet.getState("Occlusion Joint Anterior"), NavigationButtons.ZoomIn);

            target = navigationSet.getState("Midline Submental");
            target.addAdjacentState(navigationSet.getState("Midline Posteroinferior"), NavigationButtons.Down);

            target = navigationSet.getState("Midline Posteroinferior");
            target.addAdjacentState(navigationSet.getState("Midline Submental"), NavigationButtons.Down);
            target.addTwoWayAdjacentState(navigationSet.getState("Left Lateroinferior"), NavigationButtons.Left);
            target.addTwoWayAdjacentState(navigationSet.getState("Right Lateroinferior"), NavigationButtons.Right);
            target.addTwoWayAdjacentState(navigationSet.getState("Occlusion Joint Posterior"), NavigationButtons.ZoomIn);

            target = navigationSet.getState("Midline Posterosuperior");
            target.addTwoWayAdjacentState(navigationSet.getState("Right Laterosuperior"), NavigationButtons.Right);
            target.addTwoWayAdjacentState(navigationSet.getState("Left Laterosuperior"), NavigationButtons.Left);

            //Occlusion
            target = navigationSet.getState("Occlusion Joint Posterior");
            target.addTwoWayAdjacentState(navigationSet.getState("Occlusion Joint Left"), NavigationButtons.Left);
            target.addTwoWayAdjacentState(navigationSet.getState("Occlusion Joint Right"), NavigationButtons.Right);
            target.addTwoWayAdjacentState(navigationSet.getState("Dentition Anterior Lingual"), NavigationButtons.ZoomIn);

            target = navigationSet.getState("Occlusion Joint Anterior");
            target.addTwoWayAdjacentState(navigationSet.getState("Occlusion Joint Left"), NavigationButtons.Right);
            target.addTwoWayAdjacentState(navigationSet.getState("Occlusion Joint Right"), NavigationButtons.Left);
            target.addTwoWayAdjacentState(navigationSet.getState("Dentition Anterior Labial"), NavigationButtons.ZoomIn);

            target = navigationSet.getState("Occlusion Joint Left");
            target.addTwoWayAdjacentState(navigationSet.getState("Dentition Left Posterior Buccal"), NavigationButtons.ZoomIn);

            target = navigationSet.getState("Occlusion Joint Right");
            target.addTwoWayAdjacentState(navigationSet.getState("Dentition Right Posterior Buccal"), NavigationButtons.ZoomIn);

            //Dentition
            target = navigationSet.getState("Dentition Anterior Labial");
            target.addTwoWayAdjacentState(navigationSet.getState("Dentition Left Posterior Buccal"), NavigationButtons.Right);
            target.addTwoWayAdjacentState(navigationSet.getState("Dentition Right Posterior Buccal"), NavigationButtons.Left);

            target = navigationSet.getState("Dentition Left Posterior Buccal");
            target.addTwoWayAdjacentState(navigationSet.getState("Dentition Left Posterior Lingual"), NavigationButtons.Right);

            target = navigationSet.getState("Dentition Right Posterior Buccal");
            target.addTwoWayAdjacentState(navigationSet.getState("Dentition Right Posterior Lingual"), NavigationButtons.Left);

            target = navigationSet.getState("Dentition Anterior Lingual");
            target.addTwoWayAdjacentState(navigationSet.getState("Dentition Left Posterior Lingual"), NavigationButtons.Left);
            target.addTwoWayAdjacentState(navigationSet.getState("Dentition Right Posterior Lingual"), NavigationButtons.Right);

            //Joint Left
            target = navigationSet.getState("Left TMJ");
            target.addTwoWayAdjacentState(navigationSet.getState("Joint Lat Left"), NavigationButtons.ZoomIn);

            target = navigationSet.getState("Joint Lat Left");
            target.addTwoWayAdjacentState(navigationSet.getState("Joint Ant Left"), NavigationButtons.Left);
            target.addTwoWayAdjacentState(navigationSet.getState("Joint Post Left"), NavigationButtons.Right);
            target.addAdjacentState(navigationSet.getState("Joint Sup Left"), NavigationButtons.Up);
            target.addAdjacentState(navigationSet.getState("Joint Inf Left"), NavigationButtons.Down);

            target = navigationSet.getState("Joint Med Left");
            target.addTwoWayAdjacentState(navigationSet.getState("Joint Ant Left"), NavigationButtons.Right);
            target.addTwoWayAdjacentState(navigationSet.getState("Joint Post Left"), NavigationButtons.Left);
            target.addAdjacentState(navigationSet.getState("Joint Sup Left"), NavigationButtons.Up);
            target.addAdjacentState(navigationSet.getState("Joint Inf Left"), NavigationButtons.Down);

            target = navigationSet.getState("Joint Sup Left");
            target.addAdjacentState(navigationSet.getState("Joint Med Left"), NavigationButtons.Right);
            target.addAdjacentState(navigationSet.getState("Joint Lat Left"), NavigationButtons.Left);
            target.addAdjacentState(navigationSet.getState("Joint Post Left"), NavigationButtons.Down);
            target.addAdjacentState(navigationSet.getState("Joint Ant Left"), NavigationButtons.Up);

            target = navigationSet.getState("Joint Ant Left");
            target.addAdjacentState(navigationSet.getState("Joint Sup Left"), NavigationButtons.Up);

            target = navigationSet.getState("Joint Post Left");
            target.addAdjacentState(navigationSet.getState("Joint Sup Left"), NavigationButtons.Up);
            target.addAdjacentState(navigationSet.getState("Joint Inf Left"), NavigationButtons.Down);

            target = navigationSet.getState("Joint Inf Left");
            target.addAdjacentState(navigationSet.getState("Joint Med Left"), NavigationButtons.Right);
            target.addAdjacentState(navigationSet.getState("Joint Lat Left"), NavigationButtons.Left);
            target.addAdjacentState(navigationSet.getState("Joint Post Left"), NavigationButtons.Down);
            target.addTwoWayAdjacentState(navigationSet.getState("Joint Ant Left"), NavigationButtons.Up);

            //Joint Right
            target = navigationSet.getState("Right TMJ");
            target.addTwoWayAdjacentState(navigationSet.getState("Joint Lat Right"), NavigationButtons.ZoomIn);

            target = navigationSet.getState("Joint Lat Right");
            target.addTwoWayAdjacentState(navigationSet.getState("Joint Ant Right"), NavigationButtons.Right);
            target.addTwoWayAdjacentState(navigationSet.getState("Joint Post Right"), NavigationButtons.Left);
            target.addAdjacentState(navigationSet.getState("Joint Sup Right"), NavigationButtons.Up);
            target.addAdjacentState(navigationSet.getState("Joint Inf Right"), NavigationButtons.Down);

            target = navigationSet.getState("Joint Med Right");
            target.addTwoWayAdjacentState(navigationSet.getState("Joint Ant Right"), NavigationButtons.Left);
            target.addTwoWayAdjacentState(navigationSet.getState("Joint Post Right"), NavigationButtons.Right);
            target.addAdjacentState(navigationSet.getState("Joint Sup Right"), NavigationButtons.Up);
            target.addAdjacentState(navigationSet.getState("Joint Inf Right"), NavigationButtons.Down);

            target = navigationSet.getState("Joint Sup Right");
            target.addAdjacentState(navigationSet.getState("Joint Med Right"), NavigationButtons.Left);
            target.addAdjacentState(navigationSet.getState("Joint Lat Right"), NavigationButtons.Right);
            target.addAdjacentState(navigationSet.getState("Joint Post Right"), NavigationButtons.Down);
            target.addAdjacentState(navigationSet.getState("Joint Ant Right"), NavigationButtons.Up);

            target = navigationSet.getState("Joint Ant Right");
            target.addAdjacentState(navigationSet.getState("Joint Sup Right"), NavigationButtons.Up);

            target = navigationSet.getState("Joint Post Right");
            target.addAdjacentState(navigationSet.getState("Joint Sup Right"), NavigationButtons.Up);
            target.addAdjacentState(navigationSet.getState("Joint Inf Right"), NavigationButtons.Down);

            target = navigationSet.getState("Joint Inf Right");
            target.addAdjacentState(navigationSet.getState("Joint Med Right"), NavigationButtons.Left);
            target.addAdjacentState(navigationSet.getState("Joint Lat Right"), NavigationButtons.Right);
            target.addAdjacentState(navigationSet.getState("Joint Post Right"), NavigationButtons.Down);
            target.addTwoWayAdjacentState(navigationSet.getState("Joint Ant Right"), NavigationButtons.Up);

            return navigationSet;
        }
        /**
         * Code for navigation states
//create
navigation = new NavigationOverlay(name, eventManager, this, navigationController);
navigation.ShowOverlay = true;
navigation.setNavigationState(navigationController.findClosestState(translation));

//destroy
if (navigation != null)
{
    navigation.Dispose();
}

//create camera (scene created)
mainTimer.addFixedUpdateListener(navigation);

//destroy camera (scene destroyed)
mainTimer.removeFixedUpdateListener(navigation);

//pre find visible
Vector3 mousePos = eventManager.Mouse.getAbsMouse();
navigation.setVisible(callingCameraRender && this.allowMotion((int)mousePos.x, (int)mousePos.y));
         */
    }
}