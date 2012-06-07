using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    public class LayoutComponent : ViewHostComponent
    {
        private Layout layout;
        protected Widget widget;

        public LayoutComponent(String layoutFile, MyGUIViewHost viewHost)
        {
            layout = LayoutManager.Instance.loadLayout(layoutFile);
            widget = layout.getWidget(0);
            this.viewHost = viewHost;
        }

        public virtual void Dispose()
        {
            LayoutManager.Instance.unloadLayout(layout);
        }

        public virtual void topLevelResized()
        {
            
        }

        public virtual void opening()
        {
            
        }

        public virtual void closing()
        {
            
        }

        public Widget Widget
        {
            get
            {
                return widget;
            }
        }

        private MyGUIViewHost viewHost;
        public MyGUIViewHost ViewHost
        {
            get
            {
                return viewHost;
            }
        }
    }
}
