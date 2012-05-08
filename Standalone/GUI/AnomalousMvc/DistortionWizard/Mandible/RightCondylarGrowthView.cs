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

        public override ViewHost createViewHost(AnomalousMvcContext context)
        {
            return new RightCondylarGrowthGUI(this, context);
        }

        protected RightCondylarGrowthView(LoadInfo info)
            :base(info)
        {

        }
    }
}
