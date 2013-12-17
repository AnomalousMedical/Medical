using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class BorderLayoutElementName : LayoutElementName
    {
        public BorderLayoutElementName(String name, BorderLayoutLocations viewLocation)
            :base(name)
        {
            this.viewLocation = viewLocation;
        }

        private BorderLayoutLocations viewLocation;
        public BorderLayoutLocations ViewLocation
        {
            get
            {
                return viewLocation;
            }
        }

        protected override string UniqueDerivedName
        {
            get
            {
                return Name + ViewLocation;
            }
        }

        public override ViewLocations LocationHint
        {
            get
            {
                switch (viewLocation)
                {
                    case BorderLayoutLocations.Left:
                        return ViewLocations.Left;
                    case BorderLayoutLocations.Right:
                        return ViewLocations.Right;
                    case BorderLayoutLocations.Top:
                        return ViewLocations.Top;
                    case BorderLayoutLocations.Bottom:
                        return ViewLocations.Bottom;
                }
                return base.LocationHint;
            }
        }

        protected BorderLayoutElementName(LoadInfo info)
            :base(info)
        {

        }
    }
}
