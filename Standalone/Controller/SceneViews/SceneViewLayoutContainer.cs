using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;
using Logging;

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
        private int padding;

        private List<LayoutContainer> children = new List<LayoutContainer>();

        private Gui gui = Gui.Instance;
        private List<Widget> separatorWidgets = new List<Widget>();

        public SceneViewLayoutContainer(LayoutType layoutType, int padding)
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
            separatorWidgets.Add(gui.createWidgetT("Widget", "MDISeparator", 0, 0, padding, padding, Align.Left | Align.Top, "Main", ""));
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
                totalWidth -= padding; //remove the last window's worth of padding
                int i = 0;
                foreach (LayoutContainer child in children)
                {
                    Size2 childSize = child.DesiredSize;
                    Size2 actualSize = new Size2(WorkingSize.Width * (childSize.Width / totalWidth), WorkingSize.Height);
                    child.WorkingSize = actualSize;
                    child.Location = currentLocation;
                    child.layout();
                    currentLocation.x += actualSize.Width;
                    separatorWidgets[i++].setCoord((int)currentLocation.x, (int)currentLocation.y, padding, (int)actualSize.Height);
                    currentLocation.x += padding;
                }
            }
            else
            {
                //Get the total size all child controls want to be
                float totalHeight = 0.0f;
                foreach (LayoutContainer child in children)
                {
                    totalHeight += child.DesiredSize.Height + padding;
                }
                totalHeight -= padding; //remove the last window's worth of padding
                int i = 0;
                foreach (LayoutContainer child in children)
                {
                    Size2 childSize = child.DesiredSize;
                    Size2 actualSize = new Size2(WorkingSize.Width, WorkingSize.Height * (childSize.Height / totalHeight));
                    child.WorkingSize = actualSize;
                    child.Location = currentLocation;
                    child.layout();
                    currentLocation.y += actualSize.Height;
                    separatorWidgets[i++].setCoord((int)currentLocation.x, (int)currentLocation.y, (int)actualSize.Width, padding);
                    currentLocation.y += padding;
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
    }
}
