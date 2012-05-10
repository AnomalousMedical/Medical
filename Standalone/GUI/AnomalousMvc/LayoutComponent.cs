using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    class LayoutComponent : ViewHostComponent
    {
        private Layout layout;
        protected Widget widget;

        public LayoutComponent(String layoutFile, MyGUIViewHost viewHost)
            :base(viewHost)
        {
            layout = LayoutManager.Instance.loadLayout(layoutFile);
            widget = layout.getWidget(0);

            //Hmmmm
            Widget = widget;
        }

        public override void Dispose()
        {
            LayoutManager.Instance.unloadLayout(layout);
            base.Dispose();
        }
    }
}
