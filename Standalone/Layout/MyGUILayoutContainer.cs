using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;

namespace Medical
{
    public class MyGUILayoutContainer : ScreenLayoutContainer
    {
        private Widget widget;

        public MyGUILayoutContainer(Widget widget)
        {
            this.widget = widget;
        }

        public override void setAlpha(float alpha)
        {
            widget.Alpha = alpha;
        }

        public override void layout()
        {
            widget.setCoord((int)Location.x, (int)Location.y, (int)WorkingSize.Width, (int)WorkingSize.Height);
        }

        public override Size DesiredSize
        {
            get
            {
                return new Size(widget.getWidth(), widget.getHeight());
            }
        }

        public Widget Widget
        {
            get
            {
                return widget;
            }
            set
            {
                widget = value;
                invalidate();
            }
        }
    }
}
