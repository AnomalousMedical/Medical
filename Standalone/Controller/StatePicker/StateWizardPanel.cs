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
        private BasicGUI basicGUI;
        private Button button;

        public StateWizardPanel(String file, Button button, BasicGUI basicGUI)
        {
            this.button = button;
            this.basicGUI = basicGUI;
            button.MouseButtonClick += button_MouseButtonClick;
            layout = LayoutManager.Instance.loadLayout(file);
            mainWidget = layout.getWidget(0);
            mainWidget.Visible = false;
            layoutContainer = new MyGUILayoutContainer(mainWidget);
        }

        public void Dispose()
        {
            button.MouseButtonClick -= button_MouseButtonClick;
            LayoutManager.Instance.unloadLayout(layout);
            layout = null;
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {
            if (mainWidget.Visible)
            {
                basicGUI.changeLeftPanel(null);
            }
            else
            {
                basicGUI.changeLeftPanel(layoutContainer);
            }
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
