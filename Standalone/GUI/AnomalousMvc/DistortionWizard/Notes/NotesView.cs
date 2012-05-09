using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class NotesView : WizardView
    {
        public NotesView(String name)
            : base(name)
        {
            AttachToScrollView = false;
        }

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
