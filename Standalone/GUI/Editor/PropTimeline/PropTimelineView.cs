using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Medical.Controller;

namespace Medical.GUI
{
    public class PropTimelineView : MyGUIView
    {
        public PropTimelineView(String name, PropEditController propEditController)
            : base(name)
        {
            ElementName = new MDILayoutElementName(GUILocationNames.MDI, DockLocation.Top)
            {
                AllowedDockLocations = DockLocation.Top | DockLocation.Bottom | DockLocation.Floating
            };
            this.PropEditController = propEditController;
        }

        public PropEditController PropEditController { get; set; }

        public PropTimelineView(LoadInfo info)
            :base(info)
        {

        }
    }
}
