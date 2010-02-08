using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using System.Drawing;
using Medical.GUI;

namespace Medical
{
    public class DistortionWizard : IDisposable
    {
        private DistortionController controller;

        private StatePickerWizard statePicker;
        private string name;
        private string textLine1;
        private string textLine2;
        private Image imageLarge;
        private StatePickerPanelController panelController;

        public DistortionWizard(String name, String group, StatePickerPanelController panelController)
        {
            this.name = name;
            this.panelController = panelController;
            this.Group = group;
            statePicker = new StatePickerWizard(name, panelController.UiHost, panelController.StateBlender, panelController.NavigationController, panelController.LayerController);
            statePicker.StateCreated += statePicker_StateCreated;
            statePicker.Finished += statePicker_Finished;
        }

        public void Dispose()
        {
            if (statePicker != null)
            {
                statePicker.Dispose();
            }
        }

        public void addStatePanel(StatePickerPanel panel)
        {
            statePicker.addStatePanel(panel);
        }

        public void sceneChanged(SimScene scene, String rootDirectory)
        {
            if (statePicker.Visible)
            {
                statePicker.closeForSceneChange();
            }
        }

        public void setToDefault()
        {
            statePicker.setToDefault();
        }

        public void startWizard(DrawingWindow window)
        {
            statePicker.startWizard(window);
            statePicker.show();
        }

        public String Name
        {
            get
            {
                return name;
            }
        }

        public String TextLine1
        {
            get
            {
                return textLine1;
            }
            set
            {
                textLine1 = value;
            }
        }

        public String TextLine2
        {
            get
            {
                return textLine2;
            }
            set
            {
                textLine2 = value;
            }
        }

        public Image ImageLarge
        {
            get
            {
                return imageLarge;
            }
            set
            {
                imageLarge = value;
            }
        }

        public String Group { get; set; }

        void statePicker_StateCreated(MedicalState state)
        {
            //This is a bit of a hack, but apply the notes to the state here.
            panelController.applyNotes(state);
            alertStateCreated(state);
        }

        void statePicker_Finished()
        {
            alertWizardFinished();
        }

        internal void setController(DistortionController controller)
        {
            this.controller = controller;
        }

        protected void alertStateCreated(MedicalState state)
        {
            controller.stateCreated(state);
        }

        protected void alertWizardFinished()
        {
            controller.wizardFinished(this);
        }
    }
}
