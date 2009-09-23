using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Engine;
using System.Threading;
using System.IO;
using Engine.Resources;
using System.Xml;
using Engine.Saving.XMLSaver;
using Medical.Muscles;
using Medical.Properties;

namespace Medical.Controller
{
    /// <summary>
    /// This is the main controller for the Advanced program.
    /// </summary>
    public class AdvancedController : IDisposable
    {
        private MedicalController medicalController;
        private DrawingWindowController drawingWindowController;
        private AdvancedForm advancedForm;
        private GUIElementController guiElements;
        private MedicalStateController stateController = new MedicalStateController();
        private MedicalStateGUI stateGUI;
        private XmlSaver saver = new XmlSaver();
        private ImageRenderer imageRenderer;
        private MovementStateControl movementState;
        private ScenePicker scenePicker = new ScenePicker();

        /// <summary>
        /// Constructor.
        /// </summary>
        public AdvancedController()
        {
            MedicalConfig config = new MedicalConfig(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Anomalous Software/Advanced");
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
            if (medicalController != null)
            {
                medicalController.Dispose();
            }
            if (advancedForm != null)
            {
                advancedForm.Dispose();
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

            advancedForm = new AdvancedForm();
            advancedForm.initialize(this);
            medicalController = new MedicalController();
            medicalController.intialize(advancedForm);

            splashScreen.stepProgress(10);

            drawingWindowController = new DrawingWindowController(MedicalConfig.CamerasFile);
            drawingWindowController.initialize(advancedForm.DockPanel, medicalController.EventManager, PluginManager.Instance.RendererPlugin, MedicalConfig.ConfigFile);

            imageRenderer = new ImageRenderer(medicalController, drawingWindowController);

            guiElements = new GUIElementController(advancedForm.DockPanel, advancedForm.ToolStrip, medicalController);
            guiElements.EnableToolbars = true;

            //Add common gui elements
            LayersControl layersControl = new LayersControl();
            guiElements.addGUIElement(layersControl);

            PictureControl pictureControl = new PictureControl();
            pictureControl.initialize(imageRenderer, drawingWindowController);
            guiElements.addGUIElement(pictureControl);

            stateGUI = new MedicalStateGUI();
            stateGUI.initialize(stateController);
            stateGUI.CreateStateCallback = createStateCallback;
            guiElements.addGUIElement(stateGUI);

            SavedCameraGUI savedCameraGUI = new SavedCameraGUI();
            savedCameraGUI.initialize(drawingWindowController);
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

            splashScreen.stepProgress(70);

            loadDefaultScene();

            if (!guiElements.restoreWindowFile(MedicalConfig.WindowsFile, getDockContent))
            {
                drawingWindowController.createOneWaySplit();
            }

            createNewSequence();

            splashScreen.stepProgress(20);

            advancedForm.Show();
            advancedForm.Activate();
            splashScreen.fadeAway();
            medicalController.start();
        }

        /// <summary>
        /// Stop the loop and begin shutdown procedures.
        /// </summary>
        public void stop()
        {
            medicalController.shutdown();
            guiElements.saveWindowFile(MedicalConfig.WindowsFile);
            drawingWindowController.saveCameraFile();
            drawingWindowController.destroyCameras();
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

        /// <summary>
        /// Change the scene to the specified filename.
        /// </summary>
        /// <param name="filename"></param>
        private bool changeScene(String file)
        {
            guiElements.alertGUISceneUnloading();
            drawingWindowController.destroyCameras();
            if (medicalController.openScene(file))
            {
                drawingWindowController.createCameras(medicalController.MainTimer, medicalController.CurrentScene, medicalController.CurrentSceneDirectory);
                guiElements.alertGUISceneLoaded(medicalController.CurrentScene);
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
