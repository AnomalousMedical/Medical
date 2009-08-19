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

namespace Medical.Controller
{
    public class BasicController : IDisposable
    {
        private const String sceneFileName = "/Scenes/BasicScene.sim.xml";

        private MedicalController medicalController;
        private DrawingWindowController drawingWindowController;
        private BasicForm basicForm;
        private GUIElementController guiElements;
        private MedicalStateController stateController = new MedicalStateController();
        private MedicalStateGUI stateGUI;
        private XmlSaver saver = new XmlSaver();
        private Options options = null;
        private StatePicker statePicker = new StatePicker();

        /// <summary>
        /// Constructor.
        /// </summary>
        public BasicController()
        {
            MedicalConfig config = new MedicalConfig(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Anomalous Software/Basic");
            constructStatePicker();
        }

        /// <summary>
        /// Dispose function.
        /// </summary>
        public void Dispose()
        {
            if (medicalController != null)
            {
                medicalController.Dispose();
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
        }

        /// <summary>
        /// Start running the program.
        /// </summary>
        public void go()
        {
            SplashScreen splash = new SplashScreen();
            splash.Show();

            basicForm = new BasicForm();
            basicForm.initialize(this);
            medicalController = new MedicalController();
            medicalController.intialize(basicForm);

            drawingWindowController = new DrawingWindowController(MedicalConfig.CamerasFile);
            drawingWindowController.initialize(basicForm.DockPanel, medicalController.EventManager, PluginManager.Instance.RendererPlugin, MedicalConfig.ConfigFile);

            guiElements = new GUIElementController(basicForm.DockPanel, basicForm.ToolStrip, medicalController);

            //Add common gui elements
            //LayersControl layersControl = new LayersControl();
            //guiElements.addGUIElement(layersControl);

            PictureControl pictureControl = new PictureControl();
            pictureControl.initialize(medicalController, drawingWindowController);
            guiElements.addGUIElement(pictureControl);

            stateGUI = new MedicalStateGUI();
            stateGUI.initialize(stateController);
            guiElements.addGUIElement(stateGUI);

            SavedCameraGUI savedCameraGUI = new SavedCameraGUI();
            savedCameraGUI.initialize(drawingWindowController);
            guiElements.addGUIElement(savedCameraGUI);

            //Add specific gui elements
            MuscleControl muscleControl = new MuscleControl();
            guiElements.addGUIElement(muscleControl);

            SimpleLayerControl simpleLayer = new SimpleLayerControl();
            guiElements.addGUIElement(simpleLayer);

            PredefinedLayerControl predefinedLayers = new PredefinedLayerControl();
            guiElements.addGUIElement(predefinedLayers);

            openDefaultScene();

            if (!basicForm.restoreWindows(MedicalConfig.WindowsFile, getDockContent))
            {
                drawingWindowController.createOneWaySplit();
            }

            options = new Options();

            basicForm.Show();
            splash.Close();
            medicalController.start();
        }

        /// <summary>
        /// Stop the loop and begin shutdown procedures.
        /// </summary>
        public void stop()
        {
            medicalController.shutdown();
            basicForm.saveWindows(MedicalConfig.WindowsFile);
            drawingWindowController.saveCameraFile();
            drawingWindowController.destroyCameras();
        }

        /// <summary>
        /// Open the default scene.
        /// </summary>
        public void openDefaultScene()
        {
            if (File.Exists(Resource.ResourceRoot + sceneFileName))
            {
                changeScene(Resource.ResourceRoot + sceneFileName);
            }
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
            ret = guiElements.restoreWindow(persistString);
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
                SavedMedicalStates states = stateController.getSavedState();
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

        /// <summary>
        /// Change the scene to the specified filename.
        /// </summary>
        /// <param name="filename"></param>
        private void changeScene(String filename)
        {
            statePicker.setToDefault();
            guiElements.alertGUISceneUnloading();
            drawingWindowController.destroyCameras();
            if (medicalController.openScene(filename))
            {
                drawingWindowController.createCameras(medicalController.MainTimer, medicalController.CurrentScene);
                guiElements.alertGUISceneLoaded(medicalController.CurrentScene);
            }
            else
            {
                MessageBox.Show(String.Format("Could not open scene {0}.", filename), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal void showStatePicker()
        {
            statePicker.startWizard();
            statePicker.ShowDialog(basicForm);
            if (statePicker.WizardFinished)
            {
                if (stateController.getNumStates() == 0)
                {
                    stateController.createState("Normal");
                }
                stateController.addState(statePicker.CreatedState);
                stateGUI.playAll();
            }
        }

        private void constructStatePicker()
        {
            statePicker.addPresetStateSet(createGrowthSet("Left", "left"), Resources.ResourceManager);
            statePicker.addPresetStateSet(createGrowthSet("Right", "right"), Resources.ResourceManager);
            statePicker.addPresetStateSet(createDegenerationSet("Left", "left"), Resources.ResourceManager);
            statePicker.addPresetStateSet(createDegenerationSet("Right", "right"), Resources.ResourceManager);
            statePicker.addStatePanel(new DiscSpacePanel());
            statePicker.addStatePanel(new FossaStatePanel());
            statePicker.addStatePanel(new TeethStatePanel());
        }

        private PresetStateSet createGrowthSet(String sidePretty, String sideBoneBase)
        {
            BoneManipulatorPresetState boneManipulatorPreset;
            String ramusHeight = sideBoneBase + "RamusHeightMandible";
            String condyleHeight = sideBoneBase + "CondyleHeightMandible";
            String condyleRotation = sideBoneBase + "CondyleRotationMandible";
            String mandibluarNotch = sideBoneBase + "MandibularNotchMandible";
            String antegonialNotch = sideBoneBase + "AntegonialNotchMandible";
            PresetStateSet condyleGrowth = new PresetStateSet(sidePretty + " Condyle Growth");

            boneManipulatorPreset = new BoneManipulatorPresetState("Normal", "Normal", sidePretty + "GrowthNormal");
            boneManipulatorPreset.addPosition(ramusHeight, 0.1960605f);
            boneManipulatorPreset.addPosition(condyleHeight, 0.1121006f);
            boneManipulatorPreset.addPosition(condyleRotation, 0.0f);
            boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            boneManipulatorPreset.addPosition(antegonialNotch, 0.0f);
            condyleGrowth.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("No Condylar Compensation 1", "Mild Deficiency", sidePretty + "GrowthMildNoCompensation1");
            boneManipulatorPreset.addPosition(ramusHeight, 0.1960605f);
            boneManipulatorPreset.addPosition(condyleHeight, 0.2f);
            boneManipulatorPreset.addPosition(condyleRotation, 0.0f);
            boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            boneManipulatorPreset.addPosition(antegonialNotch, 0.0f);
            condyleGrowth.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("No Condylar Compensation 2", "Mild Deficiency", sidePretty + "GrowthMildNoCompensation2");
            boneManipulatorPreset.addPosition(ramusHeight, 0.1960605f);
            boneManipulatorPreset.addPosition(condyleHeight, 0.3f);
            boneManipulatorPreset.addPosition(condyleRotation, 0.0f);
            boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            boneManipulatorPreset.addPosition(antegonialNotch, 0.0f);
            condyleGrowth.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("No Condylar Compensation 3", "Mild Deficiency", sidePretty + "GrowthMildNoCompensation3");
            boneManipulatorPreset.addPosition(ramusHeight, 0.1960605f);
            boneManipulatorPreset.addPosition(condyleHeight, 0.4f);
            boneManipulatorPreset.addPosition(condyleRotation, 0.0f);
            boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            boneManipulatorPreset.addPosition(antegonialNotch, 0.0f);
            condyleGrowth.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("Condylar Compensation", "Mild Deficiency", sidePretty + "GrowthMildCompensation");
            boneManipulatorPreset.addPosition(ramusHeight, 0.1960605f);
            boneManipulatorPreset.addPosition(condyleHeight, 0.4f);
            boneManipulatorPreset.addPosition(condyleRotation, 0.4f);
            boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            boneManipulatorPreset.addPosition(antegonialNotch, 0.0f);
            condyleGrowth.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("Condylar Compensation 1", "Moderate Deficiency", sidePretty + "GrowthModerateCompensation1");
            boneManipulatorPreset.addPosition(ramusHeight, 0.1960605f);
            boneManipulatorPreset.addPosition(condyleHeight, 0.7f);
            boneManipulatorPreset.addPosition(condyleRotation, 0.2f);
            boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            boneManipulatorPreset.addPosition(antegonialNotch, 0.7f);
            condyleGrowth.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("Condylar Compensation 2", "Moderate Deficiency", sidePretty + "GrowthModerateCompensation2");
            boneManipulatorPreset.addPosition(ramusHeight, 0.1960605f);
            boneManipulatorPreset.addPosition(condyleHeight, 0.7f);
            boneManipulatorPreset.addPosition(condyleRotation, 0.7f);
            boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            boneManipulatorPreset.addPosition(antegonialNotch, 0.8f);
            condyleGrowth.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("Condylar Compensation 3", "Moderate Deficiency", sidePretty + "GrowthModerateCompensation3");
            boneManipulatorPreset.addPosition(ramusHeight, 0.1960605f);
            boneManipulatorPreset.addPosition(condyleHeight, 0.7f);
            boneManipulatorPreset.addPosition(condyleRotation, 1.0f);
            boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            boneManipulatorPreset.addPosition(antegonialNotch, 1.0f);
            condyleGrowth.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("No Condylar Compensation", "Moderate Deficiency", sidePretty + "GrowthModerateNoCompensation");
            boneManipulatorPreset.addPosition(ramusHeight, 0.1960605f);
            boneManipulatorPreset.addPosition(condyleHeight, 0.7f);
            boneManipulatorPreset.addPosition(condyleRotation, 0.0f);
            boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            boneManipulatorPreset.addPosition(antegonialNotch, 0.0f);
            condyleGrowth.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("Total Mandible 1", "Extreme Deficiency", sidePretty + "GrowthExtremeTotalMandible1");
            boneManipulatorPreset.addPosition(ramusHeight, 0.09f);
            boneManipulatorPreset.addPosition(condyleHeight, 0.95f);
            boneManipulatorPreset.addPosition(condyleRotation, 0.9f);
            boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            boneManipulatorPreset.addPosition(antegonialNotch, 0.6f);
            condyleGrowth.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("Total Mandible 2", "Extreme Deficiency", sidePretty + "GrowthExtremeTotalMandible2");
            boneManipulatorPreset.addPosition(ramusHeight, 0.1f);
            boneManipulatorPreset.addPosition(condyleHeight, 0.98f);
            boneManipulatorPreset.addPosition(condyleRotation, 1.0f);
            boneManipulatorPreset.addPosition(mandibluarNotch, 0.1f);
            boneManipulatorPreset.addPosition(antegonialNotch, 0.85f);
            condyleGrowth.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("Total Mandible 3", "Extreme Deficiency", sidePretty + "GrowthExtremeTotalMandible3");
            boneManipulatorPreset.addPosition(ramusHeight, 0.62f);
            boneManipulatorPreset.addPosition(condyleHeight, 1.0f);
            boneManipulatorPreset.addPosition(condyleRotation, 1.0f);
            boneManipulatorPreset.addPosition(mandibluarNotch, 0.25f);
            boneManipulatorPreset.addPosition(antegonialNotch, 1.0f);
            condyleGrowth.addPresetState(boneManipulatorPreset);

            return condyleGrowth;
        }

        private PresetStateSet createDegenerationSet(String sidePretty, String sideBoneBase)
        {
            String condyleDegenerationMandible = sideBoneBase + "CondyleDegenerationMandible";
            String lateralPoleMandible = sideBoneBase + "LateralPoleMandible";
            String medialPoleScale = sideBoneBase + "MedialPoleScaleMandible";
            BoneManipulatorPresetState boneManipulatorPreset;
            PresetStateSet condyleDegeneration = new PresetStateSet(sidePretty + " Condyle Degeneration");

            boneManipulatorPreset = new BoneManipulatorPresetState("Normal", "Normal", sidePretty + "DegenerationNormal");
            boneManipulatorPreset.addPosition(condyleDegenerationMandible, 0.0f);
            boneManipulatorPreset.addPosition(lateralPoleMandible, 0.0f);
            boneManipulatorPreset.addPosition(medialPoleScale, 0.0f);
            condyleDegeneration.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("Mild Degeneration", "Total Degeneration", sidePretty + "DegenerationMild");
            boneManipulatorPreset.addPosition(condyleDegenerationMandible, 0.2f);
            boneManipulatorPreset.addPosition(lateralPoleMandible, 0.2f);
            boneManipulatorPreset.addPosition(medialPoleScale, 0.2f);
            condyleDegeneration.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("Moderate Degeneration", "Total Degeneration", sidePretty + "DegenerationModerate");
            boneManipulatorPreset.addPosition(condyleDegenerationMandible, 0.4f);
            boneManipulatorPreset.addPosition(lateralPoleMandible, 0.4f);
            boneManipulatorPreset.addPosition(medialPoleScale, 0.4f);
            condyleDegeneration.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("Severe Degeneration", "Total Degeneration", sidePretty + "DegenerationSevere");
            boneManipulatorPreset.addPosition(condyleDegenerationMandible, 0.7f);
            boneManipulatorPreset.addPosition(lateralPoleMandible, 0.7f);
            boneManipulatorPreset.addPosition(medialPoleScale, 0.7f);
            condyleDegeneration.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("Extreme Degeneration", "Total Degeneration", sidePretty + "DegenerationExtreme");
            boneManipulatorPreset.addPosition(condyleDegenerationMandible, 1.0f);
            boneManipulatorPreset.addPosition(lateralPoleMandible, 1.0f);
            boneManipulatorPreset.addPosition(medialPoleScale, 1.0f);
            condyleDegeneration.addPresetState(boneManipulatorPreset);

            return condyleDegeneration;
        }
    }
}