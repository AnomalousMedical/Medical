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

        public override Button createButton(Widget parentWidget, int x, int y, int width, int height)
        {
            Button button = (Button)parentWidget.createWidgetT("Button", "Button", x, y, width, height, Align.Default, "");
            button.Caption = Text;
            return button;
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
