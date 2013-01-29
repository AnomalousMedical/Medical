using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;

namespace Medical.GUI
{
    public class OpenPropManagerView : MyGUIView
    {
        public OpenPropManagerView(String name, PropEditController propEditController)
            : base(name)
        {
            IsWindow = true;
            ViewLocation = ViewLocations.Floating;
            this.PropEditController = propEditController;
        }

        public PropEditController PropEditController { get; set; }

        public OpenPropManagerView(LoadInfo info)
            :base(info)
        {

        }
    }
}
