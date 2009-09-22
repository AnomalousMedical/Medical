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

        public StatePicker()
        {
            InitializeComponent();
            navigatorList.SelectedIndexChanged += new EventHandler(navigatorList_SelectedIndexChanged);
        }

        public void initialize(TemporaryStateBlender stateBlender)
        {
            this.stateBlender = stateBlender;
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
            panel.initialize(presetSet);
            addStatePanel(panel);
            return panel;
        }

        public void startWizard()
        {
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
            if (Finished != null)
            {
                Finished.Invoke();
            }
            e.Cancel = true;
            this.Hide();
        }
    }
}
