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
        private const String RightTemporalisDynamic = "RightTemporalisDynamic";
        private const String RightMasseterDynamic = "RightMasseterDynamic";
        private const String RightMedialPterygoidDynamic = "RightMedialPterygoidDynamic";
        private const String RightLateralPterygoidDynamic = "RightLateralPterygoidDynamic";
        private const String RightDigastricDynamic = "RightDigastricDynamic";

        private const String LeftTemporalisDynamic = "LeftTemporalisDynamic";
        private const String LeftMasseterDynamic = "LeftMasseterDynamic";
        private const String LeftMedialPterygoidDynamic = "LeftMedialPterygoidDynamic";
        private const String LeftLateralPterygoidDynamic = "LeftLateralPterygoidDynamic";
        private const String LeftDigastricDynamic = "LeftDigastricDynamic";

        private const float CP_MOVE_SPEED = 0.25f;

        private ControlPointBehavior leftCP;
        private ControlPointBehavior rightCP;

        public MuscleControl()
        {
            InitializeComponent();
            muscleSequenceView.LargeImageList = new ImageList();
            muscleSequenceView.LargeImageList.ImageSize = new Size(108, 120);
            muscleSequenceView.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
            muscleSequenceView.LargeImageList.Images.Add("OpenIcon", Resources.openmuscle);
            muscleSequenceView.LargeImageList.Images.Add("CloseIcon", Resources.clenchedmuscle);
        }

        protected override void sceneLoaded(SimScene scene)
        {
            leftCP = ControlPointController.getControlPoint("LeftCP");
            rightCP = ControlPointController.getControlPoint("RightCP");
            this.Enabled = leftCP != null && rightCP != null;
            muscleSequenceView.initializeSequences();
        }

        protected override void sceneUnloading()
        {
            leftCP = null;
            rightCP = null;
            muscleSequenceView.clearSequences();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            MuscleController.changeForce(RightDigastricDynamic, 0.0f);
            MuscleController.changeForce(RightLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightMasseterDynamic, 3.0f);
            MuscleController.changeForce(RightMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightTemporalisDynamic, 3.0f);

            MuscleController.changeForce(LeftDigastricDynamic, 0.0f);
            MuscleController.changeForce(LeftLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftMasseterDynamic, 3.0f);
            MuscleController.changeForce(LeftMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftTemporalisDynamic, 3.0f);
            TeethController.setTeethLoose(false);
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            MuscleController.changeForce(RightDigastricDynamic, 5.0f);
            MuscleController.changeForce(RightLateralPterygoidDynamic, 5.0f);
            MuscleController.changeForce(RightMasseterDynamic, 0.0f);
            MuscleController.changeForce(RightMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightTemporalisDynamic, 0.0f);

            MuscleController.changeForce(LeftDigastricDynamic, 5.0f);
            MuscleController.changeForce(LeftLateralPterygoidDynamic, 5.0f);
            MuscleController.changeForce(LeftMasseterDynamic, 0.0f);
            MuscleController.changeForce(LeftMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftTemporalisDynamic, 0.0f);
            TeethController.setTeethLoose(false);
        }

        private void clenchButton_Click(object sender, EventArgs e)
        {
            float clenchForce = 15.0f;
            MuscleController.changeForce(RightDigastricDynamic, 0.0f);
            MuscleController.changeForce(RightLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightMasseterDynamic, clenchForce);
            MuscleController.changeForce(RightMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightTemporalisDynamic, clenchForce);

            MuscleController.changeForce(LeftDigastricDynamic, 0.0f);
            MuscleController.changeForce(LeftLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftMasseterDynamic, clenchForce);
            MuscleController.changeForce(LeftMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftTemporalisDynamic, clenchForce);
            TeethController.setTeethLoose(true);
        }

        private void neutralButton_Click(object sender, EventArgs e)
        {
            MuscleController.changeForce(RightDigastricDynamic, 0.0f);
            MuscleController.changeForce(RightLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightMasseterDynamic, 0.0f);
            MuscleController.changeForce(RightMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightTemporalisDynamic, 0.0f);

            MuscleController.changeForce(LeftDigastricDynamic, 0.0f);
            MuscleController.changeForce(LeftLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftMasseterDynamic, 0.0f);
            MuscleController.changeForce(LeftMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftTemporalisDynamic, 0.0f);
            TeethController.setTeethLoose(false);

            leftCP.stopMovement();
            rightCP.stopMovement();
        }

        private void resetTeethButton_Click(object sender, EventArgs e)
        {
            MuscleController.changeForce(RightDigastricDynamic, 0.0f);
            MuscleController.changeForce(RightLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightMasseterDynamic, 0.0f);
            MuscleController.changeForce(RightMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightTemporalisDynamic, 0.0f);

            MuscleController.changeForce(LeftDigastricDynamic, 0.0f);
            MuscleController.changeForce(LeftLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftMasseterDynamic, 0.0f);
            MuscleController.changeForce(LeftMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftTemporalisDynamic, 0.0f);
            TeethController.setTeethLoose(true);
        }

        private void translateForward_Click(object sender, EventArgs e)
        {
            MuscleController.changeForce(RightDigastricDynamic, 0.0f);
            MuscleController.changeForce(RightLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightMasseterDynamic, 3.0f);
            MuscleController.changeForce(RightMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightTemporalisDynamic, 3.0f);

            MuscleController.changeForce(LeftDigastricDynamic, 0.0f);
            MuscleController.changeForce(LeftLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftMasseterDynamic, 3.0f);
            MuscleController.changeForce(LeftMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftTemporalisDynamic, 3.0f);
            TeethController.setTeethLoose(false);

            leftCP.moveToLocation(1.0f, CP_MOVE_SPEED);
            rightCP.moveToLocation(1.0f, CP_MOVE_SPEED);
        }

        private void translateBackward_Click(object sender, EventArgs e)
        {
            MuscleController.changeForce(RightDigastricDynamic, 0.0f);
            MuscleController.changeForce(RightLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightMasseterDynamic, 3.0f);
            MuscleController.changeForce(RightMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(RightTemporalisDynamic, 3.0f);

            MuscleController.changeForce(LeftDigastricDynamic, 0.0f);
            MuscleController.changeForce(LeftLateralPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftMasseterDynamic, 3.0f);
            MuscleController.changeForce(LeftMedialPterygoidDynamic, 0.0f);
            MuscleController.changeForce(LeftTemporalisDynamic, 3.0f);
            TeethController.setTeethLoose(false);

            leftCP.moveToLocation(leftCP.getNeutralLocation(), CP_MOVE_SPEED);
            rightCP.moveToLocation(rightCP.getNeutralLocation(), CP_MOVE_SPEED);
        }
    }
}
