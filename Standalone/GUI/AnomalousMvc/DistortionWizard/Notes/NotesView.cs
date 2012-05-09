using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    class NotesView : WizardView
    {
        public NotesView(String name)
            : base(name)
        {
            AttachToScrollView = false;
            WizardStateInfoName = "DefaultWizardStateInfo";
        }

        [Editable]
        public String WizardStateInfoName { get; set; }

        public override ViewHost createViewHost(AnomalousMvcContext context)
        {
            return new NotesGUI(this, context);
        }

        protected NotesView(LoadInfo info)
            :base(info)
        {

        }
    }
}
