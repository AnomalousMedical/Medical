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
        private bool allowUpdates = true;

        public TeethControl()
        {
            InitializeComponent();
            upDownUpDown.ValueChanged += offsetValueChanged;
            leftRightUpDown.ValueChanged += offsetValueChanged;
            forwardBackUpDown.ValueChanged += offsetValueChanged;

            yawUpDown.ValueChanged += rotateValueChanged;
            rollUpDown.ValueChanged += rotateValueChanged;
            pitchUpDown.ValueChanged += rotateValueChanged;

            setupTeethGroups();

            selectionSetCombo.SelectedIndexChanged += selectionSetCombo_SelectedIndexChanged;
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
            if (allowUpdates)
            {
                Vector3 offset = new Vector3((float)leftRightUpDown.Value, (float)upDownUpDown.Value, (float)forwardBackUpDown.Value);
                foreach (CheckBox control in teethPanel.Controls)
                {
                    if (control.Checked)
                    {
                        Tooth tooth = TeethController.getTooth(control.Tag.ToString());
                        tooth.Offset = offset;
                    }
                }
            }
        }

        void rotateValueChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                Quaternion rot = new Quaternion((float)yawUpDown.Value, (float)rollUpDown.Value, (float)pitchUpDown.Value);
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

        private void resetButton_Click(object sender, EventArgs e)
        {
            allowUpdates = false;
            leftRightUpDown.Value = 0.0m;
            upDownUpDown.Value = 0.0m;
            yawUpDown.Value = 0.0m;
            rollUpDown.Value = 0.0m;
            allowUpdates = true;
            pitchUpDown.Value = 0.0m;
            forwardBackUpDown.Value = 0.0m;
        }

        void selectionSetCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            TeethSelectionGroup group = selectionSetCombo.SelectedItem as TeethSelectionGroup;
            foreach (CheckBox control in teethPanel.Controls)
            {
                control.Checked = group.isSelected(control.Tag.ToString());
            }
        }

        private void setupTeethGroups()
        {
            TeethSelectionGroup group;
            //Clear
            group = new TeethSelectionGroup("Clear");
            selectionSetCombo.Items.Add(group);

            //All
            group = new TeethSelectionGroup("All");
            for (int i = 1; i < 33; ++i)
            {
                if (i < 10)
                {
                    group.addTooth("Tooth0" + i);
                }
                else
                {
                    group.addTooth("Tooth" + i);
                }
            }
            selectionSetCombo.Items.Add(group);

            //Top teeth
            group = new TeethSelectionGroup("Top Teeth");
            for (int i = 1; i < 17; ++i)
            {
                if (i < 10)
                {
                    group.addTooth("Tooth0" + i);
                }
                else
                {
                    group.addTooth("Tooth" + i);
                }
            }
            selectionSetCombo.Items.Add(group);

            //Bottom teeth
            group = new TeethSelectionGroup("Bottom Teeth");
            for (int i = 17; i < 33; ++i)
            {
                group.addTooth("Tooth" + i);
            }
            selectionSetCombo.Items.Add(group);
        }
    }
}
