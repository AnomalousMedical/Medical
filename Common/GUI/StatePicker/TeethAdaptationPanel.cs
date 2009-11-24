using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;
using Medical.Properties;

namespace Medical.GUI
{
    public partial class TeethAdaptationPanel : StatePickerPanel
    {
        public TeethAdaptationPanel()
        {
            InitializeComponent();
            this.Text = "Teeth Adaptation";
            adaptButton.CheckedChanged += new EventHandler(adaptButton_CheckedChanged);
            moveButton.CheckedChanged += new EventHandler(moveButton_CheckedChanged);
            rotateButton.CheckedChanged += new EventHandler(rotateButton_CheckedChanged);
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            TeethState undo = parentPicker.StateBlender.UndoState.Teeth;
            foreach (ToothState toothState in undo.StateEnum)
            {
                Tooth tooth = TeethController.getTooth(toothState.Name);
                tooth.Offset = toothState.Offset;
                tooth.Rotation = toothState.Rotation;
            }
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            TeethController.setAllOffsets(Vector3.Zero);
            TeethController.setAllRotations(Quaternion.Identity);
        }

        public override void applyToState(MedicalState state)
        {
            foreach(ToothState toothState in state.Teeth.StateEnum)
            {
                Tooth tooth = TeethController.getTooth(toothState.Name);
                toothState.Offset = tooth.Offset;
                toothState.Rotation = tooth.Rotation;
            }
        }

        public override void modifyScene()
        {
            ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
            ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
            MuscleBehavior movingMuscle = MuscleController.getMuscle("MovingMuscleDynamic");
            MovingMuscleTarget movingMuscleTarget = MuscleController.MovingTarget;

            leftCP.setLocation(leftCP.getNeutralLocation());
            rightCP.setLocation(rightCP.getNeutralLocation());
            movingMuscle.changeForce(1.0f);
            movingMuscleTarget.Offset = Vector3.Zero;
        }

        protected override void onPanelOpening()
        {
            TeethController.showTeethTools(true, true);
        }

        protected override void onPanelClosing()
        {
            if (adaptButton.Checked)
            {
                adaptButton.Checked = false;
            }
            TeethController.showTeethTools(false, false);
        }

        void adaptButton_CheckedChanged(object sender, EventArgs e)
        {
            TeethController.adaptAllTeeth(adaptButton.Checked);
            if (adaptButton.Checked)
            {
                moveButton.Checked = false;
                rotateButton.Checked = false;
            }
        }

        void rotateButton_CheckedChanged(object sender, EventArgs e)
        {
            TeethController.TeethMover.ShowRotateTools = rotateButton.Checked;
            if (rotateButton.Checked)
            {
                moveButton.Checked = false;
                adaptButton.Checked = false;
            }
        }

        void moveButton_CheckedChanged(object sender, EventArgs e)
        {
            TeethController.TeethMover.ShowMoveTools = moveButton.Checked;
            if (moveButton.Checked)
            {
                rotateButton.Checked = false;
                adaptButton.Checked = false;
            }
        }

        private void topCameraButton_Click(object sender, EventArgs e)
        {
            TeethController.showTeethTools(true, false);
            this.setLayerState("TopTeethLayers");
            this.setNavigationState("Top Teeth");
        }

        private void bottomCameraButton_Click(object sender, EventArgs e)
        {
            TeethController.showTeethTools(false, true);
            this.setLayerState("BottomTeethLayers");
            this.setNavigationState("Bottom Teeth");
            
        }

        private void leftLateralCameraButton_Click(object sender, EventArgs e)
        {
            TeethController.showTeethTools(true, true);
            this.setLayerState("TeethLayers");
            this.setNavigationState("Teeth Left Lateral");
        }

        private void midlineAnteriorCameraButton_Click(object sender, EventArgs e)
        {
            TeethController.showTeethTools(true, true);
            this.setLayerState("TeethLayers");
            this.setNavigationState("Teeth Midline Anterior");
        }

        private void rightLateralCameraButton_Click(object sender, EventArgs e)
        {
            TeethController.showTeethTools(true, true);
            this.setLayerState("TeethLayers");
            this.setNavigationState("Teeth Right Lateral");
        }

        private void leftMidLateralCameraButton_Click(object sender, EventArgs e)
        {
            TeethController.showTeethTools(true, true);
            this.setLayerState("TeethLayers");
            this.setNavigationState("Teeth Left Mid Anterior");
        }

        private void rightMidLateralCameraButton_Click(object sender, EventArgs e)
        {
            TeethController.showTeethTools(true, true);
            this.setLayerState("TeethLayers");
            this.setNavigationState("Teeth Right Mid Anterior");
        }
    }
}
