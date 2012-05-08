using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    abstract class MyGUIViewHost : ViewHost
    {
        private Layout layout;
        protected Widget widget;
        protected MyGUILayoutContainer layoutContainer;

        public MyGUIViewHost(String layoutFile)
        {
            layout = LayoutManager.Instance.loadLayout(layoutFile);
            widget = layout.getWidget(0);
            layoutContainer = new MyGUILayoutContainer(widget);
        }

        public virtual void Dispose()
        {
            if (layout != null)
            {
                LayoutManager.Instance.unloadLayout(layout);
            }
        }

        public virtual void opening()
        {

        }

        public virtual void closing()
        {

        }

        public LayoutContainer Container
        {
            get
            {
                return layoutContainer;
            }
        }


        public bool _RequestClosed { get; set; }

        public void _animationCallback(LayoutContainer oldChild)
        {
            Dispose();
        }
    }
}
