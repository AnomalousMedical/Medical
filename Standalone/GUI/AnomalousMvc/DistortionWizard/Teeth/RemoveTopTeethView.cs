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

        public override ViewHostComponent createViewHost(AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            return new RemoveTeethGUI("Medical.GUI.AnomalousMvc.DistortionWizard.Teeth.RemoveTopTeethGUI.layout", this, context, viewHost);
        }

        protected RemoveTopTeethView(LoadInfo info)
            :base(info)
        {

        }
    }
}
