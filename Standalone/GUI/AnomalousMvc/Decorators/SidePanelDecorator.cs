using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    class SidePanelDecorator : Component, ViewHostComponent
    {
        private ViewHostComponent child;
        private int widgetHeight;

        public SidePanelDecorator(ViewHostComponent child, ButtonCollection buttons)
            :base("Medical.GUI.AnomalousMvc.Decorators.SidePanelDecorator.layout")
        {
            if (buttons.Count > 0)
            {
                child = new ButtonDecorator(child, buttons);
            }

            this.child = child;
            child.Widget.attachToWidget(widget);
            child.Widget.setPosition(int.Parse(widget.getUserString("ChildX")), int.Parse(widget.getUserString("ChildY")));
            widget.setSize(child.Widget.Right + int.Parse(widget.getUserString("ChildWidthOffset")), child.Widget.Bottom + int.Parse(widget.getUserString("ChildHeightOffset")));
            child.Widget.Align = Align.Left | Align.VStretch;

            widgetHeight = widget.Height;
        }

        public override void Dispose()
        {
            child.Dispose();
            base.Dispose();
        }

        public void topLevelResized()
        {
            if (widget.Height != widgetHeight)
            {
                child.topLevelResized();
                widgetHeight = widget.Height;
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
