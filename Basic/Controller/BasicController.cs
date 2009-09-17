﻿using System;
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
        private GUIElementController guiElements;
        private MedicalStateController stateController = new MedicalStateController();
        private MedicalStateGUI stateGUI;
        private XmlSaver saver = new XmlSaver();
        private Options options = null;
        private StatePicker statePicker = new StatePicker();
        private ImageRenderer imageRenderer;
        private NavigationController navigationController;

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
            SplashScreen splash = new SplashScreen();
            splash.Show();

            basicForm = new BasicForm();
            basicForm.initialize(this);
            medicalController = new MedicalController();
            medicalController.intialize(basicForm);

            drawingWindowController = new DrawingWindowController(MedicalConfig.CamerasFile);
            drawingWindowController.AllowRotation = false;
            drawingWindowController.initialize(basicForm.DockPanel, medicalController.EventManager, PluginManager.Instance.RendererPlugin, MedicalConfig.ConfigFile);

            navigationController = new NavigationController(drawingWindowController, medicalController.EventManager, medicalController.MainTimer);

            imageRenderer = new ImageRenderer(medicalController, drawingWindowController);

            guiElements = new GUIElementController(basicForm.DockPanel, basicForm.ToolStrip, medicalController);

            //Add common gui elements
            //LayersControl layersControl = new LayersControl();
            //guiElements.addGUIElement(layersControl);

            //PictureControl pictureControl = new PictureControl();
            //pictureControl.initialize(imageRenderer, drawingWindowController);
            //guiElements.addGUIElement(pictureControl);

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

            Watermark.CreateResources();
            drawingWindowController.showWatermarks(true);

            constructStatePicker();

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
            guiElements.alertGUISceneUnloading();
            drawingWindowController.destroyCameras();
            if (medicalController.openScene(file))
            {
                drawingWindowController.createCameras(medicalController.MainTimer, medicalController.CurrentScene, medicalController.CurrentSceneDirectory);
                guiElements.alertGUISceneLoaded(medicalController.CurrentScene);
                TEMP_createNavigationState(drawingWindowController.SceneCameras);
                return true;
            }
            else
            {
                return false;
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
                    stateController.createAndAddState("Normal");
                }
                stateController.addState(statePicker.CreatedState);
                stateGUI.playAll();
            }
        }

        private void constructStatePicker()
        {
            //statePicker.addPresetStateSet(createGrowthSet("Left", "left"));
            //statePicker.addPresetStateSet(createGrowthSet("Right", "right"));
            //statePicker.addPresetStateSet(createDegenerationSet("Left", "left"));
            //statePicker.addPresetStateSet(createDegenerationSet("Right", "right"));

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

        private PresetStateSet createGrowthSet(String sidePretty, String sideBoneBase)
        {
            String directory = Resource.ResourceRoot + "/Presets/" + sidePretty + "Growth";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            BoneManipulatorPresetState boneManipulatorPreset;
            String ramusHeight = sideBoneBase + "RamusHeightMandible";
            String condyleHeight = sideBoneBase + "CondyleHeightMandible";
            String condyleRotation = sideBoneBase + "CondyleRotationMandible";
            String mandibluarNotch = sideBoneBase + "MandibularNotchMandible";
            String antegonialNotch = sideBoneBase + "AntegonialNotchMandible";
            PresetStateSet condyleGrowth = new PresetStateSet(sidePretty + " Condyle Growth", directory);

            boneManipulatorPreset = new BoneManipulatorPresetState("Normal", "Normal", sidePretty + "GrowthNormal.png");
            boneManipulatorPreset.addPosition(new BoneScalarStateEntry(ramusHeight, Vector3.ScaleIdentity));
            boneManipulatorPreset.addPosition(new BoneScalarStateEntry(condyleHeight, Vector3.ScaleIdentity));
            if (sidePretty == "Left")
            {
                boneManipulatorPreset.addPosition(new BoneRotatorStateEntry(condyleRotation, new Quaternion(0.1084457f, -0.0007921561f, -0.06228064f, 0.9921492f)));
            }
            else
            {
                boneManipulatorPreset.addPosition(new BoneRotatorStateEntry(condyleRotation, new Quaternion(-0.1085197f, -0.001012815f, 0.06143159f, 0.9921939f)));
            }
            boneManipulatorPreset.addPosition(new BoneScalarStateEntry(mandibluarNotch, Vector3.ScaleIdentity));
            boneManipulatorPreset.addPosition(new BoneScalarStateEntry(antegonialNotch, Vector3.ScaleIdentity));
            condyleGrowth.addPresetState(boneManipulatorPreset);
            XmlTextWriter writer = new XmlTextWriter(directory + "/" + boneManipulatorPreset.Name + ".pre", Encoding.Default);
            writer.Formatting = Formatting.Indented;
            saver.saveObject(boneManipulatorPreset, writer);
            writer.Close();

            //boneManipulatorPreset = new BoneManipulatorPresetState("No Condylar Compensation 1", "Mild Deficiency", sidePretty + "GrowthMildNoCompensation1.png");
            //boneManipulatorPreset.addPosition(ramusHeight, 0.1960605f);
            //boneManipulatorPreset.addPosition(condyleHeight, 0.2f);
            //boneManipulatorPreset.addPosition(condyleRotation, 0.0f);
            //boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            //boneManipulatorPreset.addPosition(antegonialNotch, 0.0f);
            //condyleGrowth.addPresetState(boneManipulatorPreset);
            //writer = new XmlTextWriter(Resource.ResourceRoot + "/Presets/" + sidePretty + "Growth/" + boneManipulatorPreset.Name + ".pre", Encoding.Default);
            //writer.Formatting = Formatting.Indented;
            //saver.saveObject(boneManipulatorPreset, writer);
            //writer.Close();

            //boneManipulatorPreset = new BoneManipulatorPresetState("No Condylar Compensation 2", "Mild Deficiency", sidePretty + "GrowthMildNoCompensation2.png");
            //boneManipulatorPreset.addPosition(ramusHeight, 0.1960605f);
            //boneManipulatorPreset.addPosition(condyleHeight, 0.3f);
            //boneManipulatorPreset.addPosition(condyleRotation, 0.0f);
            //boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            //boneManipulatorPreset.addPosition(antegonialNotch, 0.0f);
            //condyleGrowth.addPresetState(boneManipulatorPreset);
            //writer = new XmlTextWriter(Resource.ResourceRoot + "/Presets/" + sidePretty + "Growth/" + boneManipulatorPreset.Name + ".pre", Encoding.Default);
            //writer.Formatting = Formatting.Indented;
            //saver.saveObject(boneManipulatorPreset, writer);
            //writer.Close();

            //boneManipulatorPreset = new BoneManipulatorPresetState("No Condylar Compensation 3", "Mild Deficiency", sidePretty + "GrowthMildNoCompensation3.png");
            //boneManipulatorPreset.addPosition(ramusHeight, 0.1960605f);
            //boneManipulatorPreset.addPosition(condyleHeight, 0.4f);
            //boneManipulatorPreset.addPosition(condyleRotation, 0.0f);
            //boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            //boneManipulatorPreset.addPosition(antegonialNotch, 0.0f);
            //condyleGrowth.addPresetState(boneManipulatorPreset);
            //writer = new XmlTextWriter(Resource.ResourceRoot + "/Presets/" + sidePretty + "Growth/" + boneManipulatorPreset.Name + ".pre", Encoding.Default);
            //writer.Formatting = Formatting.Indented;
            //saver.saveObject(boneManipulatorPreset, writer);
            //writer.Close();

            //boneManipulatorPreset = new BoneManipulatorPresetState("Condylar Compensation", "Mild Deficiency", sidePretty + "GrowthMildCompensation.png");
            //boneManipulatorPreset.addPosition(ramusHeight, 0.1960605f);
            //boneManipulatorPreset.addPosition(condyleHeight, 0.4f);
            //boneManipulatorPreset.addPosition(condyleRotation, 0.4f);
            //boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            //boneManipulatorPreset.addPosition(antegonialNotch, 0.0f);
            //condyleGrowth.addPresetState(boneManipulatorPreset);
            //writer = new XmlTextWriter(Resource.ResourceRoot + "/Presets/" + sidePretty + "Growth/" + boneManipulatorPreset.Name + ".pre", Encoding.Default);
            //writer.Formatting = Formatting.Indented;
            //saver.saveObject(boneManipulatorPreset, writer);
            //writer.Close();

            //boneManipulatorPreset = new BoneManipulatorPresetState("Condylar Compensation 1", "Moderate Deficiency", sidePretty + "GrowthModerateCompensation1.png");
            //boneManipulatorPreset.addPosition(ramusHeight, 0.1960605f);
            //boneManipulatorPreset.addPosition(condyleHeight, 0.7f);
            //boneManipulatorPreset.addPosition(condyleRotation, 0.2f);
            //boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            //boneManipulatorPreset.addPosition(antegonialNotch, 0.7f);
            //condyleGrowth.addPresetState(boneManipulatorPreset);
            //writer = new XmlTextWriter(Resource.ResourceRoot + "/Presets/" + sidePretty + "Growth/" + boneManipulatorPreset.Name + ".pre", Encoding.Default);
            //writer.Formatting = Formatting.Indented;
            //saver.saveObject(boneManipulatorPreset, writer);
            //writer.Close();

            //boneManipulatorPreset = new BoneManipulatorPresetState("Condylar Compensation 2", "Moderate Deficiency", sidePretty + "GrowthModerateCompensation2.png");
            //boneManipulatorPreset.addPosition(ramusHeight, 0.1960605f);
            //boneManipulatorPreset.addPosition(condyleHeight, 0.7f);
            //boneManipulatorPreset.addPosition(condyleRotation, 0.7f);
            //boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            //boneManipulatorPreset.addPosition(antegonialNotch, 0.8f);
            //condyleGrowth.addPresetState(boneManipulatorPreset);
            //writer = new XmlTextWriter(Resource.ResourceRoot + "/Presets/" + sidePretty + "Growth/" + boneManipulatorPreset.Name + ".pre", Encoding.Default);
            //writer.Formatting = Formatting.Indented;
            //saver.saveObject(boneManipulatorPreset, writer);
            //writer.Close();

            //boneManipulatorPreset = new BoneManipulatorPresetState("Condylar Compensation 3", "Moderate Deficiency", sidePretty + "GrowthModerateCompensation3.png");
            //boneManipulatorPreset.addPosition(ramusHeight, 0.1960605f);
            //boneManipulatorPreset.addPosition(condyleHeight, 0.7f);
            //boneManipulatorPreset.addPosition(condyleRotation, 1.0f);
            //boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            //boneManipulatorPreset.addPosition(antegonialNotch, 1.0f);
            //condyleGrowth.addPresetState(boneManipulatorPreset);
            //writer = new XmlTextWriter(Resource.ResourceRoot + "/Presets/" + sidePretty + "Growth/" + boneManipulatorPreset.Name + ".pre", Encoding.Default);
            //writer.Formatting = Formatting.Indented;
            //saver.saveObject(boneManipulatorPreset, writer);
            //writer.Close();

            //boneManipulatorPreset = new BoneManipulatorPresetState("No Condylar Compensation", "Moderate Deficiency", sidePretty + "GrowthModerateNoCompensation.png");
            //boneManipulatorPreset.addPosition(ramusHeight, 0.1960605f);
            //boneManipulatorPreset.addPosition(condyleHeight, 0.7f);
            //boneManipulatorPreset.addPosition(condyleRotation, 0.0f);
            //boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            //boneManipulatorPreset.addPosition(antegonialNotch, 0.0f);
            //condyleGrowth.addPresetState(boneManipulatorPreset);
            //writer = new XmlTextWriter(Resource.ResourceRoot + "/Presets/" + sidePretty + "Growth/" + boneManipulatorPreset.Name + ".pre", Encoding.Default);
            //writer.Formatting = Formatting.Indented;
            //saver.saveObject(boneManipulatorPreset, writer);
            //writer.Close();

            //boneManipulatorPreset = new BoneManipulatorPresetState("Total Mandible 1", "Extreme Deficiency", sidePretty + "GrowthExtremeTotalMandible1.png");
            //boneManipulatorPreset.addPosition(ramusHeight, 0.09f);
            //boneManipulatorPreset.addPosition(condyleHeight, 0.95f);
            //boneManipulatorPreset.addPosition(condyleRotation, 0.9f);
            //boneManipulatorPreset.addPosition(mandibluarNotch, 0.0f);
            //boneManipulatorPreset.addPosition(antegonialNotch, 0.6f);
            //condyleGrowth.addPresetState(boneManipulatorPreset);
            //writer = new XmlTextWriter(Resource.ResourceRoot + "/Presets/" + sidePretty + "Growth/" + boneManipulatorPreset.Name + ".pre", Encoding.Default);
            //writer.Formatting = Formatting.Indented;
            //saver.saveObject(boneManipulatorPreset, writer);
            //writer.Close();

            //boneManipulatorPreset = new BoneManipulatorPresetState("Total Mandible 2", "Extreme Deficiency", sidePretty + "GrowthExtremeTotalMandible2.png");
            //boneManipulatorPreset.addPosition(ramusHeight, 0.1f);
            //boneManipulatorPreset.addPosition(condyleHeight, 0.98f);
            //boneManipulatorPreset.addPosition(condyleRotation, 1.0f);
            //boneManipulatorPreset.addPosition(mandibluarNotch, 0.1f);
            //boneManipulatorPreset.addPosition(antegonialNotch, 0.85f);
            //condyleGrowth.addPresetState(boneManipulatorPreset);
            //writer = new XmlTextWriter(Resource.ResourceRoot + "/Presets/" + sidePretty + "Growth/" + boneManipulatorPreset.Name + ".pre", Encoding.Default);
            //writer.Formatting = Formatting.Indented;
            //saver.saveObject(boneManipulatorPreset, writer);
            //writer.Close();

            //boneManipulatorPreset = new BoneManipulatorPresetState("Total Mandible 3", "Extreme Deficiency", sidePretty + "GrowthExtremeTotalMandible3.png");
            //boneManipulatorPreset.addPosition(ramusHeight, 0.62f);
            //boneManipulatorPreset.addPosition(condyleHeight, 1.0f);
            //boneManipulatorPreset.addPosition(condyleRotation, 1.0f);
            //boneManipulatorPreset.addPosition(mandibluarNotch, 0.25f);
            //boneManipulatorPreset.addPosition(antegonialNotch, 1.0f);
            //condyleGrowth.addPresetState(boneManipulatorPreset);
            //writer = new XmlTextWriter(Resource.ResourceRoot + "/Presets/" + sidePretty + "Growth/" + boneManipulatorPreset.Name + ".pre", Encoding.Default);
            //writer.Formatting = Formatting.Indented;
            //saver.saveObject(boneManipulatorPreset, writer);
            //writer.Close();

            return condyleGrowth;
        }

        private PresetStateSet createDegenerationSet(String sidePretty, String sideBoneBase)
        {
            if (!Directory.Exists(Resource.ResourceRoot + "/Presets/" + sidePretty + "Degeneration"))
            {
                Directory.CreateDirectory(Resource.ResourceRoot + "/Presets/" + sidePretty + "Degeneration");
            }

            String condyleDegenerationMandible = sideBoneBase + "CondyleDegenerationMandible";
            String lateralPoleMandible = sideBoneBase + "LateralPoleMandible";
            String medialPoleScale = sideBoneBase + "MedialPoleScaleMandible";
            BoneManipulatorPresetState boneManipulatorPreset;
            PresetStateSet condyleDegeneration = new PresetStateSet(sidePretty + " Condyle Degeneration", Resource.ResourceRoot + "/Presets/" + sidePretty + "Degeneration");
            XmlTextWriter writer;

            boneManipulatorPreset = new BoneManipulatorPresetState("Normal", "Normal", sidePretty + "DegenerationNormal.png");
            boneManipulatorPreset.addPosition(new BoneScalarStateEntry(condyleDegenerationMandible, Vector3.ScaleIdentity));
            boneManipulatorPreset.addPosition(new BoneScalarStateEntry(lateralPoleMandible, Vector3.ScaleIdentity));
            boneManipulatorPreset.addPosition(new BoneScalarStateEntry(medialPoleScale, Vector3.ScaleIdentity));
            condyleDegeneration.addPresetState(boneManipulatorPreset);
            writer = new XmlTextWriter(Resource.ResourceRoot + "/Presets/" + sidePretty + "Degeneration/" + boneManipulatorPreset.Name + ".pre", Encoding.Default);
            writer.Formatting = Formatting.Indented;
            saver.saveObject(boneManipulatorPreset, writer);
            writer.Close();

            //boneManipulatorPreset = new BoneManipulatorPresetState("Mild Degeneration", "Total Degeneration", sidePretty + "DegenerationMild.png");
            //boneManipulatorPreset.addPosition(condyleDegenerationMandible, 0.2f);
            //boneManipulatorPreset.addPosition(lateralPoleMandible, 0.2f);
            //boneManipulatorPreset.addPosition(medialPoleScale, 0.2f);
            //condyleDegeneration.addPresetState(boneManipulatorPreset);
            //writer = new XmlTextWriter(Resource.ResourceRoot + "/Presets/" + sidePretty + "Degeneration/" + boneManipulatorPreset.Name + ".pre", Encoding.Default);
            //writer.Formatting = Formatting.Indented;
            //saver.saveObject(boneManipulatorPreset, writer);
            //writer.Close();

            //boneManipulatorPreset = new BoneManipulatorPresetState("Moderate Degeneration", "Total Degeneration", sidePretty + "DegenerationModerate.png");
            //boneManipulatorPreset.addPosition(condyleDegenerationMandible, 0.4f);
            //boneManipulatorPreset.addPosition(lateralPoleMandible, 0.4f);
            //boneManipulatorPreset.addPosition(medialPoleScale, 0.4f);
            //condyleDegeneration.addPresetState(boneManipulatorPreset);
            //writer = new XmlTextWriter(Resource.ResourceRoot + "/Presets/" + sidePretty + "Degeneration/" + boneManipulatorPreset.Name + ".pre", Encoding.Default);
            //writer.Formatting = Formatting.Indented;
            //saver.saveObject(boneManipulatorPreset, writer);
            //writer.Close();

            //boneManipulatorPreset = new BoneManipulatorPresetState("Severe Degeneration", "Total Degeneration", sidePretty + "DegenerationSevere.png");
            //boneManipulatorPreset.addPosition(condyleDegenerationMandible, 0.7f);
            //boneManipulatorPreset.addPosition(lateralPoleMandible, 0.7f);
            //boneManipulatorPreset.addPosition(medialPoleScale, 0.7f);
            //condyleDegeneration.addPresetState(boneManipulatorPreset);
            //writer = new XmlTextWriter(Resource.ResourceRoot + "/Presets/" + sidePretty + "Degeneration/" + boneManipulatorPreset.Name + ".pre", Encoding.Default);
            //writer.Formatting = Formatting.Indented;
            //saver.saveObject(boneManipulatorPreset, writer);
            //writer.Close();

            //boneManipulatorPreset = new BoneManipulatorPresetState("Extreme Degeneration", "Total Degeneration", sidePretty + "DegenerationExtreme.png");
            //boneManipulatorPreset.addPosition(condyleDegenerationMandible, 1.0f);
            //boneManipulatorPreset.addPosition(lateralPoleMandible, 1.0f);
            //boneManipulatorPreset.addPosition(medialPoleScale, 1.0f);
            //condyleDegeneration.addPresetState(boneManipulatorPreset);
            //writer = new XmlTextWriter(Resource.ResourceRoot + "/Presets/" + sidePretty + "Degeneration/" + boneManipulatorPreset.Name + ".pre", Encoding.Default);
            //writer.Formatting = Formatting.Indented;
            //saver.saveObject(boneManipulatorPreset, writer);
            //writer.Close();

            return condyleDegeneration;
        }

        private void TEMP_createNavigationState(SavedCameraController sceneCameras)
        {
            navigationController.clearStates();
            foreach (SavedCameraDefinition def in sceneCameras.getSavedCameras())
            {
                NavigationState state = new NavigationState(def.Name, def.LookAt, def.Position);
                navigationController.addState(state);
            }
            //setup adjacent states
            //outer shell
            NavigationState target = navigationController.getState("Midline anterior");
            target.addTwoWayAdjacentState(navigationController.getState("Midline anterosuperior"));
            target.addTwoWayAdjacentState(navigationController.getState("Midline anteroinferior"));
            target.addTwoWayAdjacentState(navigationController.getState("Right lateral"));
            target.addTwoWayAdjacentState(navigationController.getState("Left lateral"));
            target.addTwoWayAdjacentState(navigationController.getState("Occlusion joint anterior"));

            target = navigationController.getState("Left lateral");
            target.addTwoWayAdjacentState(navigationController.getState("Left laterosuperior"));
            target.addTwoWayAdjacentState(navigationController.getState("Left lateroinferior"));
            target.addTwoWayAdjacentState(navigationController.getState("Occlusion joint left"));
            target.addTwoWayAdjacentState(navigationController.getState("Midline posterior"));
            target.addTwoWayAdjacentState(navigationController.getState("Midline anterior"));

            target = navigationController.getState("Right lateral");
            target.addTwoWayAdjacentState(navigationController.getState("Right laterosuperior"));
            target.addTwoWayAdjacentState(navigationController.getState("RightLateroinferior"));
            target.addTwoWayAdjacentState(navigationController.getState("Occlusion joint right"));
            target.addTwoWayAdjacentState(navigationController.getState("Midline posterior"));
            target.addTwoWayAdjacentState(navigationController.getState("Midline anterior"));

            target = navigationController.getState("Midline anterosuperior");
            target.addTwoWayAdjacentState(navigationController.getState("Midline superior"));
            target.addTwoWayAdjacentState(navigationController.getState("Right laterosuperior"));
            target.addTwoWayAdjacentState(navigationController.getState("Left laterosuperior"));

            target = navigationController.getState("Midline superior");
            target.addTwoWayAdjacentState(navigationController.getState("Midline posterosuperior"));

            target = navigationController.getState("Midline posterosuperior");
            target.addTwoWayAdjacentState(navigationController.getState("Right laterosuperior"));
            target.addTwoWayAdjacentState(navigationController.getState("Left laterosuperior"));
            target.addTwoWayAdjacentState(navigationController.getState("Midline posterior"));

            target = navigationController.getState("Midline posterior");
            target.addTwoWayAdjacentState(navigationController.getState("Midline posteroinferior"));

            target = navigationController.getState("Midline anteroinferior");
            target.addTwoWayAdjacentState(navigationController.getState("Left lateroinferior"));
            target.addTwoWayAdjacentState(navigationController.getState("RightLateroinferior"));
            target.addTwoWayAdjacentState(navigationController.getState("Midline submental"));

            target = navigationController.getState("Midline submental");
            target.addTwoWayAdjacentState(navigationController.getState("Midline posteroinferior"));

            target = navigationController.getState("Midline posteroinferior");
            target.addTwoWayAdjacentState(navigationController.getState("Left lateroinferior"));
            target.addTwoWayAdjacentState(navigationController.getState("RightLateroinferior"));
            target.addTwoWayAdjacentState(navigationController.getState("Occlusion joint posterior"));

            target = navigationController.getState("Midline posterosuperior");
            target.addTwoWayAdjacentState(navigationController.getState("Right laterosuperior"));
            target.addTwoWayAdjacentState(navigationController.getState("Left laterosuperior"));

            //Occlusion
            target = navigationController.getState("Occlusion joint posterior");
            target.addTwoWayAdjacentState(navigationController.getState("Occlusion joint left"));
            target.addTwoWayAdjacentState(navigationController.getState("Occlusion joint right"));
            target.addTwoWayAdjacentState(navigationController.getState("Dentition anterior lingual"));

            target = navigationController.getState("Occlusion joint anterior");
            target.addTwoWayAdjacentState(navigationController.getState("Occlusion joint left"));
            target.addTwoWayAdjacentState(navigationController.getState("Occlusion joint right"));
            target.addTwoWayAdjacentState(navigationController.getState("Dentition anterior labial"));

            target = navigationController.getState("Occlusion joint left");
            target.addTwoWayAdjacentState(navigationController.getState("Dentition left posterior buccal"));

            target = navigationController.getState("Occlusion joint right");
            target.addTwoWayAdjacentState(navigationController.getState("Dentition right posterior buccal"));

            //Dentition
            target = navigationController.getState("Dentition anterior labial");
            target.addTwoWayAdjacentState(navigationController.getState("Dentition left posterior buccal"));
            target.addTwoWayAdjacentState(navigationController.getState("Dentition right posterior buccal"));

            target = navigationController.getState("Dentition left posterior buccal");
            target.addTwoWayAdjacentState(navigationController.getState("Dentition left posterior lingual"));

            target = navigationController.getState("Dentition right posterior buccal");
            target.addTwoWayAdjacentState(navigationController.getState("Dentition right posterior lingual"));

            target = navigationController.getState("Dentition anterior lingual");
            target.addTwoWayAdjacentState(navigationController.getState("Dentition left posterior lingual"));
            target.addTwoWayAdjacentState(navigationController.getState("Dentition right posterior lingual"));
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