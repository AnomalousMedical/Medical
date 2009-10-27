using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public abstract class DistortionWizard : IDisposable
    {
        private DistortionController controller;

        public abstract void Dispose();

        public abstract void setToDefault();

        public abstract void updateStatePicker(String presetDirectory);

        public abstract void startWizard(DrawingWindow displayWindow);

        public abstract String Name { get; }

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
