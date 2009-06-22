using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Engine;
using System.Threading;

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

        /// <summary>
        /// Constructor.
        /// </summary>
        public AdvancedController()
        {

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

            guiElements = new GUIElementController(advancedForm.DockPanel, advancedForm.ToolStrip);

            //Add common gui elements
            LayersControl layersControl = new LayersControl();
            guiElements.addGUIElement(layersControl);

            //MedicalStateGUI stateGUI = new MedicalStateGUI();
            //stateGUI.initialize(medicalController);
            //guiElements.addGUIElement(stateGUI);

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
            
            if(!advancedForm.restoreWindows(MedicalConfig.WindowsFile, getDockContent))
            {
                drawingWindowController.createOneWaySplit();
            }

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
            changeScene(filename);
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
                guiElements.alertGUISceneLoaded();
            }
            else
            {
                MessageBox.Show(String.Format("Could not open scene {0}.", filename), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
