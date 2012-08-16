using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class TopBottomPanelDecorator : Component, ViewHostComponent
    {
        private ViewHostComponent child;
        private int widgetWidth;

        public TopBottomPanelDecorator(ViewHostComponent child, ButtonCollection buttons)
            : base("Medical.GUI.AnomalousMvc.Decorators.TopBottomPanelDecorator.layout")
        {
            if (buttons.Count > 0)
            {
                child = new ButtonDecorator(child, buttons);
            }

            this.child = child;
            child.Widget.attachToWidget(widget);
            child.Widget.setPosition(int.Parse(widget.getUserString("ChildX")), int.Parse(widget.getUserString("ChildY")));
            widget.setSize(child.Widget.Right + int.Parse(widget.getUserString("ChildWidthOffset")), child.Widget.Bottom + int.Parse(widget.getUserString("ChildHeightOffset")));
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

        public void populateViewData(IDataProvider dataProvider)
        {
            child.populateViewData(dataProvider);
        }

        public void analyzeViewData(IDataProvider dataProvider)
        {
            child.analyzeViewData(dataProvider);
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
