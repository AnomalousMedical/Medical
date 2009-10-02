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
        private MedicalStateController stateController = new MedicalStateController();
        private MedicalStateGUI stateGUI;
        private XmlSaver saver = new XmlSaver();
        private Options options = null;
        private StatePickerController statePicker;
        private ImageRenderer imageRenderer;
        private NavigationController navigationController;
        private WatermarkController watermarkController;
        private TemporaryStateBlender temporaryStateBlender;
        private PresetStatePanel leftGrowthPanel;
        private PresetStatePanel rightGrowthPanel;
        private PresetStatePanel leftDegenerationPanel;
        private PresetStatePanel rightDegenerationPanel;
        private PresetStatePanel leftDiscPanel;
        private PresetStatePanel rightDiscPanel;
        private PresetStatePanel leftFossaPanel;
        private PresetStatePanel rightFossaPanel;
        private ScenePicker scenePicker;
        private LayerController layerController;
        private MuscleControl muscleControl;

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
            ProgressDialog splashScreen = new ProgressDialog(Resources.articulometricsclinic);
            splashScreen.fadeIn();
            splashScreen.ProgressMaximum = 100;

            basicForm = new BasicForm();
            basicForm.initialize(this);
            medicalController = new MedicalController();
            medicalController.intialize(basicForm);

            splashScreen.stepProgress(10);

            drawingWindowController = new DrawingWindowController();
            drawingWindowController.AllowRotation = false;
            drawingWindowController.AllowZoom = false;
            drawingWindowController.initialize(basicForm.DockPanel, medicalController.EventManager, PluginManager.Instance.RendererPlugin, MedicalConfig.ConfigFile);

            navigationController = new NavigationController(drawingWindowController, medicalController.EventManager, medicalController.MainTimer);
            layerController = new LayerController();

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
            savedCameraGUI.initialize(drawingWindowController, MedicalConfig.CamerasFile, navigationController);
            viewMode.addGUIElement(savedCameraGUI);

            scenePicker = new ScenePicker();
            scenePicker.initialize();

            //Add specific gui elements
            muscleControl = new MuscleControl();
            viewMode.addGUIElement(muscleControl);

            SimpleLayerControl simpleLayer = new SimpleLayerControl();
            viewMode.addGUIElement(simpleLayer);

            viewMode.EnableToolbars = true;

            //Configure distort mode
            constructStatePicker();

            splashScreen.stepProgress(70);

            openDefaultScene();

            if (!viewMode.restoreWindowFile(MedicalConfig.WindowsFile, getDockContent))
            {
                setOneWindowLayout();
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
            viewMode.saveWindowFile(MedicalConfig.WindowsFile);
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
            statePicker.setToDefault();
            viewMode.alertGUISceneUnloading();
            drawingWindowController.destroyCameras();
            if (medicalController.openScene(file))
            {
                drawingWindowController.createCameras(medicalController.MainTimer, medicalController.CurrentScene, medicalController.CurrentSceneDirectory);
                viewMode.alertGUISceneLoaded(medicalController.CurrentScene);

                SimSubScene defaultScene = medicalController.CurrentScene.getDefaultSubScene();
                if (defaultScene != null)
                {
                    SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();
                    String layersFile = medicalController.CurrentSceneDirectory + "/" + medicalScene.LayersFile;
                    layerController.loadLayerStateSet(layersFile);
                    String cameraFile = medicalController.CurrentSceneDirectory + "/" + medicalScene.CameraFile;
                    using (Archive archive = FileSystem.OpenArchive(cameraFile))
                    {
                        if(archive.exists(cameraFile))
                        {
                            try
                            {
                                using (XmlTextReader textReader = new XmlTextReader(archive.openStream(cameraFile, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read)))
                                {
                                    NavigationStateSet navigation = NavigationSerializer.readNavigationStateSet(textReader);
                                    navigationController.NavigationSet = navigation;
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Debug("Error loading navigation file.\n{0}", ex.Message);
                            }
                        }
                    }
                    updateStatePicker(medicalController.CurrentSceneDirectory + "/" + medicalScene.PresetDirectory);
                }
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
                muscleControl.stopPlayback();
                viewMode.hideWindows();
                viewMode.EnableToolbars = false;
                statePicker.startWizard(drawingWindowController.getActiveWindow().DrawingWindow);
                statePicker.show();
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
            viewMode.EnableToolbars = true;
            viewMode.restoreHiddenWindows();
            basicForm.ResumeLayout();
        }

        private void constructStatePicker()
        {
            statePicker = new StatePickerController(new GUIElementController(basicForm.DockPanel, basicForm.ToolStrip, medicalController), temporaryStateBlender, navigationController, layerController);
            statePicker.StateCreated += statePicker_StateCreated;
            statePicker.Finished += statePicker_Finished;

            leftGrowthPanel = new PresetStatePanel();
            leftGrowthPanel.Text = "Left Condyle Growth";
            leftGrowthPanel.NavigationState = "Left Lateral";
            leftGrowthPanel.LayerState = "MandibleSizeLayers";
            statePicker.addStatePanel(leftGrowthPanel);

            leftDegenerationPanel = new PresetStatePanel();
            leftDegenerationPanel.Text = "Left Condyle Degeneration";
            leftDegenerationPanel.NavigationState = "Left TMJ";
            leftDegenerationPanel.LayerState = "MandibleSizeLayers";
            statePicker.addStatePanel(leftDegenerationPanel);

            leftDiscPanel = new PresetStatePanel();
            leftDiscPanel.Text = "Left Disc";
            leftDiscPanel.NavigationState = "Left TMJ";
            leftDiscPanel.LayerState = "DiscLayers";
            statePicker.addStatePanel(leftDiscPanel);

            leftFossaPanel = new PresetStatePanel();
            leftFossaPanel.Text = "Left Fossa";
            leftFossaPanel.NavigationState = "Left TMJ";
            leftFossaPanel.LayerState = "FossaLayers";
            statePicker.addStatePanel(leftFossaPanel);

            rightGrowthPanel = new PresetStatePanel();
            rightGrowthPanel.Text = "Right Condyle Growth";
            rightGrowthPanel.NavigationState = "Right Lateral";
            rightGrowthPanel.LayerState = "MandibleSizeLayers";
            statePicker.addStatePanel(rightGrowthPanel);

            rightDegenerationPanel = new PresetStatePanel();
            rightDegenerationPanel.Text = "Right Condyle Degeneration";
            rightDegenerationPanel.NavigationState = "Right TMJ";
            rightDegenerationPanel.LayerState = "MandibleSizeLayers";
            statePicker.addStatePanel(rightDegenerationPanel);

            rightDiscPanel = new PresetStatePanel();
            rightDiscPanel.Text = "Right Disc";
            rightDiscPanel.NavigationState = "Right TMJ";
            rightDiscPanel.LayerState = "DiscLayers";
            statePicker.addStatePanel(rightDiscPanel);

            rightFossaPanel = new PresetStatePanel();
            rightFossaPanel.Text = "Right Fossa";
            rightFossaPanel.NavigationState = "Right TMJ";
            rightFossaPanel.LayerState = "FossaLayers";
            statePicker.addStatePanel(rightFossaPanel);

            statePicker.addStatePanel(new BottomTeethRemovalPanel());
            statePicker.addStatePanel(new TopTeethRemovalPanel());
            statePicker.addStatePanel(new TeethAdaptationPanel());
            statePicker.setToDefault();
        }
        
        private void updateStatePicker(String rootDirectory)
        {
            PresetStateSet leftGrowth = new PresetStateSet("Left Condyle Growth", rootDirectory + "/LeftGrowth");
            loadPresetSet(leftGrowth);
            leftGrowthPanel.clear();
            leftGrowthPanel.initialize(leftGrowth);

            PresetStateSet rightGrowth = new PresetStateSet("Right Condyle Growth", rootDirectory + "/RightGrowth");
            loadPresetSet(rightGrowth);
            rightGrowthPanel.clear();
            rightGrowthPanel.initialize(rightGrowth);

            PresetStateSet leftDegeneration = new PresetStateSet("Left Condyle Degeneration", rootDirectory + "/LeftDegeneration");
            loadPresetSet(leftDegeneration);
            leftDegenerationPanel.clear();
            leftDegenerationPanel.initialize(leftDegeneration);

            PresetStateSet rightDegeneration = new PresetStateSet("Right Condyle Degeneration", rootDirectory + "/RightDegeneration");
            loadPresetSet(rightDegeneration);
            rightDegenerationPanel.clear();
            rightDegenerationPanel.initialize(rightDegeneration);

            PresetStateSet leftDisc = new PresetStateSet("Left Disc", rootDirectory + "/LeftDisc");
            loadPresetSet(leftDisc);
            leftDiscPanel.clear();
            leftDiscPanel.initialize(leftDisc);

            PresetStateSet rightDisc = new PresetStateSet("Right Disc", rootDirectory + "/RightDisc");
            loadPresetSet(rightDisc);
            rightDiscPanel.clear();
            rightDiscPanel.initialize(rightDisc);

            PresetStateSet leftFossa = new PresetStateSet("Left Fossa", rootDirectory + "/LeftFossa");
            loadPresetSet(leftFossa);
            leftFossaPanel.clear();
            leftFossaPanel.initialize(leftFossa);

            PresetStateSet rightFossa = new PresetStateSet("Right Fossa", rootDirectory + "/RightFossa");
            loadPresetSet(rightFossa);
            rightFossaPanel.clear();
            rightFossaPanel.initialize(rightFossa);
        }

        private void loadPresetSet(PresetStateSet presetStateSet)
        {
            using (Archive archive = FileSystem.OpenArchive(presetStateSet.SourceDirectory))
            {
                String[] files = archive.listFiles(presetStateSet.SourceDirectory, "*.pre", false);
                foreach (String file in files)
                {
                    XmlTextReader reader = new XmlTextReader(archive.openStream(file, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read));
                    PresetState preset = saver.restoreObject(reader) as PresetState;
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
    }
}