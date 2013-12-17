using Engine.Saving;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class MDILayoutElementName : LayoutElementName
    {
        public MDILayoutElementName(String name, DockLocation location)
            :base(name)
        {
            this.dockLocation = location;
        }

        private DockLocation dockLocation;
        public DockLocation DockLocation
        {
            get
            {
                return dockLocation;
            }
        }

        public override ViewLocations LocationHint
        {
            get
            {
                switch (dockLocation)
                {
                    case DockLocation.Left:
                        return ViewLocations.Left;
                    case DockLocation.Right:
                        return ViewLocations.Right;
                    case DockLocation.Top:
                        return ViewLocations.Top;
                    case DockLocation.Bottom:
                        return ViewLocations.Bottom;
                    case DockLocation.Floating:
                        return ViewLocations.Floating;
                }
                return base.LocationHint;
            }
        }

        protected MDILayoutElementName(LoadInfo info)
            :base(info)
        {

        }
    }
}
