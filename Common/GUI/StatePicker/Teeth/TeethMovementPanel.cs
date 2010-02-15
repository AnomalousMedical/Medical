using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;

namespace Medical.GUI.StatePicker.Teeth
{
    public partial class TeethMovementPanel : UserControl
    {
        private static String[] RIGHT_LATERAL_TEETH = 
        {
            "Tooth01", "Tooth02", "Tooth03", "Tooth04", "Tooth05", "Tooth06", 
            "Tooth32", "Tooth31", "Tooth30", "Tooth29", "Tooth28", "Tooth27"
        };

        private static String[] MIDLINE_ANTERIOR_TEETH = 
        {
            "Tooth05", "Tooth06", "Tooth07", "Tooth08", "Tooth09", "Tooth10", 
            "Tooth11", "Tooth12",
            "Tooth28", "Tooth27", "Tooth26", "Tooth25", "Tooth24", "Tooth23", 
            "Tooth22", "Tooth21"
        };

        private static String[] LEFT_LATERAL_TEETH = 
        {
            "Tooth16", "Tooth15", "Tooth14", "Tooth13", "Tooth12", "Tooth11",
            "Tooth17", "Tooth18", "Tooth19", "Tooth20", "Tooth21", "Tooth22"
        };

        private StatePickerPanelController panelController;

        public TeethMovementPanel()
        {
            InitializeComponent();
            adaptButton.CheckedChanged += new EventHandler(adaptButton_CheckedChanged);
            moveButton.CheckedChanged += new EventHandler(moveButton_CheckedChanged);
            rotateButton.CheckedChanged += new EventHandler(rotateButton_CheckedChanged);
        }

        public void initialize(StatePickerPanelController panelController)
        {
            this.panelController = panelController;
        }

        public void disableAllButtons()
        {
            adaptButton.Checked = false;
            moveButton.Checked = false;
            rotateButton.Checked = false;
        }

        public void setDefaultTools()
        {
            TeethController.showTeethTools(MIDLINE_ANTERIOR_TEETH);
            TeethController.TeethMover.setActivePlanes(MovementAxis.X | MovementAxis.Y, MovementPlane.XY);
        }

        private void topCameraButton_Click(object sender, EventArgs e)
        {
            TeethController.showTeethTools(true, false);
            this.setLayerState("TopTeethLayers");
            this.setNavigationState("WizardTopTeeth");
            TeethController.TeethMover.setActivePlanes(MovementAxis.X | MovementAxis.Z, MovementPlane.XZ);
        }

        private void bottomCameraButton_Click(object sender, EventArgs e)
        {
            TeethController.showTeethTools(false, true);
            this.setLayerState("BottomTeethLayers");
            this.setNavigationState("WizardBottomTeeth");
            TeethController.TeethMover.setActivePlanes(MovementAxis.X | MovementAxis.Z, MovementPlane.XZ);
        }

        private void leftLateralCameraButton_Click(object sender, EventArgs e)
        {
            TeethController.showTeethTools(LEFT_LATERAL_TEETH);
            this.setLayerState("TeethLayers");
            this.setNavigationState("WizardTeethLeftLateral");
            TeethController.TeethMover.setActivePlanes(MovementAxis.Y | MovementAxis.Z, MovementPlane.YZ);
        }

        private void midlineAnteriorCameraButton_Click(object sender, EventArgs e)
        {
            TeethController.showTeethTools(MIDLINE_ANTERIOR_TEETH);
            this.setLayerState("TeethLayers");
            this.setNavigationState("WizardTeethMidlineAnterior");
            TeethController.TeethMover.setActivePlanes(MovementAxis.X | MovementAxis.Y, MovementPlane.XY);
        }

        private void rightLateralCameraButton_Click(object sender, EventArgs e)
        {
            TeethController.showTeethTools(RIGHT_LATERAL_TEETH);
            this.setLayerState("TeethLayers");
            this.setNavigationState("WizardTeethRightLateral");
            TeethController.TeethMover.setActivePlanes(MovementAxis.Y | MovementAxis.Z, MovementPlane.YZ);
        }

        void adaptButton_CheckedChanged(object sender, EventArgs e)
        {
            TeethController.adaptAllTeeth(adaptButton.Checked);
            if (adaptButton.Checked)
            {
                ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
                ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
                MuscleBehavior movingMuscle = MuscleController.getMuscle("MovingMuscleDynamic");
                MovingMuscleTarget movingMuscleTarget = MuscleController.MovingTarget;

                leftCP.setLocation(leftCP.getNeutralLocation());
                rightCP.setLocation(rightCP.getNeutralLocation());
                movingMuscle.changeForce(1.0f);
                movingMuscleTarget.Offset = Vector3.Zero;

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

        void setLayerState(String layerState)
        {
            panelController.setLayerState(layerState);
        }

        void setNavigationState(String navigationState)
        {
            panelController.setNavigationState(navigationState);
        }
    }
}
