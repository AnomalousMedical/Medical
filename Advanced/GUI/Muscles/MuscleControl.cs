﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Medical.Properties;
using Engine.ObjectManagement;

namespace Medical.GUI
{
    public partial class MuscleControl : GUIElement
    {
        private bool allowMuscleUpdates = true;

        public MuscleControl()
        {
            InitializeComponent(); forceUpDown.ValueChanged += new EventHandler(forceUpDown_ValueChanged);

            rightMasseter.CheckedChanged += new System.EventHandler(this.muscleButton_CheckedChanged);
            rightDigastric.CheckedChanged += new System.EventHandler(this.muscleButton_CheckedChanged);
            rightLateralPterygoid.CheckedChanged += new System.EventHandler(this.muscleButton_CheckedChanged);
            rightMedialPterygoid.CheckedChanged += new System.EventHandler(this.muscleButton_CheckedChanged);
            rightTemporalis.CheckedChanged += new System.EventHandler(this.muscleButton_CheckedChanged);

            leftDigastric.CheckedChanged += new System.EventHandler(this.muscleButton_CheckedChanged);
            leftLateralPterygoid.CheckedChanged += new System.EventHandler(this.muscleButton_CheckedChanged);
            leftMasseter.CheckedChanged += new System.EventHandler(this.muscleButton_CheckedChanged);
            leftMedialPterygoid.CheckedChanged += new System.EventHandler(this.muscleButton_CheckedChanged);
            leftTemporalis.CheckedChanged += new System.EventHandler(this.muscleButton_CheckedChanged);

            leftTemporalisForce.ValueChanged += new EventHandler(individualMuscle_ValueChanged);
            leftMasseterForce.ValueChanged += new EventHandler(individualMuscle_ValueChanged);
            leftMedPtForce.ValueChanged += new EventHandler(individualMuscle_ValueChanged);
            leftLatPtForce.ValueChanged += new EventHandler(individualMuscle_ValueChanged);
            leftDigastricForce.ValueChanged += new EventHandler(individualMuscle_ValueChanged);

            rightTemporalisForce.ValueChanged += new EventHandler(individualMuscle_ValueChanged);
            rightMasseterForce.ValueChanged += new EventHandler(individualMuscle_ValueChanged);
            rightMedPtForce.ValueChanged += new EventHandler(individualMuscle_ValueChanged);
            rightLatPtForce.ValueChanged += new EventHandler(individualMuscle_ValueChanged);
            rightDigastricForce.ValueChanged += new EventHandler(individualMuscle_ValueChanged);

            rightMuscleGroups.SelectedValueChanged += new EventHandler(rightMuscleGroups_SelectedValueChanged);
            leftMuscleGroups.SelectedValueChanged += new EventHandler(leftMuscleGroups_SelectedValueChanged);
            bothSides.SelectedValueChanged += new EventHandler(bothSides_SelectedValueChanged);
            setupMuscleGroups();

            muscleSequenceView.LargeImageList = new ImageList();
            muscleSequenceView.LargeImageList.ImageSize = new Size(108, 120);
            muscleSequenceView.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
            muscleSequenceView.LargeImageList.Images.Add("OpenIcon", Resources.openmuscle);
            muscleSequenceView.LargeImageList.Images.Add("CloseIcon", Resources.clenchedmuscle);
        }

        protected override void sceneLoaded(SimScene scene)
        {
            captureMuscleForces();
            clearSelection();

            MuscleGroup group = leftMuscleGroups.SelectedItem as MuscleGroup;
            if (group != null)
            {
                setLeftMuscleGroup(group);
            }

            group = rightMuscleGroups.SelectedItem as MuscleGroup;
            if (group != null)
            {
                setRightMuscleGroup(group);
            }

            muscleSequenceView.initializeSequences();
        }

        protected override void sceneUnloading()
        {
            base.sceneUnloading();
            muscleSequenceView.clearSequences();
        }

        void bothSides_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            if (combo != null)
            {
                rightMuscleGroups.SelectedIndex = combo.SelectedIndex;
                leftMuscleGroups.SelectedIndex = combo.SelectedIndex;
                leftMuscleGroups_SelectedValueChanged(sender, e);
                rightMuscleGroups_SelectedValueChanged(sender, e);
            }
        }

        void leftMuscleGroups_SelectedValueChanged(object sender, EventArgs e)
        {

            MuscleGroup group = leftMuscleGroups.SelectedItem as MuscleGroup;
            if (group != null)
            {
                setLeftMuscleGroup(group);
            }
        }

        void rightMuscleGroups_SelectedValueChanged(object sender, EventArgs e)
        {
            MuscleGroup group = rightMuscleGroups.SelectedItem as MuscleGroup;
            if (group != null)
            {
                setRightMuscleGroup(group);
            }
        }

