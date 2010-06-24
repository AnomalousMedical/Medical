using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class WizardPanel : IDisposable
    {
        private Layout layout;
        private MyGUILayoutContainer layoutContainer;
        protected Widget mainWidget;
        private BasicGUI basicGUI;
        private Button button;

        public WizardPanel(String file, Button button, BasicGUI basicGUI)
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

        internal void applyToState(MedicalState createdState)
        {
            throw new NotImplementedException();
        }

        internal void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            throw new NotImplementedException();
        }

        internal void setToDefault()
        {
            throw new NotImplementedException();
        }

        internal void recordOpeningState()
        {
            throw new NotImplementedException();
        }

        internal void resetToOpeningState()
        {
            throw new NotImplementedException();
        }

        internal void callPanelOpening()
        {
            throw new NotImplementedException();
        }

        internal void modifyScene()
        {
            throw new NotImplementedException();
        }

        internal void callPanelClosing()
        {
            throw new NotImplementedException();
        }
    }
}
