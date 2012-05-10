using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    class MyGUIWindowViewHost : Dialog, ViewHost
    {
        protected MyGUILayoutContainer layoutContainer;
        private ViewHostComponent child;

        public MyGUIWindowViewHost(ViewHostComponent child)
            :base("Medical.GUI.AnomalousMvc.Decorators.MyGUIWindowViewHost.layout")
        {
            this.child = child;
            child.Widget.attachToWidget(window);
            IntCoord clientCoord = window.ClientCoord;
            child.Widget.setCoord(0, 0, clientCoord.width, clientCoord.height);
            child.Widget.Align = Align.Stretch;
            layoutContainer = new MyGUILayoutContainer(window);

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

        public LayoutContainer Container
        {
            get
            {
                return layoutContainer;
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
