﻿using System;
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

        protected void setNavigationState(String name)
        {
            parentPicker.setNavigationState(name);
        }

        protected void setLayerState(String name)
        {
            parentPicker.setLayerState(name);
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

        public String TextLine1 { get; set; }

        public String TextLine2 { get; set; }

        public Image LargeIcon { get; set; }

        protected void showChanges(bool immediate)
        {
            parentPicker.showChanges(immediate, false);
        }

        protected void showChanges(bool immediate, bool captureCurrentState)
        {
            parentPicker.showChanges(immediate, captureCurrentState);
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
