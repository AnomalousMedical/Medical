using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Logging;

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
        private LayerController layerController;

        //Wizard state
        private LayerState layerStatusBeforeShown;
        private String navigationStateBeforeShown;
        private int currentIndex;
        private StateWizard currentWizard;

        //UI
        private BasicGUI basicGUI;
        private BorderLayoutContainer screenLayout;
        private StateWizardButtons stateWizardButtons;

        public StateWizardController(TemporaryStateBlender stateBlender, NavigationController navigationController, LayerController layerController, BasicGUI basicGUI)
        {
            this.basicGUI = basicGUI;
            this.stateBlender = stateBlender;
            this.navigationController = navigationController;
            this.layerController = layerController;

            screenLayout = new BorderLayoutContainer();
            stateWizardButtons = new StateWizardButtons(this);
            screenLayout.Top = stateWizardButtons.LayoutContainer;
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
                layerStatusBeforeShown = new LayerState("Temp");
                layerStatusBeforeShown.captureState();
                //currentDrawingWindow = controllingWindow;
                //NavigationState currentState = navigationController.getNavigationState(currentDrawingWindow);
                //if (currentState != null)
                //{
                //    navigationStateBeforeShown = currentState.Name;
                //}
                //else
                {
                    navigationStateBeforeShown = null;
                }
                stateBlender.recordUndoState();
                currentWizard.startWizard();
                basicGUI.changeLeftPanel(screenLayout);
            }
            else
            {
                Log.Error("Could not open wizard {0}. It does not exist.", name);
            }
        }

        /// <summary>
        /// Show a wizard panel, called by the wizards
        /// </summary>
        /// <param name="panel"></param>
        internal void showPanel(StateWizardPanel panel)
        {
            screenLayout.Center = panel.LayoutContainer;
        }

        /// <summary>
        /// Hide a wizard panel, called by the wizards
        /// </summary>
        /// <param name="stateWizardPanel"></param>
        internal void hidePanel(StateWizardPanel stateWizardPanel)
        {
            screenLayout.Center = null;
        }

        /// <summary>
        /// Add a panel to the controller. Called by the wizards
        /// </summary>
        /// <param name="panel"></param>
        internal void addMode(StateWizardPanel panel)
        {
            
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
        }

        /// <summary>
        /// Next button clicked. Called by the StateWizardPanel.
        /// </summary>
        internal void next()
        {
            currentWizard.hidePanel(currentIndex);
            currentIndex++;
            currentWizard.showPanel(currentIndex);
        }

        /// <summary>
        /// Previous button clicked. Called by the StateWizardPanel.
        /// </summary>
        internal void previous()
        {
            currentWizard.hidePanel(currentIndex);
            currentIndex--;
            currentWizard.showPanel(currentIndex);
        }

        /// <summary>
        /// Cancel button clicked. Called by the StateWizardPanel.
        /// </summary>
        internal void cancel()
        {
            stateBlender.blendToUndo();
            currentWizard.resetPanels();
        }

        //public void modeChanged(int modeIndex)
        //{
        //    if (updatePanel)
        //    {
        //        hidePanel();
        //        //currentIndex = uiHost.SelectedIndex;
        //        showPanel();
        //    }
        //}
    }
}
