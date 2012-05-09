using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;
using Medical.Editor;

namespace Medical.GUI.AnomalousMvc
{
    class TeethAdaptationView : WizardView
    {
        public TeethAdaptationView(String name)
            :base(name)
        {

        }

        public override ViewHost createViewHost(AnomalousMvcContext context)
        {
            return new TeethAdaptationGUI(this, context);
        }

        [EditableAction]
        public string UndoAction { get; set; }

        [EditableAction]
        public string TopButtonAction { get; set; }

        [EditableAction]
        public string BottomButtonAction { get; set; }

        [EditableAction]
        public string LeftLateralAction { get; set; }

        [EditableAction]
        public string MidlineAnteriorAction { get; set; }

        [EditableAction]
        public string RightLateralAction { get; set; }

        protected TeethAdaptationView(LoadInfo info)
            :base(info)
        {

        }
    }
}
