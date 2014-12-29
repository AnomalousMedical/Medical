using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;

namespace Medical.GUI.AnomalousMvc
{
    class RightCondylarGrowthView : WizardView
    {
        public RightCondylarGrowthView(String name)
            : base(name)
        {

        }

        public override ViewHostComponent createViewHost(AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            return new RightCondylarGrowthGUI(this, context, viewHost);
        }

        protected RightCondylarGrowthView(LoadInfo info)
            :base(info)
        {

        }
    }
}
