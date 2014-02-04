using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;

namespace Medical
{
    public class VariableSizeMyGUILayoutContainer : LayoutContainer
    {
        private Widget widget;

        public event Action LayoutChanged;
        public event Action<IntSize2> AnimatedResizeStarted;
        public event Action<IntSize2> AnimatedResizeCompleted;
        Func<IntSize2> desiredSizeCallback;

        public VariableSizeMyGUILayoutContainer(Widget widget, Func<IntSize2> desiredSizeCallback)
        {
            this.widget = widget;
            this.desiredSizeCallback = desiredSizeCallback;
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
                return desiredSizeCallback();
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

        public override void animatedResizeStarted(IntSize2 finalSize)
        {
            base.animatedResizeStarted(finalSize);
            if (AnimatedResizeStarted != null)
            {
                AnimatedResizeStarted.Invoke(finalSize);
            }
        }

        public override void animatedResizeCompleted(IntSize2 finalSize)
        {
            base.animatedResizeCompleted(finalSize);
            if (AnimatedResizeCompleted != null)
            {
                AnimatedResizeCompleted.Invoke(finalSize);
            }
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
