﻿using System;
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
            
        }

        public override ViewHostComponent createViewHost(AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            return new WizardComponent<WizardView>("Medical.GUI.DistortionWizard.Disclaimer.DisclaimerGUI.layout", this, context, viewHost);
        }

        protected DisclaimerView(LoadInfo info)
            :base(info)
        {

        }
    }
}
