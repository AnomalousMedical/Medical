using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;

namespace Medical.GUI.AnomalousMvc
{
    class RemoveBottomTeethView : WizardView
    {
        public RemoveBottomTeethView(String name)
            :base(name)
        {

        }

        public override ViewHostComponent createViewHost(AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            return new RemoveTeethGUI("Medical.GUI.AnomalousMvc.DistortionWizard.Teeth.RemoveBottomTeethGUI.layout", this, context, viewHost);
        }

        protected RemoveBottomTeethView(LoadInfo info)
            :base(info)
        {

        }
    }
}
