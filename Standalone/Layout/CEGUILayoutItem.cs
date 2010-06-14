using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using CEGUIPlugin;

namespace Medical
{
    public class CEGUILayoutItem : ScreenLayoutContainer
    {
        private Window window;

        public CEGUILayoutItem(Window window)
        {
            this.window = window;
        }

        public override void layout()
        {
            window.setArea(new UVector2(0, Location.x, 0, Location.y), new UVector2(0, WorkingSize.Width, 0, WorkingSize.Height));
        }

        public override Size DesiredSize
        {
            get 
            {
                return window.getPixelSize();
            }
        }
    }
}