        void forceUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (allowMuscleUpdates)
            {
                MuscleController.changeForce((float)forceUpDown.Value);
                captureMuscleForces();
            }
        }

        void individualMuscle_ValueChanged(object sender, EventArgs e)
        {
            if (allowMuscleUpdates)
            {
                MuscleController.changeForce((String)((NumericUpDown)sender).Tag, (float)((NumericUpDown)sender).Value);
            }
        }

        private void muscleButton_CheckedChanged(object sender, EventArgs e)
        {
            MuscleController.selectMuscle((String)((CheckBox)sender).Tag, ((CheckBox)sender).Checked);
        }

        private void captureMuscleForces()
        {
            allowMuscleUpdates = false;
            foreach (Control control in tabPage2.Controls)
            {
                NumericUpDown upDown = control as NumericUpDown;
                if (upDown != null && upDown.Tag != null)
                {
                    MuscleBehavior muscle = MuscleController.getMuscle(upDown.Tag.ToString());
                    upDown.Value = muscle != null ? (decimal)muscle.getForce() : 0m;
                }
            }
            allowMuscleUpdates = true;
        }

        private void setRightMuscleGroup(MuscleGroup group)
        {
            rightMasseter.Checked = group.getActivation(rightMasseter.Tag.ToString());
            rightDigastric.Checked = group.getActivation(rightDigastric.Tag.ToString());
            rightLateralPterygoid.Checked = group.getActivation(rightLateralPterygoid.Tag.ToString());
            rightMedialPterygoid.Checked = group.getActivation(rightMedialPterygoid.Tag.ToString());
            rightTemporalis.Checked = group.getActivation(rightTemporalis.Tag.ToString());
        }

        private void setLeftMuscleGroup(MuscleGroup group)
        {
            leftMasseter.Checked = group.getActivation(leftMasseter.Tag.ToString());
            leftDigastric.Checked = group.getActivation(leftDigastric.Tag.ToString());
            leftLateralPterygoid.Checked = group.getActivation(leftLateralPterygoid.Tag.ToString());
            leftMedialPterygoid.Checked = group.getActivation(leftMedialPterygoid.Tag.ToString());
            leftTemporalis.Checked = group.getActivation(leftTemporalis.Tag.ToString());
        }

        private void clearSelection()
        {
            rightMasseter.Checked = false;
            rightDigastric.Checked = false;
            rightLateralPterygoid.Checked = false;
            rightMedialPterygoid.Checked = false;
            rightTemporalis.Checked = false;

            leftMasseter.Checked = false;
            leftDigastric.Checked = false;
            leftLateralPterygoid.Checked = false;
            leftMedialPterygoid.Checked = false;
            leftTemporalis.Checked = false;
        }

        private void clearSelectionButton_Click(object sender, EventArgs e)
        {
            clearSelection();
        }

        private void setupMuscleGroups()
        {
            MuscleGroup clear = new MuscleGroup("Clear");
            rightMuscleGroups.Items.Add(clear);
            leftMuscleGroups.Items.Add(clear);

            //Right

            MuscleGroup rightElevators = new MuscleGroup("All Elevators");
            rightElevators.setActivation(rightMasseter.Tag.ToString(), true);
            rightElevators.setActivation(rightTemporalis.Tag.ToString(), true);
            rightElevators.setActivation(rightMedialPterygoid.Tag.ToString(), true);
            rightMuscleGroups.Items.Add(rightElevators);

            MuscleGroup rightAllGroups = new MuscleGroup("All Groups");
            rightAllGroups.setActivation(rightMasseter.Tag.ToString(), true);
            rightAllGroups.setActivation(rightDigastric.Tag.ToString(), true);
            rightAllGroups.setActivation(rightLateralPterygoid.Tag.ToString(), true);
            rightAllGroups.setActivation(rightMedialPterygoid.Tag.ToString(), true);
            rightAllGroups.setActivation(rightTemporalis.Tag.ToString(), true);
            rightMuscleGroups.Items.Add(rightAllGroups);

            MuscleGroup rightDepressors = new MuscleGroup("Depressors");
            rightDepressors.setActivation(rightDigastric.Tag.ToString(), true);
            rightDepressors.setActivation(rightLateralPterygoid.Tag.ToString(), true);
            rightMuscleGroups.Items.Add(rightDepressors);

            //Left

            MuscleGroup leftElevators = new MuscleGroup("All Elevators");
            leftElevators.setActivation(leftMasseter.Tag.ToString(), true);
            leftElevators.setActivation(leftTemporalis.Tag.ToString(), true);
            leftElevators.setActivation(leftMedialPterygoid.Tag.ToString(), true);
            leftMuscleGroups.Items.Add(leftElevators);

            MuscleGroup leftAllGroups = new MuscleGroup("All Groups");
            leftAllGroups.setActivation(leftMasseter.Tag.ToString(), true);
            leftAllGroups.setActivation(leftDigastric.Tag.ToString(), true);
            leftAllGroups.setActivation(leftLateralPterygoid.Tag.ToString(), true);
            leftAllGroups.setActivation(leftMedialPterygoid.Tag.ToString(), true);
            leftAllGroups.setActivation(leftTemporalis.Tag.ToString(), true);
            leftMuscleGroups.Items.Add(leftAllGroups);

            MuscleGroup leftDepressors = new MuscleGroup("Depressors");
            leftDepressors.setActivation(leftDigastric.Tag.ToString(), true);
            leftDepressors.setActivation(leftLateralPterygoid.Tag.ToString(), true);
            leftMuscleGroups.Items.Add(leftDepressors);

            foreach (Object obj in rightMuscleGroups.Items)
            {
                bothSides.Items.Add(obj.ToString());
            }

        }
    }
}
