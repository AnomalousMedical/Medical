using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;
using Medical.Editor;

namespace Medical.GUI.AnomalousMvc
{
    class ProfileDistortionView : WizardView
    {
        public ProfileDistortionView(String name)
            :base(name)
        {

        }

        public override ViewHost createViewHost(AnomalousMvcContext context)
        {
            return new ProfileDistortionGUI(this, context);
        }

        [EditableAction]
        public string UndoAction { get; set; }

        [EditableAction]
        public string LeftSideAction { get; set; }

        [EditableAction]
        public string LeftMidAction { get; set; }

        [EditableAction]
        public string MidlineAction { get; set; }

        [EditableAction]
        public string RightMidAction { get; set; }

        [EditableAction]
        public string RightSideAction { get; set; }

        protected ProfileDistortionView(LoadInfo info)
            :base(info)
        {

        }
    }
}
