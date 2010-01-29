using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using System.Drawing;
using Medical.Properties;
using Medical.GUI;
using Engine.Saving.XMLSaver;

namespace Medical
{
    public class CTStatePicker : DistortionWizard
    {
        private StatePickerWizard statePicker;

        public CTStatePicker(StatePickerPanelController panelController)
        {
            statePicker = new StatePickerWizard(Name, panelController.UiHost, panelController.StateBlender, panelController.NavigationController, panelController.LayerController);
            statePicker.StateCreated += statePicker_StateCreated;
            statePicker.Finished += statePicker_Finished;

            statePicker.addStatePanel(panelController.getPanel(WizardPanels.LeftCondylarGrowth));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.LeftCondylarDegeneration));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.LeftFossa));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.LeftDiscSpacePanel));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.RightCondylarGrowth));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.RightCondylarDegeneration));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.RightFossa));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.RightDiscSpacePanel));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.TopTeethRemovalPanel));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.TeethAdaptationPanel));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.NotesPanel));

            statePicker.setToDefault();
        }

        public override void Dispose()
        {
            if (statePicker != null)
            {
                statePicker.Dispose();
            }
        }

        public override void sceneChanged(SimScene scene, String rootDirectory)
        {
            if (statePicker.Visible)
            {
                statePicker.closeForSceneChange();
            }
        }

        public override void setToDefault()
        {
            statePicker.setToDefault();
        }

        public override void startWizard(DrawingWindow window)
        {
            statePicker.startWizard(window);
            statePicker.show();
        }

        public override string Name
        {
            get 
            {
                return "CT Wizard";
            }
        }

        public override string TextLine1
        {
            get
            {
                return "CT Wizard";
            }
        }

        public override string TextLine2
        {
            get 
            {
                return null;
            }
        }

        public override Image ImageLarge
        {
            get
            {
                return Resources.CTWizardLarge;
            }
        }

        void statePicker_StateCreated(MedicalState state)
        {
            alertStateCreated(state);
        }

        void statePicker_Finished()
        {
            alertWizardFinished();
        }
    }
}
