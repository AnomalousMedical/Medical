using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using Medical.Controller;
using Engine.ObjectManagement;
using Engine.Saving.XMLSaver;

namespace Medical.GUI
{
    /// <summary>
    /// This class allows a collection of TimelineWizardPanels to act as a wizard.
    /// 
    /// The lifcycle for these wizards is as follows:
    /// 1. The TimelineWizardPanel subclass is created by its TimelineGUIFactoryPrototype.
    /// 2. It is shown by the timeline action.
    /// 3. It is brought into this class with the show method.
    /// 4. The timeline is advanced somehow with the TimelineGUIButtons or something else. This comes back to this class to change out the active interfaces.
    /// 5. When the animations are complete for the panels being shown this class will dispose the old TimelineWizardPanel.
    /// 
    /// In short as long as a TimelineWizardPanel is shown by this class it will be disposed by this class.
    /// </summary>
    public class TimelineWizard : IDisposable
    {
        //State
        private bool wizardInterfaceShown = false;
        private StandaloneController standaloneController;
        private XmlSaver xmlSaver = new XmlSaver();

        //Startup options
        Vector3 cameraPosition;
        Vector3 cameraLookAt;
        LayerState layers;
        TemporaryStateBlender stateBlender;

        public TimelineWizard(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            this.stateBlender = standaloneController.TemporaryStateBlender;

            Notes = new NotesGUI(this, standaloneController.ImageRenderer);
            Notes.Visible = false;
        }

        public void Dispose()
        {
            Notes.Dispose();
        }

        /// <summary>
        /// Called by TimelineWizardPanels when their action instructs them to
        /// show themselves. Puts the panel under management of this
        /// TimelineWizard.
        /// </summary>
        /// <param name="panel">The panel to show.</param>
        public void show(TimelineWizardPanel panel)
        {
            //Set panel scene properties
            MedicalController medicalController = standaloneController.MedicalController;
            SimSubScene defaultScene = medicalController.CurrentScene.getDefaultSubScene();
            SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();
            panel.opening(medicalController, medicalScene);

            //Show panel
            if (!wizardInterfaceShown) //If this is false no interfaces have been shown yet for this wizard.
            {
                wizardInterfaceShown = true;
                //Store scene settings
                SceneViewWindow window = standaloneController.SceneViewController.ActiveWindow;
                if (window != null)
                {
                    cameraPosition = window.Translation;
                    cameraLookAt = window.LookAt;
                }
                layers = new LayerState("");
                layers.captureState();
                stateBlender.recordUndoState();
                Notes.setToDefault();
            }
        }

        /// <summary>
        /// Finish the wizard.
        /// </summary>
        public void finish()
        {
            if (wizardInterfaceShown)
            {
                wizardInterfaceShown = false;
                restoreCameraAndLayers();

                //Create state
                stateBlender.forceFinishBlend();
                MedicalState createdState = stateBlender.createBaselineState();
                Notes.applyToState(createdState);
                standaloneController.MedicalStateController.addState(createdState);
            }
        }

        /// <summary>
        /// Cancel the wizard.
        /// </summary>
        public void cancel()
        {
            if (wizardInterfaceShown)
            {
                wizardInterfaceShown = false;
                restoreCameraAndLayers();
                stateBlender.blendToUndo();
            }
        }

        public void applyPresetState(PresetState presetState)
        {
            MedicalState createdState;
            createdState = stateBlender.createBaselineState();
            presetState.applyToState(createdState);
            stateBlender.startTemporaryBlend(createdState);
        }

        public XmlSaver Saver
        {
            get
            {
                return xmlSaver;
            }
        }

        public TemporaryStateBlender StateBlender
        {
            get
            {
                return stateBlender;
            }
        }

        public SceneViewController SceneViewController
        {
            get
            {
                return standaloneController.SceneViewController;
            }
        }

        public NotesGUI Notes { get; private set; }

        private void restoreCameraAndLayers()
        {
            SceneViewWindow window = standaloneController.SceneViewController.ActiveWindow;
            if (window != null)
            {
                window.setPosition(cameraPosition, cameraLookAt);
                layers.apply();
            }
        }
    }
}
