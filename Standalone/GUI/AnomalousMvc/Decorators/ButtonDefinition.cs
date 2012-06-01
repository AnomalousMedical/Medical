using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;
using Engine.Editing;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    public class ButtonDefinition : ButtonDefinitionBase
    {
        public ButtonDefinition(String name)
            :this(name, null)
        {
            
        }

        public ButtonDefinition(String name, String action)
            : base(name, action)
        {
            Text = name;
        }

        public override void createButton(ButtonFactory factory, int x, int y, int width, int height)
        {
            factory.addTextButton(this, x, y, width, height);
        }

        [Editable]
        public String Text { get; set; }

        public override bool FixedSize
        {
            get
            {
                return false;
            }
        }

        protected ButtonDefinition(LoadInfo info)
            : base(info)
        {

        }
    }
}
