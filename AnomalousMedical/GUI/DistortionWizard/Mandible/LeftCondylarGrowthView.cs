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

        public override ViewHostComponent createViewHost(AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            return new LeftCondylarGrowthGUI(this, context, viewHost);
        }

        protected LeftCondylarGrowthView(LoadInfo info)
            :base(info)
        {

        }
    }
}
