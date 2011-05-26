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
        //UI
        private BorderLayoutContainer screenLayout;
        private CrossFadeLayoutContainer crossFadeContainer;
        private TimelineGUIButtons timelineGUIButtons;

        //State
        private TimelineWizardPanel currentPanel;
        private TimelineWizardPanel lastPanel;
        private GUIManager guiManager;
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
            this.guiManager = standaloneController.GUIManager;
            this.standaloneController = standaloneController;
            this.stateBlender = standaloneController.TemporaryStateBlender;

            screenLayout = new BorderLayoutContainer();
            timelineGUIButtons = new TimelineGUIButtons(this);
            screenLayout.Top = timelineGUIButtons.LayoutContainer;
            crossFadeContainer = new CrossFadeLayoutContainer(standaloneController.MedicalController.MainTimer);
            screenLayout.Center = crossFadeContainer;

            timelineGUIButtons.setPreviousButtonActive(false);
        }

        public void Dispose()
        {
            timelineGUIButtons.Dispose();
        }

        /// <summary>
        /// Called by TimelineWizardPanels when their action instructs them to
        /// show themselves. Puts the panel under management of this
        /// TimelineWizard.
        /// </summary>
        /// <param name="panel">The panel to show.</param>
        public void show(TimelineWizardPanel panel)
        {
            //Swap panels
            lastPanel = currentPanel;
            currentPanel = panel;

            //Set panel scene properties
            MedicalController medicalController = standaloneController.MedicalController;
            SimSubScene defaultScene = medicalController.CurrentScene.getDefaultSubScene();
            SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();
            currentPanel.setSceneProperties(medicalController, medicalScene);

            //Show panel
            if (!wizardInterfaceShown) //If this is false no interfaces have been shown yet for this wizard.
            {
                guiManager.changeLeftPanel(screenLayout);
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
            }
            crossFadeContainer.changePanel(panel.Container, 0.25f, animationCompleted);
            timelineGUIButtons.setNextButtonActive(panel.ShowGUIAction.HasNextTimeline);
        }

        /// <summary>
        /// This will completely shut down the wizard interface. This can be
        /// called by anyone, but it is here to provide the hide method
        /// functionality for the TimelineWizardPanel hide method.
        /// </summary>
        public void hide()
        {
            lastPanel = currentPanel;
            currentPanel = null;
            crossFadeContainer.changePanel(null, 0.25f, animationCompleted);
            guiManager.changeLeftPanel(null);
            wizardInterfaceShown = false;
        }

        /// <summary>
        /// Finish the wizard.
        /// </summary>
        public void finish()
        {
            if (currentPanel != null)
            {
                currentPanel.ShowGUIAction.stopTimelines();
                hide();
                restoreCameraAndLayers();

                //Create state
                stateBlender.forceFinishBlend();
                MedicalState createdState = stateBlender.createBaselineState();
                standaloneController.MedicalStateController.addState(createdState);
            }
        }

        /// <summary>
        /// Play the next timeline as specified.
        /// </summary>
        public void next()
        {
            if (currentPanel != null)
            {
                currentPanel.ShowGUIAction.showNextTimeline();
            }
        }

        /// <summary>
        /// Play the previous timeline.
        /// </summary>
        public void previous()
        {
            //Does nothing right now
        }

        /// <summary>
        /// Cancel the wizard.
        /// </summary>
        public void cancel()
        {
            if (currentPanel != null)
            {
                currentPanel.ShowGUIAction.stopTimelines();
                hide();
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

        /// <summary>
        /// Callback that destroys old panels when they are not being animated anymore.
        /// </summary>
        /// <param name="oldChild"></param>
        private void animationCompleted(LayoutContainer oldChild)
        {
            if (lastPanel != null)
            {
                lastPanel.Dispose();
                lastPanel = null;
            }
        }

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
