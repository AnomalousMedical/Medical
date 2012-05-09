using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;

namespace Medical.GUI.AnomalousMvc
{
    class RemoveTopTeethView : WizardView
    {
        public RemoveTopTeethView(String name)
            :base(name)
        {

        }

        public override ViewHost createViewHost(AnomalousMvcContext context)
        {
            return new RemoveTeethGUI("Medical.GUI.AnomalousMvc.DistortionWizard.Teeth.RemoveTopTeethGUI.layout", this, context);
        }

        protected RemoveTopTeethView(LoadInfo info)
            :base(info)
        {

        }
    }
}
