using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.Controller
{
    class MDILayoutContainerAbsolute : MDILayoutContainer
    {
        public MDILayoutContainerAbsolute(LayoutType layoutType, int padding, DockLocation dockLocation)
            :base(layoutType, padding, dockLocation)
        {
            setSeparatorWidgetManager(new SeparatorWidgetManagerAbsolute(this));
        }

        public override void layout()
        {
            IntVector2 currentLocation = Location;
            int i = 0;
            int childCount = children.Count - 1;
            if (Layout == LayoutType.Horizontal)
            {
                float sizeWithoutPadding = WorkingSize.Width - Padding * childCount;
                foreach (MDIContainerBase child in children)
                {
                    IntSize2 actualSize = child.DesiredSize;
                    actualSize.Height = WorkingSize.Height;
                    child.WorkingSize = actualSize;
                    child.Location = currentLocation;
                    child.layout();
                    currentLocation.x += actualSize.Width;
                    separatorWidgetManager.setSeparatorCoord(i++, (int)currentLocation.x, (int)currentLocation.y, Padding, (int)actualSize.Height);
                    currentLocation.x += Padding;
                }
            }
            else
            {
                float sizeWithoutPadding = WorkingSize.Height - Padding * childCount;
                foreach (MDIContainerBase child in children)
                {
                    IntSize2 actualSize = child.DesiredSize;
                    actualSize.Width = WorkingSize.Width;
                    child.WorkingSize = actualSize;
                    child.Location = currentLocation;
                    child.layout();
                    currentLocation.y += actualSize.Height;
                    separatorWidgetManager.setSeparatorCoord(i++, (int)currentLocation.x, (int)currentLocation.y, (int)actualSize.Width, Padding);
                    currentLocation.y += Padding;
                }
            }
        }
    }
}
