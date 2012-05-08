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
    abstract class WizardView : View
    {
        public WizardView(String name)
            :base(name)
        {
            AttachToScrollView = true;
            CancelAction = "Wizard/Cancel";
            FinishAction = "Wizard/Finish";
        }

        [Editable]
        public bool AttachToScrollView { get; set; }

        [EditableAction]
        public String CancelAction { get; set; }

        [EditableAction]
        public String FinishAction { get; set; }

        public abstract ViewHost createViewHost(AnomalousMvcContext context);

        protected WizardView(LoadInfo info)
            :base (info)
        {

        }
    }
}
