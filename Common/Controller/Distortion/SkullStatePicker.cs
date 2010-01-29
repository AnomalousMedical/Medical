#define USE_SLIDER_GUIS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.Resources;
using System.Xml;
using Logging;
using Engine.Saving.XMLSaver;
using Medical.GUI;
using Engine.ObjectManagement;
using Medical.Properties;
using System.Drawing;

namespace Medical
{
    public class SkullStatePicker : DistortionWizard
    {
        private StatePickerWizard statePicker;

        public SkullStatePicker(StatePickerPanelController panelController)
        {
            statePicker = new StatePickerWizard(Name, panelController.UiHost, panelController.StateBlender, panelController.NavigationController, panelController.LayerController);
            statePicker.StateCreated += statePicker_StateCreated;
            statePicker.Finished += statePicker_Finished;

            statePicker.addStatePanel(panelController.getPanel(WizardPanels.LeftCondylarGrowth));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.LeftCondylarDegeneration));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.LeftDiscClockFacePanel));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.LeftFossa));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.RightCondylarGrowth));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.RightCondylarDegeneration));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.RightDiscClockFacePanel));
            statePicker.addStatePanel(panelController.getPanel(WizardPanels.RightFossa));
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

        public override String Name
        {
            get
            {
                return "MRI Wizard";
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

        public override String TextLine1
        {
            get
            {
                return "MRI Wizard";
            }
        }

        public override String TextLine2
        {
            get
            {
                return "";
            }
        }

        public override Image ImageLarge
        {
            get
            {
                return Resources.MRIWizardLarge;
            }
        }
    }
}
