using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.AnomalousMvc;

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

        public virtual void populateViewData(IDataProvider dataProvider)
        {

        }

        public virtual void analyzeViewData(IDataProvider dataProvider)
        {
            
        }

        public virtual ViewHostControl findControl(string name)
        {
            return null;
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
