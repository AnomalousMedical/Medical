using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Resources;

namespace Medical.GUI
{
    public delegate void MedicalStateCreated(MedicalState state);
    public delegate void StatePickerFinished();

    public partial class StatePicker : GUIElement
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

        public StatePicker()
        {
            InitializeComponent();
            navigatorList.SelectedIndexChanged += new EventHandler(navigatorList_SelectedIndexChanged);
        }

        public void initialize(TemporaryStateBlender stateBlender, NavigationController navigationController, LayerController layerController)
        {
            this.stateBlender = stateBlender;
            this.navigationController = navigationController;
            this.layerController = layerController;
        }

        public StatePickerPanel addStatePanel(StatePickerPanel panel)
        {
            panel.setStatePicker(this);
            panels.Add(panel);
            ListViewItem item = navigatorList.Items.Add(panel.Text, panel.Text, panel.Text);
            item.Tag = panel;
            return panel;
        }

        public PresetStatePanel addPresetStateSet(PresetStateSet presetSet)
        {
            PresetStatePanel panel = new PresetStatePanel();
            panel.Text = presetSet.Name;
            panel.initialize(presetSet);
            addStatePanel(panel);
            return panel;
        }

        public void startWizard(DrawingWindow controllingWindow)
        {
            layerStatusBeforeShown = new LayerState("Temp");
            layerStatusBeforeShown.captureState();
            currentDrawingWindow = controllingWindow;
            navigationStateBeforeShown = navigationController.getNavigationState(currentDrawingWindow).Name;
            stateBlender.recordUndoState();
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

        internal void showChanges(bool immediate)
        {
            MedicalState createdState = new MedicalState("Test");
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

        private void showPanel()
        {
            if (updatePanel)
            {
                updatePanel = false;
                StatePickerPanel panel = panels[currentIndex];
                this.statePickerPanelHost.Controls.Add(panel);
                navigatorList.SelectedItems.Clear();
                ListViewItem item = navigatorList.Items[panel.Text];
                item.Selected = true;
                navigatorList.Select();
                if (currentIndex == panels.Count - 1)
                {
                    nextButton.Visible = false;
                }
                else
                {
                    nextButton.Visible = true;
                }
                if (currentIndex == 0)
                {
                    previousButton.Visible = false;
                }
                else
                {
                    previousButton.Visible = true;
                }
                if (panel.NavigationState != null)
                {
                    navigationController.setNavigationState(panel.NavigationState, currentDrawingWindow);
                }
                if (panel.LayerState != null)
                {
                    layerController.applyLayerState(panel.LayerState);
                }
                updatePanel = true;
            }
        }

        private void hidePanel()
        {
            this.statePickerPanelHost.Controls.Remove(panels[currentIndex]);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            hidePanel();
            currentIndex++;
            showPanel();
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            hidePanel();
            currentIndex--;
            showPanel();
        }

        private void finishButton_Click(object sender, EventArgs e)
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
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            stateBlender.blendToUndo();
            this.Close();
        }

        void navigatorList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatePanel && navigatorList.SelectedIndices.Count > 0)
            {
                hidePanel();
                currentIndex = navigatorList.SelectedIndices[0];
                showPanel();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            layerStatusBeforeShown.apply();
            navigationController.setNavigationState(navigationStateBeforeShown, currentDrawingWindow);
            if (Finished != null)
            {
                Finished.Invoke();
            }
            e.Cancel = true;
            this.Hide();
        }
    }
}
