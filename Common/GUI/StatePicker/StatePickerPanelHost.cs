using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

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
            Color panelColor = KryptonManager.CurrentGlobalPalette.GetBackColor1(panelHost.PanelBackStyle, PaletteState.ContextNormal);
            if (panel.BackColor != panelColor)
            {
                panel.BackColor = panelColor;
            }
            panelHost.Controls.Add(panel);
            if (panel.AutoSize == true)
            {
                panel.Dock = DockStyle.Fill;
            }
        }

        public void hidePanel(StatePickerPanel panel)
        {
            panelHost.AutoScrollPosition = Point.Empty;
            panelHost.Controls.Remove(panel);
        }

        public bool NextButtonVisible
        {
            get
            {
                return nextButton.Enabled;
            }
            set
            {
                nextButton.Enabled = value;
                if (!value)
                {
                    previousButton.Focus();
                }
            }
        }

        public bool PreviousButtonVisible
        {
            get
            {
                return previousButton.Enabled;
            }
            set
            {
                previousButton.Enabled = value;
                if (!value)
                {
                    nextButton.Focus();
                }
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
