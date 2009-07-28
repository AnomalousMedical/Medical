using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using WeifenLuo.WinFormsUI.Docking;
using Engine;
using System.Windows.Forms;
using Medical.GUI.StateWizard;
using System.IO;
using Engine.Resources;
using Engine.Saving;
using Engine.Saving.XMLSaver;
using System.Xml;

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
        private StateWizardForm stateWizard = new StateWizardForm();
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
            if (stateWizard != null)
            {
                stateWizard.Dispose();
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

        public void showStateWizard()
        {
            if (stateController.getNumStates() == 0)
            {
                stateController.createState("Normal");
            }
            stateWizard.startWizard();
            stateWizard.ShowDialog(basicForm);
            if (stateWizard.WizardFinished)
            {
                stateController.addState(stateWizard.CreatedState);
                stateGUI.playAll();
            }
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
            BoneManipulatorPresetState boneManipulatorPreset;
            DiscPresetState discPreset;
            FossaPresetState fossaPreset;

            //Left condyle growth
            PresetStateSet leftCondyleGrowth = new PresetStateSet("Left Condyle Growth");

            boneManipulatorPreset = new BoneManipulatorPresetState("Normal", "Normal", "GrowthNormalLeft");
            boneManipulatorPreset.addPosition("leftRamusHeightMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftCondyleHeightMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftCondyleRotationMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftMandibularNotchMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftAntegonialNotchMandible", 0.0f);
            leftCondyleGrowth.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("No Condylar Compensation 1", "Mild Deficiency", "GrowthMildDefiencyCondylarCompensation");
            boneManipulatorPreset.addPosition("leftRamusHeightMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftCondyleHeightMandible", 0.1f);
            boneManipulatorPreset.addPosition("leftCondyleRotationMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftMandibularNotchMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftAntegonialNotchMandible", 0.0f);
            leftCondyleGrowth.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("No Condylar Compensation 2", "Mild Deficiency", "GrowthMildDefiencyCondylarCompensation");
            boneManipulatorPreset.addPosition("leftRamusHeightMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftCondyleHeightMandible", 0.2f);
            boneManipulatorPreset.addPosition("leftCondyleRotationMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftMandibularNotchMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftAntegonialNotchMandible", 0.0f);
            leftCondyleGrowth.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("No Condylar Compensation 3", "Mild Deficiency", "GrowthMildDefiencyCondylarCompensation");
            boneManipulatorPreset.addPosition("leftRamusHeightMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftCondyleHeightMandible", 0.3f);
            boneManipulatorPreset.addPosition("leftCondyleRotationMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftMandibularNotchMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftAntegonialNotchMandible", 0.0f);
            leftCondyleGrowth.addPresetState(boneManipulatorPreset);

            boneManipulatorPreset = new BoneManipulatorPresetState("Condylar Compensation", "Mild Deficiency", "GrowthMildDefiencyCondylarCompensation");
            boneManipulatorPreset.addPosition("leftRamusHeightMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftCondyleHeightMandible", 0.3f);
            boneManipulatorPreset.addPosition("leftCondyleRotationMandible", 0.4f);
            boneManipulatorPreset.addPosition("leftMandibularNotchMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftAntegonialNotchMandible", 0.0f);
            leftCondyleGrowth.addPresetState(boneManipulatorPreset);

            //Right Condyle Growth
            PresetStateSet rightCondyleGrowth = new PresetStateSet("Right Condyle Growth");

            boneManipulatorPreset = new BoneManipulatorPresetState("Normal", "Normal", "GrowthNormalRight");
            boneManipulatorPreset.addPosition("rightRamusHeightMandible", 0.0f);
            boneManipulatorPreset.addPosition("rightCondyleHeightMandible", 0.0f);
            boneManipulatorPreset.addPosition("rightCondyleRotationMandible", 0.0f);
            boneManipulatorPreset.addPosition("rightMandibularNotchMandible", 0.0f);
            boneManipulatorPreset.addPosition("rightAntegonialNotchMandible", 0.0f);
            rightCondyleGrowth.addPresetState(boneManipulatorPreset);

            //Left condyle degeneration
            PresetStateSet leftCondyleDegeneration = new PresetStateSet("Left Condyle Degeneration");

            boneManipulatorPreset = new BoneManipulatorPresetState("Normal", "Normal", "DegenerationNormalLeft");
            boneManipulatorPreset.addPosition("leftCondyleDegenerationMandible", 0.0f);
            leftCondyleDegeneration.addPresetState(boneManipulatorPreset);

            //Right condyle degeneration
            PresetStateSet rightCondyleDegeneration = new PresetStateSet("Right Condyle Degeneration");

            boneManipulatorPreset = new BoneManipulatorPresetState("Normal", "Normal", "DegenerationNormalLeft");
            boneManipulatorPreset.addPosition("rightCondyleDegenerationMandible", 0.0f);
            rightCondyleDegeneration.addPresetState(boneManipulatorPreset);

            //Left Disc
            PresetStateSet leftDiscSpace = new PresetStateSet("Left Disc Space");
            discPreset = new DiscPresetState("Normal", "Normal", "NormalLeftDisc");
            discPreset.addPosition("LeftTMJDisc", new Vector3(0.0f, -0.302f, 0.0f));
            leftDiscSpace.addPresetState(discPreset);

            //Right Disc
            PresetStateSet rightDiscSpace = new PresetStateSet("Right Disc Space");
            discPreset = new DiscPresetState("Normal", "Normal", "NormalLeftDisc");
            discPreset.addPosition("RightTMJDisc", new Vector3(0.0f, -0.302f, 0.0f));
            rightDiscSpace.addPresetState(discPreset);

            //Left Fossa
            PresetStateSet leftFossa = new PresetStateSet("Left Fossa");
            fossaPreset = new FossaPresetState("Normal", "Normal", "Normal");
            fossaPreset.addPosition("LeftFossa", 0.0f);
            leftFossa.addPresetState(fossaPreset);

            //Right Fossa
            PresetStateSet rightFossa = new PresetStateSet("Right Fossa");
            fossaPreset = new FossaPresetState("Normal", "Normal", "Normal");
            fossaPreset.addPosition("RightFossa", 0.0f);
            rightFossa.addPresetState(fossaPreset);

            statePicker.addPresetStateSet(leftCondyleGrowth);
            statePicker.addPresetStateSet(rightCondyleGrowth);
            statePicker.addPresetStateSet(leftCondyleDegeneration);
            statePicker.addPresetStateSet(rightCondyleDegeneration);
            statePicker.addPresetStateSet(leftDiscSpace);
            statePicker.addPresetStateSet(rightDiscSpace);
            statePicker.addPresetStateSet(leftFossa);
            statePicker.addPresetStateSet(rightFossa);
            statePicker.addStatePanel(new TeethStatePanel());
        }
    }
}
/*
boneManipulatorPreset = new BoneManipulatorPresetState("Condylar Compensation", "Mild Deficiency", "GrowthMildDefiencyCondylarCompensation");
            boneManipulatorPreset.addPosition("leftRamusHeightMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftCondyleHeightMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftCondyleRotationMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftMandibularNotchMandible", 0.0f);
            boneManipulatorPreset.addPosition("leftAntegonialNotchMandible", 0.0f);
*/