using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public class StateWizardPanel : IDisposable
    {
        private Layout layout;
        private MyGUILayoutContainer layoutContainer;
        protected Widget mainWidget;
        protected StateWizardPanelController controller;

        public StateWizardPanel(String file, StateWizardPanelController controller)
        {
            this.controller = controller;
            layout = LayoutManager.Instance.loadLayout(file);
            mainWidget = layout.getWidget(0);
            mainWidget.Visible = false;
            layoutContainer = new MyGUILayoutContainer(mainWidget);
        }

        public virtual void Dispose()
        {
            LayoutManager.Instance.unloadLayout(layout);
            layout = null;
        }

        public MyGUILayoutContainer LayoutContainer
        {
            get
            {
                return layoutContainer;
            }
        }

        protected void setNavigationState(String name)
        {
            controller.setNavigationState(name);
        }

        protected void setLayerState(String name)
        {
            controller.setLayerState(name);
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

        public virtual void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {

        }

        public String NavigationState { get; set; }

        public String LayerState { get; set; }

        public String TextLine1 { get; set; }

        public String TextLine2 { get; set; }

        public String ImageKey { get; set; }

        protected void showChanges(bool immediate)
        {
            controller.showChanges(this, immediate);
        }

        internal void callPanelOpening()
        {
            if (NavigationState != null)
            {
                setNavigationState(NavigationState);
            }
            if (LayerState != null)
            {
                setLayerState(LayerState);
            }
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
