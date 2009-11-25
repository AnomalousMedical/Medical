using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medical.GUI;
using Engine.ObjectManagement;

namespace Medical
{
    public class DistortionController
    {
        private Dictionary<String, DistortionWizard> wizards = new Dictionary<string, DistortionWizard>();
        public event MedicalStateCreated StateCreated;
        public event StatePickerFinished Finished;

        public DistortionController()
        {
            Visible = false;
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

        public void sceneChanged(SimScene scene, String presetDirectory)
        {
            foreach (DistortionWizard wizard in wizards.Values)
            {
                wizard.sceneChanged(scene, presetDirectory);
            }
        }

        public void startWizard(String name, DrawingWindow displayWindow)
        {
            wizards[name].startWizard(displayWindow);
            Visible = true;
        }

        public bool Visible { get; set; }

        public IEnumerable<DistortionWizard> Wizards
        {
            get
            {
                return wizards.Values;
            }
        }

        internal void stateCreated(MedicalState state)
        {
            if (StateCreated != null)
            {
                StateCreated.Invoke(state);
            }
        }

        internal void wizardFinished(DistortionWizard wizard)
        {
            Visible = false;
            if (Finished != null)
            {
                Finished.Invoke();
            }
        }
    }
}
