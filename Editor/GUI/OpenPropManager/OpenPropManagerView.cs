using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;

namespace Medical.GUI
{
    class OpenPropManagerView : MyGUIView
    {
        public OpenPropManagerView(String name)
            : base(name)
        {
            IsWindow = true;
            ViewLocation = ViewLocations.Floating;
        }

        public OpenPropManagerView(LoadInfo info)
            :base(info)
        {

        }
    }
}
