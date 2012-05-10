using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    class WindowDecorator : Dialog, ViewHostComponent
    {
        private ViewHostComponent child;

        public WindowDecorator(ViewHostComponent child)
            : base("Medical.GUI.AnomalousMvc.Decorators.WindowDecorator.layout")
        {
            this.child = child;
            child.Widget.attachToWidget(window);
            IntCoord clientCoord = window.ClientCoord;
            child.Widget.setCoord(0, 0, clientCoord.width, clientCoord.height);
            child.Widget.Align = Align.Stretch;

            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);
            child.topLevelResized();
        }

        public override void Dispose()
        {
            child.Dispose();
            base.Dispose();
        }

        public void opening()
        {
            this.open(false);
            child.opening();
        }

        public void closing()
        {
            child.closing();
            this.close();
        }

        public void topLevelResized()
        {
            child.topLevelResized();
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
                return window;
            }
        }

        public bool _RequestClosed { get; set; }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            child.topLevelResized();
        }

        public void _animationCallback(LayoutContainer oldChild)
        {
            Dispose();
        }
    }
}
