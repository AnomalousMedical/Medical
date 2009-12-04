using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.ObjectManagement;
using Engine;
using Engine.Platform;

namespace Medical.GUI
{
    public partial class TeethControl : GUIElement
    {
        private MedicalController medicalController;
        private bool allowUpdates = true;

        public TeethControl()
        {
            InitializeComponent();

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

        protected override void sceneUnloading()
        {
            base.sceneUnloading();
            adaptButton.Checked = false;
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

        private void resetButton_Click(object sender, EventArgs e)
        {
            TeethController.setAllOffsets(Vector3.Zero);
            TeethController.setAllRotations(Quaternion.Identity);
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

        private void highlightCollisionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            TeethController.HighlightContacts = highlightCollisionCheckBox.Checked;
        }

        private void adaptButton_CheckedChanged(object sender, EventArgs e)
        {
            if (adaptButton.Checked)
            {
                foreach (CheckBox control in teethPanel.Controls)
                {
                    if (control.Checked)
                    {
                        TeethController.adaptSingleTooth(control.Tag.ToString(), true);
                    }
                }
            }
            else
            {
                foreach (CheckBox control in teethPanel.Controls)
                {
                    TeethController.adaptSingleTooth(control.Tag.ToString(), false);
                }
            }
        }

        private void moveButton_Click(object sender, EventArgs e)
        {
            TeethController.TeethMover.ShowMoveTools = true;
            TeethController.TeethMover.ShowRotateTools = false;
            TeethController.showTeethTools(!hideTop.Checked, !hideBottom.Checked);
        }

        private void rotateButton_Click(object sender, EventArgs e)
        {
            TeethController.TeethMover.ShowMoveTools = false;
            TeethController.TeethMover.ShowRotateTools = true;
            TeethController.showTeethTools(!hideTop.Checked, !hideBottom.Checked);
        }

        private void offButton_Click(object sender, EventArgs e)
        {
            TeethController.TeethMover.ShowMoveTools = false;
            TeethController.TeethMover.ShowRotateTools = false;
            TeethController.showTeethTools(!hideTop.Checked, !hideBottom.Checked);
        }

        private void hideTop_CheckedChanged(object sender, EventArgs e)
        {
            TeethController.showTeethTools(!hideTop.Checked, !hideBottom.Checked);
            TransparencyGroup teeth = TransparencyController.getTransparencyGroup(RenderGroup.Teeth);
            for (int i = 1; i < 17; ++i)
            {
                teeth.getTransparencyObject("Tooth " + i).setAlpha(hideTop.Checked ? 0.0f : 1.0f);
            }
        }

        private void hideBottom_CheckedChanged(object sender, EventArgs e)
        {
            TeethController.showTeethTools(!hideTop.Checked, !hideBottom.Checked);
            TransparencyGroup teeth = TransparencyController.getTransparencyGroup(RenderGroup.Teeth);
            for (int i = 17; i < 33; ++i)
            {
                teeth.getTransparencyObject("Tooth " + i).setAlpha(hideBottom.Checked ? 0.0f : 1.0f);
            }
        }
    }
}
