using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Medical.GUI
{
    public partial class TeethControl : DockContent
    {
        private MedicalController medicalController;

        public TeethControl()
        {
            InitializeComponent();
        }

        public void initialize(MedicalController medicalController)
        {
            this.medicalController = medicalController;
        }

        public void sceneChanged()
        {
            foreach (CheckBox control in teethPanel.Controls)
            {
                control.Visible = TeethController.hasTooth(control.Tag.ToString());
                control.Checked = false;
            }
        }

        public void sceneUnloading()
        {

        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            foreach (CheckBox control in teethPanel.Controls)
            {
                if (control.Checked)
                {
                    Tooth tooth = TeethController.getTooth(control.Tag.ToString());
                    tooth.Extracted = true;
                }
            }
        }

        private void restoreButton_Click(object sender, EventArgs e)
        {
            foreach (CheckBox control in teethPanel.Controls)
            {
                if (control.Checked)
                {
                    Tooth tooth = TeethController.getTooth(control.Tag.ToString());
                    tooth.Extracted = false;
                }
            }
        }
    }
}
