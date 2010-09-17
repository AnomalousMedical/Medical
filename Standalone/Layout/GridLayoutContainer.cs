using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    class GridLayoutContainer : LayoutContainer
    {
        public event EventHandler GridLaidOut;

        public enum LayoutType
        {
            Horizontal,
            Vertical
        }

        private float alpha = 1.0f;
        private bool visible = true;
        private LayoutType layoutType;
        private float padding;
        private Vector2 startLocation;
        private Size2 gridSize = new Size2();

        private List<LayoutContainer> children = new List<LayoutContainer>();

        public GridLayoutContainer(LayoutType layoutType, float padding, Vector2 startLocation)
        {
            this.layoutType = layoutType;
            this.padding = padding;
            this.startLocation = startLocation;
        }

        public void addChild(LayoutContainer child)
        {
            child.SuppressLayout = true;
            children.Add(child);
            child.Visible = visible;
            child.setAlpha(alpha);
            child.SuppressLayout = false;
            invalidate();
        }

        public void insertChild(LayoutContainer child, int index)
        {
            child.SuppressLayout = true;
            children.Insert(index, child);
            child.Visible = visible;
            child.setAlpha(alpha);
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
            gridSize = WorkingSize;
            Vector2 currentLocation = startLocation;
            if (layoutType == LayoutType.Horizontal)
            {
                foreach (LayoutContainer child in children)
                {
                    Size2 childSize = child.DesiredSize;

                    if (currentLocation.x + childSize.Width > WorkingSize.Width)
                    {
                        currentLocation.y += childSize.Height + padding;
                        currentLocation.x = startLocation.x;
                    }

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
                    Size2 childSize = child.DesiredSize;

                    if (currentLocation.y + childSize.Height > WorkingSize.Height)
                    {
                        currentLocation.x += childSize.Width + padding;
                        currentLocation.y = startLocation.y;
                    }

                    child.WorkingSize = childSize;
                    child.Location = currentLocation;
                    child.layout();
                    currentLocation.y += childSize.Height + padding;
                }
            }
            if (children.Count > 0)
            {
                LayoutContainer child = children[children.Count - 1];
                float childRight = child.Location.x + child.WorkingSize.Width;
                if (childRight > gridSize.Width)
                {
                    gridSize.Width = childRight;
                }
                float childBottom = child.Location.y + child.WorkingSize.Height;
                if (childBottom > gridSize.Height)
                {
                    gridSize.Height = childBottom;
                }
            }
            if (GridLaidOut != null)
            {
                GridLaidOut.Invoke(this, EventArgs.Empty);
            }
        }

        public override Size2 DesiredSize
        {
            get
            {
                Size2 desiredSize = new Size2(startLocation.x, startLocation.y);
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

        public Vector2 StartLocation
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

        public Size2 GridSize
        {
            get
            {
                return gridSize;
            }
            set
            {
                gridSize = value;
            }
        }
    }
}
