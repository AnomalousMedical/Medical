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
        private IntSize2 desiredSize;

        public event Action LayoutChanged;

        public MyGUILayoutContainer(Widget widget)
        {
            this.widget = widget;
            this.desiredSize = new IntSize2(widget.Width, widget.Height);
        }

        /// <summary>
        /// Change the desired size and invalidate.
        /// </summary>
        /// <param name="desiredSize">The new desired size.</param>
        public void changeDesiredSize(IntSize2 desiredSize)
        {
            this.desiredSize = desiredSize;
            invalidate();
        }

        public override void setAlpha(float alpha)
        {
            widget.Alpha = alpha;
        }

        public override void layout()
        {
            widget.setCoord(Location.x, Location.y, WorkingSize.Width, WorkingSize.Height);
            if (LayoutChanged != null)
            {
                LayoutChanged.Invoke();
            }
        }

        public override IntSize2 DesiredSize
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
