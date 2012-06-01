using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Editing;
using Engine.Saving;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    public abstract class ButtonDefinitionBase : SaveableEditableItem
    {
        public ButtonDefinitionBase(String name)
            :base(name)
        {
            
        }

        public ButtonDefinitionBase(String name, String action)
            : base(name)
        {
            Action = action;
        }

        public abstract void createButton(ButtonFactory factory, int x, int y, int width, int height);

        [EditableAction]
        public String Action { get; set; }

        public abstract bool FixedSize { get; }

        protected ButtonDefinitionBase(LoadInfo info)
            : base(info)
        {

        }
    }
}
