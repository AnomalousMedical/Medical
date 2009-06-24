using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI.StateWizard
{
    public partial class StateWizardForm : Form
    {
        private List<StateWizardPanel> panels = new List<StateWizardPanel>(3);
        int currentIndex = 0;
        MedicalState createdState;

        public StateWizardForm()
        {
            InitializeComponent();
            panels.Add(new MandiblePanel());
            panels.Add(new JointPanel());
            panels.Add(new TeethPanel());
        }

        public void startWizard()
        {
            foreach (StateWizardPanel panel in panels)
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
            Controls.Add(panels[currentIndex]);
            Size size = this.PreferredSize;
            size.Height += buttonPanel.PreferredSize.Height;
            this.Size = size;
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
        }

        private void hidePanel()
        {
            Controls.Remove(panels[currentIndex]);
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
            foreach (StateWizardPanel panel in panels)
            {
                panel.applyToState(createdState);
            }
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
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
