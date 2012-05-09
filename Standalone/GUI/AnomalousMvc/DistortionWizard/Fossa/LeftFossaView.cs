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

        public override ViewHost createViewHost(AnomalousMvcContext context)
        {
            return new FossaGUI("LeftFossa", "Medical.GUI.AnomalousMvc.DistortionWizard.Fossa.FossaGUILeft.layout", this, context);
        }

        protected LeftFossaView(LoadInfo info)
            :base(info)
        {

        }
    }
}
