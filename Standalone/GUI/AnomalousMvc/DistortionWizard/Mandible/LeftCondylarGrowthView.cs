using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;

namespace Medical.GUI.AnomalousMvc
{
    class LeftCondylarGrowthView : WizardView
    {
        public LeftCondylarGrowthView(String name)
            : base(name)
        {

        }

        public override ViewHost createViewHost(AnomalousMvcContext context)
        {
            return new LeftCondylarGrowthGUI(this, context);
        }

        protected LeftCondylarGrowthView(LoadInfo info)
            :base(info)
        {

        }
    }
}
