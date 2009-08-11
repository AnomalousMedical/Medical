using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Engine.ObjectManagement;
using Engine;

namespace Medical.GUI
{
    public partial class TeethControl : GUIElement
    {
        private MedicalController medicalController;

        public TeethControl()
        {
            InitializeComponent();
            recessUpDown.ValueChanged += offsetValueChanged;
            leftRightUpDown.ValueChanged += offsetValueChanged;
            forwardBackUpDown.ValueChanged += offsetValueChanged;

            numericUpDown1.ValueChanged += rotateValueChanged;
            numericUpDown2.ValueChanged += rotateValueChanged;
            numericUpDown3.ValueChanged += rotateValueChanged;
        }

        public void initialize(MedicalController medicalController)
        {
            this.medicalController = medicalController;
        }

        protected override void sceneLoaded(SimScene scene)
        {
            foreach (CheckBox control in teethPanel.Controls)
            {
                control.Visible = TeethController.hasTooth(control.Tag.ToString());
                control.Checked = false;
            }
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

        void offsetValueChanged(object sender, EventArgs e)
        {
            Vector3 offset = new Vector3((float)leftRightUpDown.Value, (float)recessUpDown.Value, (float)forwardBackUpDown.Value);
            foreach (CheckBox control in teethPanel.Controls)
            {
                if (control.Checked)
                {
                    Tooth tooth = TeethController.getTooth(control.Tag.ToString());
                    tooth.Offset = offset;
                }
            }
        }

        void rotateValueChanged(object sender, EventArgs e)
        {
            Quaternion rot = new Quaternion((float)numericUpDown1.Value, (float)numericUpDown2.Value, (float)numericUpDown3.Value);
            foreach (CheckBox control in teethPanel.Controls)
            {
                if (control.Checked)
                {
                    Tooth tooth = TeethController.getTooth(control.Tag.ToString());
                    tooth.Rotation = rot;
                }
            }
        }
    }
}
