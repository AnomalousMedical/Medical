using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine;
using Engine.Editing;
using Medical.Editor;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class DopplerView : WizardView
    {
        public DopplerView(String name)
            :base(name)
        {
            
        }

        public override ViewHostComponent createViewHost(AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            return new DopplerGUI(this, context, viewHost);
        }

        [Editable]
        public String PresetDirectory { get; set; }

        [EditableAction]
        public String LateralJointAction { get; set; }

        [EditableAction]
        public String SuperiorJointAction { get; set; }

        [EditableAction]
        public String BothJointsAction { get; set; }

        protected DopplerView(LoadInfo info)
            :base(info)
        {

        }
    }
}
