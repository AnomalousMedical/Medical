using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Engine;

namespace Medical.Controller
{
    class MDILayoutContainerScale : MDILayoutContainer
    {
        public MDILayoutContainerScale(LayoutType layoutType, int padding, DockLocation dockLocation)
            :base(layoutType, padding, dockLocation)
        {
            setSeparatorWidgetManager(new SeparatorWidgetManagerScale(this));
        }

        /// <summary>
        /// LayoutContainer function
        /// </summary>
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
                    IntSize2 actualSize = new IntSize2((int)(sizeWithoutPadding * (child.Scale / totalScale)), WorkingSize.Height);
                    if (i == childCount) //Make sure to stretch the last child out completely, sometimes there is an extra pixel. This stops unsightly flickering.
                    {
                        actualSize.Width = WorkingSize.Width + Location.x - currentLocation.x;
                    }
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
                    IntSize2 actualSize = new IntSize2(WorkingSize.Width, (int)(sizeWithoutPadding * (child.Scale / totalScale)));
                    if (i == childCount) //Make sure to stretch the last child out completely, sometimes there is an extra pixel. This stops unsightly flickering.
                    {
                        actualSize.Height = WorkingSize.Height + Location.y - currentLocation.y;
                    }
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
