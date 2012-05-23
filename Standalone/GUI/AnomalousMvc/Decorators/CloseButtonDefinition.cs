using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Saving;

namespace Medical.GUI.AnomalousMvc
{
    class CloseButtonDefinition : ButtonDefinitionBase
    {
        public CloseButtonDefinition(String name)
            :base(name)
        {
            
        }

        public CloseButtonDefinition(String name, String action)
            : base(name, action)
        {
            
        }

        public override Button createButton(Widget parentWidget, int x, int y, int width, int height)
        {
            return (Button)parentWidget.createWidgetT("Button", "ButtonXBig", x, y + 4, 24, 20, Align.Default, "");
        }

        public override bool FixedSize
        {
            get
            {
                return true;
            }
        }

        protected CloseButtonDefinition(LoadInfo info)
            : base(info)
        {

        }
    }
}

