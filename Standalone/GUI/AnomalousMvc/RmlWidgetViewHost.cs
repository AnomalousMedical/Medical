using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class RmlWidgetViewHost : MyGUIViewHost
    {
        public RmlWidgetViewHost(RmlView view)
            :base("Medical.GUI.AnomalousMvc.RmlWidgetViewHost.layout")
        {

        }
    }
}
