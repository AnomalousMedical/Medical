using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    public class ButtonDefinition : SaveableEditableItem
    {
        public ButtonDefinition(String name)
            :base(name)
        {
            Text = name;
        }

        [Editable]
        public String Text { get; set; }

        [EditableAction]
        public String Action { get; set; }

        protected ButtonDefinition(LoadInfo info)
            : base(info)
        {

        }
    }
}
