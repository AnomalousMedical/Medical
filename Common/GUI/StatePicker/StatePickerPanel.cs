using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;

namespace Medical.GUI
{
    public partial class StatePickerPanel : UserControl
    {
        protected StatePickerWizard parentPicker;

        public StatePickerPanel()
        {
            InitializeComponent();
        }

        public void setStatePicker(StatePickerWizard parentPicker)
        {
            this.parentPicker = parentPicker;
            statePickerSet(parentPicker);
        }

        public virtual void applyToState(MedicalState state)
        { 
        
        }

        public virtual void setToDefault()
        {

        }

        public virtual void recordOpeningState()
        {

        }

        public virtual void resetToOpeningState()
        {

        }

        /// <summary>
        /// Set the state of the muscles and other scene elements as appropriate
        /// for this panel's editing. By default this will reset the muscles as
        /// they are when the scene starts.
        /// </summary>
        public virtual void modifyScene()
        {
            ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
            ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
            MuscleBehavior movingMuscle = MuscleController.getMuscle("MovingMuscleDynamic");
            MovingMuscleTarget movingMuscleTarget = MuscleController.MovingTarget;

            leftCP.setLocation(leftCP.getNeutralLocation());
            rightCP.setLocation(rightCP.getNeutralLocation());
            movingMuscle.changeForce(6.0f);
            movingMuscleTarget.Offset = Vector3.Zero;
        }

        public String NavigationState { get; set; }

        public String LayerState { get; set; }

        internal virtual String NavigationImageKey
        {
            get
            {
                return "";
            }
        }

        protected void showChanges(bool immediate)
        {
            parentPicker.showChanges(immediate);
        }

        protected virtual void statePickerSet(StatePickerWizard controller)
        {

        }

        internal void callPanelOpening()
        {
            onPanelOpening();
        }

        protected virtual void onPanelOpening()
        {

        }

        internal void callPanelClosing()
        {
            onPanelClosing();
        }

        protected virtual void onPanelClosing()
        {

        }
    }
}
