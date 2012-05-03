using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class MyGUIViewHostFactory : ViewHostFactory
    {
        public ViewHost createViewHost(View view, AnomalousMvcContext context)
        {
            if (view.GetType().IsAssignableFrom(typeof(RmlView)))
            {
                return new RmlWidgetViewHost((RmlView)view, context);
            }
            throw new Exception(String.Format("No ViewHost defined for {0}", view));
        }
    }
}
