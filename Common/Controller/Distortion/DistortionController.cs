using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using Medical.GUI;

namespace Medical
{
    public class DistortionController
    {
        private Dictionary<String, DistortionWizard> wizards = new Dictionary<string, DistortionWizard>();
        public event MedicalStateCreated StateCreated;
        public event StatePickerFinished Finished;

        public DistortionController()
        {
            
        }

        public void addDistortionWizard(DistortionWizard wizard)
        {
            wizards.Add(wizard.Name, wizard);
            wizard.setController(this);
        }

        public void setToDefault()
        {
            foreach (DistortionWizard wizard in wizards.Values)
            {
                wizard.setToDefault();
            }
        }

        public void updateStatePicker(String presetDirectory)
        {
            foreach (DistortionWizard wizard in wizards.Values)
            {
                wizard.updateStatePicker(presetDirectory);
            }
        }

        public void startWizard(String name, DrawingWindow displayWindow)
        {
            wizards[name].startWizard(displayWindow);
        }

        public bool Visible { get; set; }

        internal void stateCreated(MedicalState state)
        {
            if (StateCreated != null)
            {
                StateCreated.Invoke(state);
            }
        }

        internal void wizardFinished(DistortionWizard wizard)
        {
            if (Finished != null)
            {
                Finished.Invoke();
            }
        }
    }
}
