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
    public partial class StatePicker : Form
    {
        private List<StatePickerPanel> panels = new List<StatePickerPanel>(3);
        int currentIndex = 0;
        MedicalState createdState;
        private bool updatePanel = true;

        public StatePicker()
        {
            InitializeComponent();
            navigatorList.SelectedIndexChanged += new EventHandler(navigatorList_SelectedIndexChanged);
        }

        public void addStatePanel(StatePickerPanel panel)
        {
            panels.Add(panel);
            ListViewItem item = navigatorList.Items.Add(panel.Text, panel.Text, panel.Text);
            item.Tag = panel;
        }

        public void addPresetStateSet(PresetStateSet presetSet, ResourceManager imageResources)
        {
            PresetStatePanel panel = new PresetStatePanel();
            panel.initialize(presetSet, imageResources);
            addStatePanel(panel);
        }

        public void startWizard()
        {
            foreach (StatePickerPanel panel in panels)
            {
                panel.setToDefault();
            }
            hidePanel();
            currentIndex = 0;
            showPanel();
            createdState = null;
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
                    finishButton.Visible = true;
                }
                else
                {
                    nextButton.Visible = true;
                    finishButton.Visible = false;
                }
                if (currentIndex == 0)
                {
                    previousButton.Visible = false;
                    cancelButton.Visible = true;
                }
                else
                {
                    previousButton.Visible = true;
                    cancelButton.Visible = false;
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
            createdState = new MedicalState("Test");
            foreach (StatePickerPanel panel in panels)
            {
                panel.applyToState(createdState);
            }
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
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

        public bool WizardFinished
        {
            get
            {
                return createdState != null;
            }
        }

        public MedicalState CreatedState
        {
            get
            {
                return createdState;
            }
        }
    }
}
