using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class LeftFossaView : WizardView
    {
        public LeftFossaView(String name)
            :base(name)
        {

        }

        public override ViewHostComponent createViewHost(AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            return new FossaGUI("LeftFossa", "Medical.GUI.AnomalousMvc.DistortionWizard.Fossa.FossaGUILeft.layout", this, context, viewHost);
        }

        protected LeftFossaView(LoadInfo info)
            :base(info)
        {

        }
    }
}
