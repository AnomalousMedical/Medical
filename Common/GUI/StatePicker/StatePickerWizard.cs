using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using WeifenLuo.WinFormsUI.Docking;

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
        private StatePickerModeList modeList;
        private StatePickerPanelHost panelHost;

        private ImageList pickerImageList;

        private StatePickerUIHost uiHost;

        public StatePickerWizard(StatePickerUIHost uiHost, TemporaryStateBlender stateBlender, NavigationController navigationController, LayerController layerController)
        {
            this.uiHost = uiHost;

            pickerImageList = new ImageList();
            pickerImageList.ColorDepth = ColorDepth.Depth32Bit;
            pickerImageList.ImageSize = new Size(100, 100);

            this.modeList = new StatePickerModeList(pickerImageList, this);
            this.panelHost = new StatePickerPanelHost(this);

            this.stateBlender = stateBlender;
            this.navigationController = navigationController;
            this.layerController = layerController;
        }

        public void Dispose()
        {
            pickerImageList.Dispose();
            modeList.Dispose();
            panelHost.Dispose();
        }

        public void addStatePanel(StatePickerPanel panel)
        {
            panel.setStatePicker(this);
            panels.Add(panel);
            modeList.addMode(panel);
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
                panel.recordOpeningState();
                modeList.updateImage(panel);
            }
            hidePanel();
            currentIndex = 0;
            showPanel();
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
            uiHost.setDataControl(panelHost);
            uiHost.setTopInformation(modeList);
        }

        public void close()
        {
            hidePanel();
            uiHost.setDataControl(null);
            uiHost.setTopInformation(null);
            layerStatusBeforeShown.apply();
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
                return modeList.Visible;
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

        internal void showChanges(bool immediate)
        {
            MedicalState createdState = new MedicalState("");
            foreach (StatePickerPanel panel in panels)
            {
                panel.applyToState(createdState);
                modeList.updateImage(panel);
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

        internal void modeChanged(int modeIndex)
        {
            if (updatePanel)
            {
                hidePanel();
                currentIndex = modeList.SelectedIndex;
                showPanel();
            }
        }

        private void showPanel()
        {
            if (updatePanel)
            {
                updatePanel = false;
                StatePickerPanel panel = panels[currentIndex];
                panelHost.showPanel(panel);
                modeList.SelectedIndex = currentIndex;
                panelHost.NextButtonVisible = !(currentIndex == panels.Count - 1);
                panelHost.PreviousButtonVisible = !(currentIndex == 0);
                if (panel.NavigationState != null)
                {
                    navigationController.setNavigationState(panel.NavigationState, currentDrawingWindow);
                }
                if (panel.LayerState != null)
                {
                    layerController.applyLayerState(panel.LayerState);
                }
                panel.modifyScene();
                updatePanel = true;
            }
        }

        private void hidePanel()
        {
            panels[currentIndex].callPanelClosing();
            panelHost.hidePanel(panels[currentIndex]);
        }
    }
}
