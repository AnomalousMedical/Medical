using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{

    /// <summary>
    /// Creates a border layout. The top and bottom will fill the width and
    /// height of the screen and left, right and center will be sandwiched in
    /// between.
    /// </summary>
    public class BorderLayoutContainer : ScreenLayoutContainer
    {
        private ScreenLayoutContainer left;
        private ScreenLayoutContainer right;
        private ScreenLayoutContainer top;
        private ScreenLayoutContainer bottom;
        private ScreenLayoutContainer center;

        private bool visible = true;

        public override void setAlpha(float alpha)
        {
            //Top
            if (top != null)
            {
                top.setAlpha(alpha);
            }

            //Bottom
            if (bottom != null)
            {
                bottom.setAlpha(alpha);
            }

            //Left
            if (left != null)
            {
                left.setAlpha(alpha);
            }

            //Center
            if (center != null)
            {
                center.setAlpha(alpha);
            }

            //Right
            if (right != null)
            {
                right.setAlpha(alpha);
            }
        }

        public override void layout()
        {
            Size leftDesired = left != null ? left.DesiredSize : new Size();
            Size rightDesired = right != null ? right.DesiredSize : new Size();
            Size topDesired = top != null ? top.DesiredSize : new Size();
            Size bottomDesired = bottom != null ? bottom.DesiredSize : new Size();

            //Determine center region size.
            Size centerSize = new Size(WorkingSize.Width - leftDesired.Width - rightDesired.Width, WorkingSize.Height - topDesired.Height - bottomDesired.Height);
            
            //Top
            if (top != null)
            {
                top.Location = this.Location;
                top.WorkingSize = new Size(WorkingSize.Width, topDesired.Height);
                top.layout();
            }

            //Bottom
            if(bottom != null)
            {
                bottom.Location = new Vector2(this.Location.x, this.Location.y + topDesired.Height + centerSize.Height);
                bottom.WorkingSize = new Size(WorkingSize.Width, bottomDesired.Height);
                bottom.layout();
            }

            //Left
            if(left != null)
            {
                left.Location = new Vector2(this.Location.x, this.Location.y + topDesired.Height);
                left.WorkingSize = new Size(leftDesired.Width, centerSize.Height);
                left.layout();
            }

            //Center
            if(center != null)
            {
                center.Location = new Vector2(this.Location.x + leftDesired.Width, this.Location.y + topDesired.Height);
                center.WorkingSize = centerSize;
                center.layout();
            }

            //Right
            if(right != null)
            {
                right.Location = new Vector2(this.Location.x + leftDesired.Width + centerSize.Width, this.Location.y + topDesired.Height);
                right.WorkingSize = new Size(rightDesired.Width, centerSize.Height);
                right.layout();
            }
        }

        public override Size DesiredSize
        {
            get
            {
                Size desiredSize = new Size();
                Size leftDesired = left != null ? left.DesiredSize : new Size();
                Size rightDesired = right != null ? right.DesiredSize : new Size();
                Size topDesired = top != null ? top.DesiredSize : new Size();
                Size bottomDesired = bottom != null ? bottom.DesiredSize : new Size();
                if(leftDesired.Height > rightDesired.Height)
                {
                    desiredSize.Height = leftDesired.Height + topDesired.Height + bottomDesired.Height;
                }
                else
                {
                    desiredSize.Height = rightDesired.Height + topDesired.Height + bottomDesired.Height;
                }
                if(topDesired.Width > bottomDesired.Width)
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

        public ScreenLayoutContainer Left
        {
            get
            {
                return left;
            }
            set
            {
                if (left != null)
                {
                    left._setParent(null);
                }
                left = value;
                if (left != null)
                {
                    left._setParent(this);
                }
                invalidate();
            }
        }

        public ScreenLayoutContainer Right
        {
            get
            {
                return right;
            }
            set
            {
                if (right != null)
                {
                    right._setParent(null);
                }
                right = value;
                if (right != null)
                {
                    right._setParent(this);
                }
                invalidate();
            }
        }

        public ScreenLayoutContainer Top
        {
            get
            {
                return top;
            }
            set
            {
                if (top != null)
                {
                    top._setParent(null);
                }
                top = value;
                if (top != null)
                {
                    top._setParent(this);
                }
                invalidate();
            }
        }

        public ScreenLayoutContainer Bottom
        {
            get
            {
                return bottom;
            }
            set
            {
                if (bottom != null)
                {
                    bottom._setParent(null);
                }
                bottom = value;
                if (bottom != null)
                {
                    bottom._setParent(this);
                }
                invalidate();
            }
        }

        public ScreenLayoutContainer Center
        {
            get
            {
                return center;
            }
            set
            {
                if (center != null)
                {
                    center._setParent(null);
                }
                center = value;
                if (center != null)
                {
                    center._setParent(this);
                }
                invalidate();
            }
        }

        public override void bringToFront()
        {
            if (center != null)
            {
                center.bringToFront();
            }
            if (left != null)
            {
                left.bringToFront();
            }
            if (right != null)
            {
                right.bringToFront();
            }
            if (top != null)
            {
                top.bringToFront();
            }
            if (bottom != null)
            {
                bottom.bringToFront();
            }
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
                if (center != null)
                {
                    center.Visible = value;
                }
                if (left != null)
                {
                    left.Visible = value;
                }
                if (right != null)
                {
                    right.Visible = value;
                }
                if (top != null)
                {
                    top.Visible = value;
                }
                if (bottom != null)
                {
                    bottom.Visible = value;
                }
            }
        }
    }
}
