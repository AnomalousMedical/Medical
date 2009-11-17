using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI
{
    public partial class StatePickerPanelHost : UserControl
    {
        private StatePickerWizard controller;

        public StatePickerPanelHost(StatePickerWizard controller)
        {
            InitializeComponent();
            this.controller = controller;
        }

        public void showPanel(StatePickerPanel panel)
        {
            panelHost.Controls.Add(panel);
            if (panel.AutoSize == true)
            {
                panel.Dock = DockStyle.Fill;
            }
        }

        public void hidePanel(StatePickerPanel panel)
        {
            panelHost.Controls.Remove(panel);
        }

        public bool NextButtonVisible
        {
            get
            {
                return nextButton.Visible;
            }
            set
            {
                nextButton.Visible = value;
            }
        }

        public bool PreviousButtonVisible
        {
            get
            {
                return previousButton.Visible;
            }
            set
            {
                previousButton.Visible = value;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            controller.cancel();
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            controller.previous();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            controller.next();
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            controller.finish();
        }
    }
}
