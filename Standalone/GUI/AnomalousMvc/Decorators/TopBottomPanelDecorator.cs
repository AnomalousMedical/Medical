using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    class TopBottomPanelDecorator : Component, ViewHostComponent
    {
        private ViewHostComponent child;
        private int widgetWidth;

        public TopBottomPanelDecorator(ViewHostComponent child)
            : base("Medical.GUI.AnomalousMvc.Decorators.TopBottomPanelDecorator.layout")
        {
            this.child = child;
            child.Widget.attachToWidget(widget);
            child.Widget.setCoord(int.Parse(widget.getUserString("ChildX")),
                                  int.Parse(widget.getUserString("ChildY")),
                                  widget.Width - int.Parse(widget.getUserString("ChildWidthOffset")),
                                  widget.Height - int.Parse(widget.getUserString("ChildHeightOffset")));
            child.Widget.Align = Align.Top | Align.HStretch;

            widgetWidth = widget.Width;
        }

        public override void Dispose()
        {
            child.Dispose();
            base.Dispose();
        }

        public void topLevelResized()
        {
            if (widget.Width != widgetWidth)
            {
                child.topLevelResized();
                widgetWidth = widget.Width;
            }
        }

        public void opening()
        {
            child.opening();
        }

        public void closing()
        {
            child.closing();
        }

        public MyGUIViewHost ViewHost
        {
            get
            {
                return child.ViewHost;
            }
        }

        public Widget Widget
        {
            get
            {
                return widget;
            }
        }
    }
}
