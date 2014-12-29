using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;
using Engine.Editing;
using Medical.Editor;

namespace Medical.GUI.AnomalousMvc
{
    abstract class WizardView : MyGUIView
    {
        public WizardView(String name)
            :base(name)
        {

        }

        public abstract ViewHostComponent createViewHost(AnomalousMvcContext context, MyGUIViewHost viewHost);

        protected WizardView(LoadInfo info)
            :base (info)
        {

        }
    }
}
