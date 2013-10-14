using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.AnomalousMvc;
using Engine;

namespace Medical.GUI.AnomalousMvc
{
    class SidePanelDecorator : Component, ViewHostComponent
    {
        private ViewHostComponent child;
        private int widgetHeight;

        private IntVector2 childPosition;
        private IntSize2 childSizeOffset;

        public SidePanelDecorator(ViewHostComponent child, ButtonCollection buttons, bool transparent)
            : base(transparent ? "Medical.GUI.AnomalousMvc.Decorators.SidePanelDecoratorTransparent.layout" : "Medical.GUI.AnomalousMvc.Decorators.SidePanelDecorator.layout")
        {
            if (buttons.Count > 0)
            {
                child = new ButtonDecorator(child, buttons);
            }

            this.child = child;
            child.Widget.attachToWidget(widget);
            childPosition = new IntVector2(int.Parse(widget.getUserString("ChildX")), int.Parse(widget.getUserString("ChildY")));
            child.Widget.setPosition(childPosition.x, childPosition.y);
            childSizeOffset = new IntSize2(int.Parse(widget.getUserString("ChildWidthOffset")), int.Parse(widget.getUserString("ChildHeightOffset")));
            widget.setSize(child.Widget.Right + childSizeOffset.Width, child.Widget.Bottom + childSizeOffset.Height);
            child.Widget.Align = Align.HStretch | Align.VStretch;

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

        public void animatedResizeStarted(IntSize2 finalSize)
        {
            //This class eats this event, it sets up the child to be a fixed size at the final size
            //This prevents it from laying out the child a million times during animation.
            child.Widget.Align = Align.Left | Align.VStretch;
            child.Widget.setPosition(childPosition.x, childPosition.y);
            child.Widget.setSize(finalSize.Width - childSizeOffset.Width, finalSize.Height - childSizeOffset.Height);
            child.topLevelResized();
        }

        public void animatedResizeCompleted()
        {
            //This class eats this event, it sets up the child to be stretched again.
            child.Widget.Align = Align.HStretch | Align.VStretch;
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

        public ViewHostControl findControl(string name)
        {
            return child.findControl(name);
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
