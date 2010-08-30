using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;

namespace Medical
{
    public class MyGUILayoutContainer : LayoutContainer
    {
        private Widget widget;
        private Size2 desiredSize;

        public MyGUILayoutContainer(Widget widget)
        {
            this.widget = widget;
            this.desiredSize = new Size2(widget.Width, widget.Height);
        }

        public override void setAlpha(float alpha)
        {
            widget.Alpha = alpha;
        }

        public override void layout()
        {
            widget.setCoord((int)Location.x, (int)Location.y, (int)WorkingSize.Width, (int)WorkingSize.Height);
        }

        public override Size2 DesiredSize
        {
            get
            {
                return desiredSize;
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

        public override void bringToFront()
        {
            LayerManager.Instance.upLayerItem(widget);
        }

        public override bool Visible
        {
            get
            {
                return widget.Visible;
            }
            set
            {
                widget.Visible = value;
            }
        }
    }
}
