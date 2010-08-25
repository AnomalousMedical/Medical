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
        private List<StateWizard> wizards = new List<StateWizard>();

        //Controllers
        private TemporaryStateBlender stateBlender;
        private NavigationController navigationController;
        private LayerController layerController;

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

        public StateWizardController(UpdateTimer mainTimer, TemporaryStateBlender stateBlender, NavigationController navigationController, LayerController layerController, BasicGUI basicGUI)
        {
            this.basicGUI = basicGUI;
            this.stateBlender = stateBlender;
            this.navigationController = navigationController;
            this.layerController = layerController;

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
            wizards.Add(wizard);
        }

        public void startWizard(StateWizard wizard)
        {
            currentWizard = wizard;
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
                layerController.CurrentLayerState = layerStatusBeforeShown;
                if (Finished != null)
                {
                    Finished.Invoke();
                }
            }
        }

        public SceneViewWindow CurrentSceneView { get; set; }

        public IEnumerable<StateWizard> WizardEnum
        {
            get
            {
                return wizards;
            }
        }

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
            currentWizard.applyToState(createdState);
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
