using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class StateWizardPanel : IDisposable
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

        public void Dispose()
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

        internal void applyToState(MedicalState createdState)
        {
            
        }

        internal void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            
        }

        internal void setToDefault()
        {
            
        }

        internal void recordOpeningState()
        {
            
        }

        internal void resetToOpeningState()
        {
            
        }

        internal void callPanelOpening()
        {
            
        }

        internal void modifyScene()
        {
            
        }

        internal void callPanelClosing()
        {
            
        }
    }
}
