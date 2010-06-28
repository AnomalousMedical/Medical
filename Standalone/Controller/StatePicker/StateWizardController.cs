using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Logging;
using Engine.Platform;

namespace Medical.GUI
{
    public delegate void MedicalStateCreated(MedicalState state);
    public delegate void StatePickerFinished();

    class StateWizardController : IDisposable
    {
        //Events
        public event MedicalStateCreated StateCreated;
        public event StatePickerFinished Finished;

        //Wizard collection
        private Dictionary<String, StateWizard> wizards = new Dictionary<String, StateWizard>();

        //Controllers
        private TemporaryStateBlender stateBlender;
        private NavigationController navigationController;

        //Wizard state
        private LayerState layerStatusBeforeShown = new LayerState("WizardStartLayerStatus");
        private String navigationStateBeforeShown;
        private int currentIndex;
        private int maxIndex = 0;
        private StateWizard currentWizard;

        //UI
        private BasicGUI basicGUI;
        private BorderLayoutContainer screenLayout;
        private CrossFadeLayoutContainer crossFadeContainer;
        private StateWizardButtons stateWizardButtons;
        private WizardIconPanel wizardIconPanel;

        public StateWizardController(UpdateTimer mainTimer, TemporaryStateBlender stateBlender, NavigationController navigationController, BasicGUI basicGUI)
        {
            this.basicGUI = basicGUI;
            this.stateBlender = stateBlender;
            this.navigationController = navigationController;

            screenLayout = new BorderLayoutContainer();
            stateWizardButtons = new StateWizardButtons(this);
            screenLayout.Top = stateWizardButtons.LayoutContainer;
            crossFadeContainer = new CrossFadeLayoutContainer(mainTimer);
            screenLayout.Center = crossFadeContainer;
            wizardIconPanel = new WizardIconPanel();
            wizardIconPanel.ModeChanged += new WizardIconPanel.ModeChangedDelegate(wizardIconPanel_ModeChanged);
        }

        public void Dispose()
        {
            stateWizardButtons.Dispose();
        }

        public void addWizard(StateWizard wizard)
        {
            wizards.Add(wizard.Name, wizard);
        }

        public void startWizard(String name)
        {
            wizards.TryGetValue(name, out currentWizard);
            if (currentWizard != null)
            {
                layerStatusBeforeShown.captureState();
                NavigationState currentState = navigationController.getNavigationState(CurrentSceneView);
                if (currentState != null)
                {
                    navigationStateBeforeShown = currentState.Name;
                }
                else
                {
                    navigationStateBeforeShown = null;
                }
                stateBlender.recordUndoState();
                maxIndex = 0;
                currentIndex = 0;
                wizardIconPanel.SuppressLayout = true;
                currentWizard.startWizard();
                wizardIconPanel.SuppressLayout = false;
                wizardIconPanel.invalidate();
                currentWizard.showPanel(currentIndex);
                basicGUI.changeLeftPanel(screenLayout);
                basicGUI.changeTopPanel(wizardIconPanel.LayoutContainer);
            }
            else
            {
                Log.Error("Could not open wizard {0}. It does not exist.", name);
            }
        }

        public void closeWizard()
        {
            if (currentWizard != null)
            {
                currentWizard.hidePanel(currentIndex);
                wizardIconPanel.clearPanels();
                crossFadeContainer.changePanel(null, 0.0f, animationCompleted);
                basicGUI.changeLeftPanel(null);
                basicGUI.resetTopPanel();
                currentWizard = null;
                if (navigationStateBeforeShown != null)
                {
                    navigationController.setNavigationState(navigationStateBeforeShown, CurrentSceneView);
                }
                layerStatusBeforeShown.apply();
            }
        }

        public SceneViewWindow CurrentSceneView { get; set; }

        /// <summary>
        /// Show a wizard panel, called by the wizards
        /// </summary>
        /// <param name="panel"></param>
        internal void showPanel(StateWizardPanel panel)
        {
            stateWizardButtons.setPreviousButtonActive(currentIndex != 0);
            stateWizardButtons.setNextButtonActive(currentIndex != maxIndex - 1);
            panel.LayoutContainer.Visible = true;
            panel.LayoutContainer.bringToFront();
            //screenLayout.Center = panel.LayoutContainer;
            crossFadeContainer.changePanel(panel.LayoutContainer, 0.25f, animationCompleted);
            wizardIconPanel.indexChanged(currentIndex);
        }

        /// <summary>
        /// Hide a wizard panel, called by the wizards
        /// </summary>
        /// <param name="stateWizardPanel"></param>
        internal void hidePanel(StateWizardPanel stateWizardPanel)
        {
            stateWizardPanel.LayoutContainer.Visible = false;
            //screenLayout.Center = null;
        }

        /// <summary>
        /// Add a panel to the controller. Called by the wizards
        /// </summary>
        /// <param name="panel"></param>
        internal void addMode(StateWizardPanel panel)
        {
            wizardIconPanel.addPanel(panel, maxIndex);
            maxIndex++;
        }

        /// <summary>
        /// Next button clicked. Called by the StateWizardPanel.
        /// </summary>
        internal void next()
        {
            currentWizard.hidePanel(currentIndex);
            currentIndex++;
            if (currentIndex >= maxIndex)
            {
                currentIndex = maxIndex - 1;
            }
            currentWizard.showPanel(currentIndex);
        }

        /// <summary>
        /// Previous button clicked. Called by the StateWizardPanel.
        /// </summary>
        internal void previous()
        {
            currentWizard.hidePanel(currentIndex);
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = 0;
            }
            currentWizard.showPanel(currentIndex);
        }

        /// <summary>
        /// Cancel button clicked. Called by the StateWizardPanel.
        /// </summary>
        internal void cancel()
        {
            stateBlender.blendToUndo();
            currentWizard.resetPanels();
            closeWizard();
        }

        /// <summary>
        /// Finish button clicked. Called by the StateWizardPanel.
        /// </summary>
        internal void finish()
        {
            stateBlender.forceFinishBlend();
            MedicalState createdState = stateBlender.createBaselineState();
            if (StateCreated != null)
            {
                StateCreated.Invoke(createdState);
            }
            closeWizard();
        }

        private void animationCompleted(LayoutContainer oldChild)
        {
            if (oldChild != null)
            {
                oldChild.Visible = false;
            }
        }

        /// <summary>
        /// Callback from the WizardIconPanel for direct index changes.
        /// </summary>
        /// <param name="modeIndex"></param>
        private void wizardIconPanel_ModeChanged(int modeIndex)
        {
            if (currentIndex != modeIndex)
            {
                currentWizard.hidePanel(currentIndex);
                currentIndex = modeIndex;
                currentWizard.showPanel(currentIndex);
            }
        }
    }
}
