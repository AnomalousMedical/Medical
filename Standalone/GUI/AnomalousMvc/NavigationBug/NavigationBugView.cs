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
    class NavigationBugView : MyGUIView
    {
        public NavigationBugView(String name)
            : base(name)
        {
            NavigationModelName = NavigationModel.DefaultName;
        }

        [EditableModel(typeof(NavigationModel))]
        public String NavigationModelName { get; set; }

        [EditableAction]
        public String CloseButtonAction { get; set; }

        [EditableAction]
        public String PreviousButtonAction { get; set; }

        [EditableAction]
        public String NextButtonAction { get; set; }

        protected NavigationBugView(LoadInfo info)
            :base(info)
        {

        }
    }
}
