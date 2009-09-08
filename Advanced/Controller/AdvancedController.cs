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
        }

        /// <summary>
        /// Start running the program.
        /// </summary>
        public void go()
        {
            SplashScreen splash = new SplashScreen();
            splash.Show();

            advancedForm = new AdvancedForm();
            advancedForm.initialize(this);
            medicalController = new MedicalController();
            medicalController.intialize(advancedForm);

            drawingWindowController = new DrawingWindowController(MedicalConfig.CamerasFile);
            drawingWindowController.initialize(advancedForm.DockPanel, medicalController.EventManager, PluginManager.Instance.RendererPlugin, MedicalConfig.ConfigFile);

            imageRenderer = new ImageRenderer(medicalController, drawingWindowController);

            guiElements = new GUIElementController(advancedForm.DockPanel, advancedForm.ToolStrip, medicalController);

            //Add common gui elements
            LayersControl layersControl = new LayersControl();
            guiElements.addGUIElement(layersControl);

            PictureControl pictureControl = new PictureControl();
            pictureControl.initialize(imageRenderer, drawingWindowController);
            guiElements.addGUIElement(pictureControl);

            stateGUI = new MedicalStateGUI();
            stateGUI.initialize(stateController);
            guiElements.addGUIElement(stateGUI);

            SavedCameraGUI savedCameraGUI = new SavedCameraGUI();
            savedCameraGUI.initialize(drawingWindowController);
            guiElements.addGUIElement(savedCameraGUI);

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

            BonePresetSaveWindow bonePresetSaver = new BonePresetSaveWindow();
            bonePresetSaver.initialize(imageRenderer, stateController);
            guiElements.addGUIElement(bonePresetSaver);

            movementState = new MovementStateControl();
            guiElements.addGUIElement(movementState);

            newScene();
            
            if(!advancedForm.restoreWindows(MedicalConfig.WindowsFile, getDockContent))
            {
                drawingWindowController.createOneWaySplit();
            }

            createNewSequence();

            advancedForm.Show();
            splash.Close();
            medicalController.start();
        }

        /// <summary>
        /// Stop the loop and begin shutdown procedures.
        /// </summary>
        public void stop()
        {
            medicalController.shutdown();
            advancedForm.saveWindows(MedicalConfig.WindowsFile);
            drawingWindowController.saveCameraFile();
            drawingWindowController.destroyCameras();
        }

        /// <summary>
        /// Open the specified file and change the scene.
        /// </summary>
        /// <param name="filename"></param>
        public void open(String filename)
        {
            using (Archive sceneArchive = FileSystem.OpenArchive(Path.GetDirectoryName(filename)))
            {
                if (sceneArchive.exists(filename))
                {
                    using (Stream stream = sceneArchive.openStream(filename, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                    {
                        changeScene(stream);
                    }
                }
                stateController.clearStates();
            }
        }

        public void newScene()
        {
            loadDefaultScene();
            stateController.clearStates();
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
            loadDefaultScene();
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
        private bool changeScene(Stream file)
        {
            guiElements.alertGUISceneUnloading();
            drawingWindowController.destroyCameras();
            if (medicalController.openScene(file))
            {
                drawingWindowController.createCameras(medicalController.MainTimer, medicalController.CurrentScene);
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
            String sceneFileName = "/Scenes/MasterScene.sim.xml";
            using (Archive sceneArchive = FileSystem.OpenArchive(Resource.ResourceRoot))
            {
                if (sceneArchive.exists(Resource.ResourceRoot + sceneFileName))
                {
                    using (Stream stream = sceneArchive.openStream(Resource.ResourceRoot + sceneFileName, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                    {
                        changeScene(stream);
                    }
                }
                stateController.clearStates();
            }
        }
    }
}
