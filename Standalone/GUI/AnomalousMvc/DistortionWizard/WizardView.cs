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
            AttachToScrollView = true;
            CancelAction = "Wizard/Cancel";
            FinishAction = "Wizard/Finish";
            NextAction = "Wizard/Next";
            PreviousAction = "Wizard/Previous";
        }

        [Editable]
        public bool AttachToScrollView { get; set; }

        [EditableAction]
        public String CancelAction { get; set; }

        [EditableAction]
        public String FinishAction { get; set; }

        [EditableAction]
        public String NextAction { get; set; }

        [EditableAction]
        public String PreviousAction { get; set; }

        public abstract ViewHostComponent createViewHost(AnomalousMvcContext context, MyGUIViewHost viewHost);

        protected WizardView(LoadInfo info)
            :base (info)
        {

        }
    }
}
