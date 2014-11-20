using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    /// <summary>
    /// This is very similar to flow layout, but it will stretch all the children along the secondary axis.
    /// </summary>
    public class StretchLayoutContainer : LayoutContainer
    {
        public enum LayoutType
        {
            Horizontal,
            Vertical
        }

        private float alpha = 1.0f;
        private bool visible = true;
        private LayoutType layoutType;
        private int padding;
        private IntVector2 startLocation;

        private List<LayoutContainer> children = new List<LayoutContainer>();

        public StretchLayoutContainer(LayoutType layoutType, int padding, IntVector2 startLocation)
        {
            this.layoutType = layoutType;
            this.padding = padding;
            this.startLocation = startLocation;
        }

        public void addChild(LayoutContainer child)
        {
            child.SuppressLayout = true;
            children.Add(child);
            child._setParent(this);
            child.Visible = visible;
            child.setAlpha(alpha);
            child.SuppressLayout = false;
            invalidate();
        }

        public void insertChild(LayoutContainer child, int index)
        {
            child.SuppressLayout = true;
            children.Insert(index, child);
            child._setParent(this);
            child.Visible = visible;
            child.setAlpha(alpha);
            child.SuppressLayout = false;
            invalidate();
        }

        public void removeChild(LayoutContainer child)
        {
            if (children.Remove(child))
            {
                invalidate();
                child._setParent(null);
            }
        }

        public void clearChildren()
        {
            foreach (LayoutContainer child in children)
            {
                child._setParent(null);
            }
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
            IntVector2 currentLocation = startLocation + Location;
            if (layoutType == LayoutType.Horizontal)
            {
                foreach (LayoutContainer child in children)
                {
                    IntSize2 childSize = child.DesiredSize;
                    childSize.Height = WorkingSize.Height;
                    child.WorkingSize = childSize;
                    child.Location = currentLocation;
                    child.layout();
                    currentLocation.x += childSize.Width + padding;
                }
            }
            else
            {
                foreach (LayoutContainer child in children)
                {
                    IntSize2 childSize = child.DesiredSize;
                    childSize.Width = WorkingSize.Width;
                    child.WorkingSize = childSize;
                    child.Location = currentLocation;
                    child.layout();
                    currentLocation.y += childSize.Height + padding;
                }
            }
        }

        public override IntSize2 DesiredSize
        {
            get 
            {
                IntSize2 desiredSize = new IntSize2(startLocation.x, startLocation.y);
                if (layoutType == LayoutType.Horizontal)
                {
                    foreach (LayoutContainer child in children)
                    {
                        IntSize2 childSize = child.DesiredSize;
                        if (childSize.Height > desiredSize.Height)
                        {
                            desiredSize.Height = childSize.Height;
                        }
                        desiredSize.Width += childSize.Width + padding;
                    }

                    if (children.Count > 0)
                    {
                        desiredSize.Width -= padding;
                    }
                }
                else
                {
                    foreach (LayoutContainer child in children)
                    {
                        IntSize2 childSize = child.DesiredSize;
                        if (childSize.Width > desiredSize.Width)
                        {
                            desiredSize.Width = childSize.Width;
                        }
                        desiredSize.Height += childSize.Height + padding;
                    }

                    if (children.Count > 0)
                    {
                        desiredSize.Height -= padding;
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

        public int Padding
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

        public IntVector2 StartLocation
        {
            get
            {
                return startLocation;
            }
            set
            {
                startLocation = value;
                invalidate();
            }
        }
    }
}
