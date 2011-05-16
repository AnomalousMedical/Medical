using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;

namespace Medical.GUI
{
    public enum WizardType
    {
        Anatomy,
        Exam
    }

    public class StateWizard : IDisposable
    {
        private List<StateWizardPanel> panels = new List<StateWizardPanel>();
        private bool updatePanel = true;
        private StateWizardController wizardController;

        public StateWizard(String name, StateWizardController wizardController, WizardType wizardType)
        {
            this.Name = name;
            this.wizardController = wizardController;
            this.WizardType = wizardType;
        }

        public void Dispose()
        {
            
        }

        public void addStatePanel(StateWizardPanel panel)
        {
            panels.Add(panel);
        }

        public void startWizard()
        {
            foreach (StateWizardPanel panel in panels)
            {
                wizardController.addMode(panel);
                panel.recordOpeningState();
            }
        }

        public void setToDefault()
        {
            foreach (StateWizardPanel panel in panels)
            {
                panel.setToDefault();
            }
        }

        public void resetPanels()
        {
            foreach (StateWizardPanel panel in panels)
            {
                panel.resetToOpeningState();
            }
        }

        public void applyToState(MedicalState state)
        {
            foreach (StateWizardPanel panel in panels)
            {
                panel.applyToState(state);
            }
        }

        public void showPanel(int index)
        {
            if (updatePanel)
            {
                updatePanel = false;
                StateWizardPanel panel = panels[index];
                panel.callPanelOpening();
                wizardController.showPanel(panel);
                panel.modifyScene();
                updatePanel = true;
            }
        }

        public void hidePanel(int index)
        {
            panels[index].callPanelClosing();
            wizardController.hidePanel(panels[index]);
        }

        public String Name { get; private set; }

        public String TextLine1 { get; set; }

        public String TextLine2 { get; set; }

        public String ImageKey { get; set; }

        public WizardType WizardType { get; set; }
    }
}
