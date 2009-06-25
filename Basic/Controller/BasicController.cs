using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using WeifenLuo.WinFormsUI.Docking;
using Engine;
using System.Windows.Forms;
using Medical.GUI.StateWizard;

namespace Medical.Controller
{
    public class BasicController : IDisposable
    {
        private MedicalController medicalController;
        private DrawingWindowController drawingWindowController;
        private BasicForm basicForm;
        private GUIElementController guiElements;
        private MedicalStateController stateController = new MedicalStateController();
        private StateWizardForm stateWizard = new StateWizardForm();
        private MedicalStateGUI stateGUI;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BasicController()
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
            if (basicForm != null)
            {
                basicForm.Dispose();
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

            guiElements = new GUIElementController(basicForm.DockPanel, basicForm.ToolStrip);

            //Add common gui elements
            LayersControl layersControl = new LayersControl();
            guiElements.addGUIElement(layersControl);

            stateGUI = new MedicalStateGUI();
            stateGUI.initialize(stateController, medicalController);
            guiElements.addGUIElement(stateGUI);

            SavedCameraGUI savedCameraGUI = new SavedCameraGUI();
            savedCameraGUI.initialize(drawingWindowController);
            guiElements.addGUIElement(savedCameraGUI);

            //Add specific gui elements
            MuscleControl muscleControl = new MuscleControl();
            guiElements.addGUIElement(muscleControl);

            if(!basicForm.restoreWindows(MedicalConfig.WindowsFile, getDockContent))
            {
                drawingWindowController.createOneWaySplit();
            }

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

        public void showStateWizard()
        {
            stateWizard.startWizard();
            stateWizard.ShowDialog(basicForm);
            if (stateWizard.WizardFinished)
            {
                stateController.addState(stateWizard.CreatedState);
                stateGUI.playAll();
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
                guiElements.alertGUISceneLoaded();
            }
            else
            {
                MessageBox.Show(String.Format("Could not open scene {0}.", filename), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
