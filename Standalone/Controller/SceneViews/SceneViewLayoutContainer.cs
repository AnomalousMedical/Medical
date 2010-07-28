using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.Controller
{
    class SceneViewLayoutContainer : LayoutContainer
    {
        public enum LayoutType
        {
            Horizontal,
            Vertical
        }

        private float alpha = 1.0f;
        private bool visible = true;
        private LayoutType layoutType;
        private float padding;

        private List<LayoutContainer> children = new List<LayoutContainer>();

        public SceneViewLayoutContainer(LayoutType layoutType, float padding)
        {
            this.layoutType = layoutType;
            this.padding = padding;
        }

        public void addChild(LayoutContainer child)
        {
            child.SuppressLayout = true;
            children.Add(child);
            child.Visible = visible;
            child.setAlpha(alpha);
            child._setParent(this);
            child.SuppressLayout = false;
            invalidate();
        }

        public void removeChild(LayoutContainer child)
        {
            children.Remove(child);
            invalidate();
        }

        public void clearChildren()
        {
            children.Clear();
        }

        public override void bringToFront()
        {
            foreach (LayoutContainer child in children)
            {
                child.bringToFront();
            }
        }

        public override void setAlpha(float alpha)
        {
            this.alpha = alpha;
            foreach (LayoutContainer child in children)
            {
                child.setAlpha(alpha);
            }
        }

        public override void layout()
        {
            Vector2 currentLocation = Location;
            if (layoutType == LayoutType.Horizontal)
            {
                //Get the total size all child controls want to be
                float totalWidth = 0.0f;
                foreach (LayoutContainer child in children)
                {
                     totalWidth += child.DesiredSize.Width + padding;
                }
                foreach (LayoutContainer child in children)
                {
                    Size2 childSize = child.DesiredSize;
                    Size2 acutalSize = new Size2(WorkingSize.Width * (childSize.Width / totalWidth), WorkingSize.Height);
                    child.WorkingSize = acutalSize;
                    child.Location = currentLocation;
                    child.layout();
                    currentLocation.x += acutalSize.Width + padding;
                }
            }
            else
            {
                foreach (LayoutContainer child in children)
                {
                    Size2 childSize = child.DesiredSize;
                    child.WorkingSize = childSize;
                    child.Location = currentLocation;
                    currentLocation.y += childSize.Height + padding;
                }
            }
        }

        public override Size2 DesiredSize
        {
            get 
            {
                Size2 desiredSize = new Size2();
                if (layoutType == LayoutType.Horizontal)
                {
                    foreach (LayoutContainer child in children)
                    {
                        Size2 childSize = child.DesiredSize;
                        if (childSize.Height > desiredSize.Height)
                        {
                            desiredSize.Height = childSize.Height;
                        }
                        desiredSize.Width += childSize.Width + padding;
                    }
                }
                else
                {
                    foreach (LayoutContainer child in children)
                    {
                        Size2 childSize = child.DesiredSize;
                        if (childSize.Width > desiredSize.Width)
                        {
                            desiredSize.Width = childSize.Width;
                        }
                        desiredSize.Height += childSize.Height + padding;
                    }
                }
                return desiredSize;
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
                foreach (LayoutContainer child in children)
                {
                    child.Visible = visible;
                }
            }
        }

        public float Padding
        {
            get
            {
                return padding;
            }
            set
            {
                padding = value;
                invalidate();
            }
        }
    }
}
