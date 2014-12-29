using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class PresetStateView : WizardView
    {
        public PresetStateView(String name)
            :base(name)
        {
            
        }

        public override ViewHostComponent createViewHost(AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            return new PresetStateGUI(this, context, viewHost);
        }

        [Editable]
        public String PresetDirectory { get; set; }

        protected PresetStateView(LoadInfo info)
            :base(info)
        {

        }
    }
}
