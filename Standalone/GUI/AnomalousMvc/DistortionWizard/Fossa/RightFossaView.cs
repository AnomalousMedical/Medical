using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;

namespace Medical.GUI.AnomalousMvc
{
    class RightFossaView: WizardView
    {
        public RightFossaView(String name)
            :base(name)
        {

        }

        public override ViewHostComponent createViewHost(AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            return new FossaGUI("RightFossa", "Medical.GUI.AnomalousMvc.DistortionWizard.Fossa.FossaGUIRight.layout", this, context, viewHost);
        }

        protected RightFossaView(LoadInfo info)
            :base(info)
        {

        }
    }
}
