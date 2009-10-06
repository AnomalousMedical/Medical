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
        }

        private void adaptButton_CheckedChanged(object sender, EventArgs e)
        {
            if (adaptButton.Checked)
            {
                TeethController.adaptAllTeeth(true);
            }
            else
            {
                TeethController.adaptAllTeeth(false);
            }
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

        internal override string NavigationImageKey
        {
            get
            {
                return "__AdaptationIcon";
            }
        }

        protected override void onPanelClosing()
        {
            if (adaptButton.Checked)
            {
                adaptButton.Checked = false;
            }
        }

        protected override void statePickerSet(StatePickerController controller)
        {
            parentPicker.ImageList.Images.Add(NavigationImageKey, Resources.AdaptationIcon);
        }
    }
}
