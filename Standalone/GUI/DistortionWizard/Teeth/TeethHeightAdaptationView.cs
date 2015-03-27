using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class TeethHeightAdaptationView : TeethAdaptationView
    {
        public TeethHeightAdaptationView(String name)
            : base(name)
        {

        }

        public override ViewHostComponent createViewHost(AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            return new TeethHeightAdaptationGUI(this, context, viewHost);
        }

        protected TeethHeightAdaptationView(LoadInfo info)
            : base(info)
        {

        }
    }
}
