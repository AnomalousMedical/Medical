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

        /// <summary>
        /// Do not call this constructor, it exists to make the designer work
        /// </summary>
        protected TeethAdaptationPanel()
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                throw new NotImplementedException();
            }
            InitializeComponent();
        }

        public TeethAdaptationPanel(StatePickerPanelController panelController)
            : base(panelController)
        {
            InitializeComponent();
            this.Text = "Teeth Adaptation";
            adaptButton.CheckedChanged += new EventHandler(adaptButton_CheckedChanged);
            moveButton.CheckedChanged += new EventHandler(moveButton_CheckedChanged);
            rotateButton.CheckedChanged += new EventHandler(rotateButton_CheckedChanged);
            gridPropertiesControl1.setGrid(panelController.MeasurementGrid);
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            TeethState undo = panelController.StateBlender.UndoState.Teeth;
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
            TeethController.showTeethTools(MIDLINE_ANTERIOR_TEETH);
            gridPropertiesControl1.updateGrid();
        }

        protected override void onPanelClosing()
        {
            if (adaptButton.Checked)
            {
                adaptButton.Checked = false;
            }
            TeethController.showTeethTools(false, false);
            panelController.MeasurementGrid.Visible = false;
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
            this.setNavigationState("WizardTopTeeth");
        }

        private void bottomCameraButton_Click(object sender, EventArgs e)
        {
            TeethController.showTeethTools(false, true);
            this.setLayerState("BottomTeethLayers");
            this.setNavigationState("WizardBottomTeeth");
            
        }

        private void leftLateralCameraButton_Click(object sender, EventArgs e)
        {
            TeethController.showTeethTools(LEFT_LATERAL_TEETH);
            this.setLayerState("TeethLayers");
            this.setNavigationState("WizardTeethLeftLateral");
        }

        private void midlineAnteriorCameraButton_Click(object sender, EventArgs e)
        {
            TeethController.showTeethTools(MIDLINE_ANTERIOR_TEETH);
            this.setLayerState("TeethLayers");
            this.setNavigationState("WizardTeethMidlineAnterior");
        }

        private void rightLateralCameraButton_Click(object sender, EventArgs e)
        {
            TeethController.showTeethTools(RIGHT_LATERAL_TEETH);
            this.setLayerState("TeethLayers");
            this.setNavigationState("WizardTeethRightLateral");
        }
    }
}
