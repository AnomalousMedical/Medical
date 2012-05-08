using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class DisclaimerView : WizardView
    {
        public DisclaimerView(String name)
            :base(name)
        {
            AttachToScrollView = false;
        }

        public override ViewHost createViewHost(AnomalousMvcContext context)
        {
            return new WizardPanel("Medical.GUI.AnomalousMvc.DistortionWizard.Disclaimer.DisclaimerGUI.layout", this, context);
        }

        protected DisclaimerView(LoadInfo info)
            :base(info)
        {

        }
    }
}
