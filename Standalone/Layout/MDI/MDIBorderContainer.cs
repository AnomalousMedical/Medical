using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.Controller
{

    /// <summary>
    /// Creates a border layout. The top and bottom will fill the width and
    /// height of the screen and left, right and center will be sandwiched in
    /// between.
    /// </summary>
    public class MDIBorderContainer : MDIContainerBase, IDisposable
    {
        private MDILayoutContainer left;
        private MDILayoutContainer right;
        private MDILayoutContainer top;
        private MDILayoutContainer bottom;
        private MDILayoutContainer center;

        private bool visible = true;

        public MDIBorderContainer(int padding)
        {
            left = new MDILayoutContainer(MDILayoutContainer.LayoutType.Vertical, padding);
            left._setParent(this);

            right = new MDILayoutContainer(MDILayoutContainer.LayoutType.Vertical, padding);
            right._setParent(this);
            
            top = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, padding);
            top._setParent(this);
            
            bottom = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, padding);
            bottom._setParent(this);
            
            center = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, padding);
            center._setParent(this);
        }

        public void Dispose()
        {
            left.Dispose();
            right.Dispose();
            top.Dispose();
            bottom.Dispose();
            center.Dispose();
        }

        public void addChild(MDIWindow child)
        {
            switch (child.CurrentDockLocation)
            {
                case DockLocation.Left:
                    left.addChild(child);
                    break;
                case DockLocation.Right:
                    right.addChild(child);
                    break;
                case DockLocation.Top:
                    top.addChild(child);
                    break;
                case DockLocation.Bottom:
                    bottom.addChild(child);
                    break;
                case DockLocation.Center:
                    center.addChild(child);
                    break;
            }
        }

        public override MDIWindow findWindowAtPosition(float mouseX, float mouseY)
        {
            MDIWindow retVal = null;
            retVal = findWindowPositionImpl(mouseX, mouseY, center);
            if (retVal != null)
            {
                return retVal;
            } 
            retVal = findWindowPositionImpl(mouseX, mouseY, left);
            if (retVal != null)
            {
                return retVal;
            }
            retVal = findWindowPositionImpl(mouseX, mouseY, right);
            if (retVal != null)
            {
                return retVal;
            }
            retVal = findWindowPositionImpl(mouseX, mouseY, top);
            if (retVal != null)
            {
                return retVal;
            }
            retVal = findWindowPositionImpl(mouseX, mouseY, bottom);
            if (retVal != null)
            {
                return retVal;
            }
            return null;
        }

        private MDIWindow findWindowPositionImpl(float mouseX, float mouseY, MDILayoutContainer container)
        {
            float left = container.Location.x;
            float top = container.Location.y;
            float right = left + container.WorkingSize.Width;
            float bottom = top + container.WorkingSize.Height;
            if (mouseX > left && mouseX < right &&
                mouseY > top && mouseY < bottom)
            {
                return container.findWindowAtPosition(mouseX, mouseY);
            }
            return null;
        }

        public override void setAlpha(float alpha)
        {
            top.setAlpha(alpha);
            bottom.setAlpha(alpha);
            left.setAlpha(alpha);
            center.setAlpha(alpha);
            right.setAlpha(alpha);
        }

        public override void layout()
        {
            Size2 leftDesired = left != null ? left.DesiredSize : new Size2();
            Size2 rightDesired = right != null ? right.DesiredSize : new Size2();
            Size2 topDesired = top != null ? top.DesiredSize : new Size2();
            Size2 bottomDesired = bottom != null ? bottom.DesiredSize : new Size2();

            //Determine center region size.
            Size2 centerSize = new Size2(WorkingSize.Width - leftDesired.Width - rightDesired.Width, WorkingSize.Height - topDesired.Height - bottomDesired.Height);

            top.Location = this.Location;
            top.WorkingSize = new Size2(WorkingSize.Width, topDesired.Height);
            top.layout();
            
            bottom.Location = new Vector2(this.Location.x, this.Location.y + topDesired.Height + centerSize.Height);
            bottom.WorkingSize = new Size2(WorkingSize.Width, bottomDesired.Height);
            bottom.layout();
            
            left.Location = new Vector2(this.Location.x, this.Location.y + topDesired.Height);
            left.WorkingSize = new Size2(leftDesired.Width, centerSize.Height);
            left.layout();
            
            center.Location = new Vector2(this.Location.x + leftDesired.Width, this.Location.y + topDesired.Height);
            center.WorkingSize = centerSize;
            center.layout();
            
            right.Location = new Vector2(this.Location.x + leftDesired.Width + centerSize.Width, this.Location.y + topDesired.Height);
            right.WorkingSize = new Size2(rightDesired.Width, centerSize.Height);
            right.layout();
        }

        public override Size2 DesiredSize
        {
            get
            {
                Size2 desiredSize = new Size2();
                Size2 leftDesired = left.DesiredSize;
                Size2 rightDesired = right.DesiredSize;
                Size2 topDesired = top.DesiredSize;
                Size2 bottomDesired = bottom.DesiredSize;
                if (leftDesired.Height > rightDesired.Height)
                {
                    desiredSize.Height = leftDesired.Height + topDesired.Height + bottomDesired.Height;
                }
                else
                {
                    desiredSize.Height = rightDesired.Height + topDesired.Height + bottomDesired.Height;
                }
                if (topDesired.Width > bottomDesired.Width)
                {
                    desiredSize.Width = topDesired.Width;
                }
                else
                {
                    desiredSize.Width = bottomDesired.Width;
                }
                return desiredSize;
            }
        }

        public override void bringToFront()
        {
            center.bringToFront();
            left.bringToFront();
            right.bringToFront();
            top.bringToFront();
            bottom.bringToFront();
        }

        public override bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
                center.Visible = value;
                left.Visible = value;
                right.Visible = value;
                top.Visible = value;
                bottom.Visible = value;
            }
        }
    }
}
