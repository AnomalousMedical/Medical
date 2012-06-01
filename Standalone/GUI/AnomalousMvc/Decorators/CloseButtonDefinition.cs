using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Saving;

namespace Medical.GUI.AnomalousMvc
{
    public class CloseButtonDefinition : ButtonDefinitionBase
    {
        public CloseButtonDefinition(String name)
            :base(name)
        {
            
        }

        public CloseButtonDefinition(String name, String action)
            : base(name, action)
        {
            
        }

        public override void createButton(ButtonFactory factory, int x, int y, int width, int height)
        {
            factory.addCloseButton(this, x, y, width, height);
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

