using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Medical.GUI
{
    public delegate void MedicalStateCreated(MedicalState state);
    public delegate void StatePickerFinished();

    public class StatePickerWizard : IDisposable
    {
        public event MedicalStateCreated StateCreated;
        public event StatePickerFinished Finished;

        private List<StatePickerPanel> panels = new List<StatePickerPanel>(3);
        int currentIndex = 0;
        private bool updatePanel = true;
        TemporaryStateBlender stateBlender;
        NavigationController navigationController;
        private DrawingWindow currentDrawingWindow;
        private LayerController layerController;
        private LayerState layerStatusBeforeShown;
        private String navigationStateBeforeShown;
        private StatePickerPanelHost panelHost;

        private ImageList pickerImageList;

        private StatePickerUIHost uiHost;

        public StatePickerWizard(String name, StatePickerUIHost uiHost, TemporaryStateBlender stateBlender, NavigationController navigationController, LayerController layerController)
        {
            this.Name = name;
            this.uiHost = uiHost;

            pickerImageList = new ImageList();
            pickerImageList.ColorDepth = ColorDepth.Depth32Bit;
            pickerImageList.ImageSize = new Size(100, 100);

            this.panelHost = new StatePickerPanelHost(this);

            this.stateBlender = stateBlender;
            this.navigationController = navigationController;
            this.layerController = layerController;
        }

        public void Dispose()
        {
            pickerImageList.Dispose();
            uiHost.Dispose();
            panelHost.Dispose();
        }

        public void addStatePanel(StatePickerPanel panel)
        {
            panel.setStatePicker(this);
            panels.Add(panel);
        }

        public void startWizard(DrawingWindow controllingWindow)
        {
            layerStatusBeforeShown = new LayerState("Temp");
            layerStatusBeforeShown.captureState();
            currentDrawingWindow = controllingWindow;
            navigationStateBeforeShown = navigationController.getNavigationState(currentDrawingWindow).Name;
            stateBlender.recordUndoState();
            foreach (StatePickerPanel panel in panels)
            {
                uiHost.addMode(panel);
                panel.recordOpeningState();
            }
            hidePanel();
            currentIndex = 0;
            showPanel();
            uiHost.setCurrentWizard(this);
        }

        public void setToDefault()
        {
            foreach (StatePickerPanel panel in panels)
            {
                panel.setToDefault();
            }
        }

        /// <summary>
        /// Call this function to force the handle to be created to avoid lag
        /// the first time the wizard is opened. This should be done after all
        /// images are loaded into the wizard.
        /// </summary>
        public void initializeImageHandle()
        {
            IntPtr handle = pickerImageList.Handle;
        }

        public void show()
        {
            uiHost.Visible = true;
        }

        public void close()
        {
            uiHost.clearModes();
            hidePanel();
            uiHost.Visible = false;
            layerController.CurrentLayerState = layerStatusBeforeShown;
            navigationController.setNavigationState(navigationStateBeforeShown, currentDrawingWindow);
            if (Finished != null)
            {
                Finished.Invoke();
            }
        }

        public bool Visible
        {
            get
            {
                return uiHost.Visible;
            }
        }

        public Control WizardControl
        {
            get
            {
                return panelHost;
            }
        }

        internal ImageList ImageList
        {
            get
            {
                return pickerImageList;
            }
        }

        public TemporaryStateBlender StateBlender
        {
            get
            {
                return stateBlender;
            }
        }

        public String Name { get; private set; }

        internal void showChanges(bool immediate, bool captureCurrentState)
        {
            MedicalState createdState;
            if (captureCurrentState)
            {
                createdState = stateBlender.createBaselineState();
            }
            else
            {
                createdState = new MedicalState("");
            }
            foreach (StatePickerPanel panel in panels)
            {
                panel.applyToState(createdState);
            }
            if (immediate)
            {
                createdState.blend(1.0f, createdState);
            }
            else
            {
                stateBlender.startTemporaryBlend(createdState);
            }
        }

        internal void next()
        {
            hidePanel();
            currentIndex++;
            showPanel();
        }

        internal void previous()
        {
            hidePanel();
            currentIndex--;
            showPanel();
        }

        internal void finish()
        {
            MedicalState createdState = new MedicalState("Test");
            foreach (StatePickerPanel panel in panels)
            {
                panel.applyToState(createdState);
            }
            stateBlender.stopBlend();
            if (StateCreated != null)
            {
                StateCreated.Invoke(createdState);
            }
            this.close();
        }

        internal void cancel()
        {
            stateBlender.blendToUndo();
            foreach (StatePickerPanel panel in panels)
            {
                panel.resetToOpeningState();
            }
            this.close();
        }

        public void modeChanged(int modeIndex)
        {
            if (updatePanel)
            {
                hidePanel();
                currentIndex = uiHost.SelectedIndex;
                showPanel();
            }
        }

        private void showPanel()
        {
            if (updatePanel)
            {
                updatePanel = false;
                StatePickerPanel panel = panels[currentIndex];
                panel.callPanelOpening();
                panelHost.showPanel(panel);
                uiHost.SelectedIndex = currentIndex;
                panelHost.NextButtonVisible = !(currentIndex == panels.Count - 1);
                panelHost.PreviousButtonVisible = !(currentIndex == 0);
                if (panel.NavigationState != null)
                {
                    setNavigationState(panel.NavigationState);
                }
                if (panel.LayerState != null)
                {
                    setLayerState(panel.LayerState);
                }
                panel.modifyScene();
                updatePanel = true;
            }
        }

        internal void setNavigationState(String name)
        {
            navigationController.setNavigationState(name, currentDrawingWindow);
        }

        internal void setLayerState(String name)
        {
            layerController.applyLayerState(name);
        }

        private void hidePanel()
        {
            panels[currentIndex].callPanelClosing();
            panelHost.hidePanel(panels[currentIndex]);
        }
    }
}
