using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;
using Logging;

namespace Medical.Controller
{
    class MDILayoutContainer : LayoutContainer, IDisposable
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

        public MDILayoutContainer(LayoutType layoutType, int padding)
        {
            this.layoutType = layoutType;
            this.padding = padding;
        }

        public void Dispose()
        {
            foreach (Widget widget in separatorWidgets)
            {
                gui.destroyWidget(widget);
            }
        }

        public void addChild(LayoutContainer child)
        {
            setChildProperties(child);
            children.Add(child);
            invalidate();
        }

        public void insertChild(LayoutContainer child, LayoutContainer previous, bool after)
        {
            int index = children.IndexOf(previous);
            if (index == -1)
            {
                throw new MDIException("Attempted to add a MDIWindow with a previous window that does not exist in this collection.");
            }
            setChildProperties(child);
            if (after)
            {
                //Increment index and make sure it isnt the end of the list
                if (++index == children.Count)
                {
                    children.Add(child);
                }
                else
                {
                    children.Insert(index, child);
                }
            }
            else
            {
                children.Insert(index, child);
            }
        }

        public void swapAndRemove(LayoutContainer newChild, LayoutContainer oldChild)
        {
            int index = children.IndexOf(oldChild);
            if (index == -1)
            {
                throw new MDIException("Attempted to swap a MDIWindow with a old child window that does not exist in this collection.");
            }
            setChildProperties(newChild);
            children.Insert(index, newChild);
            children.Remove(oldChild);
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
            int i = 0;
            int childCount = children.Count - 1;
            float paddingOffset = childCount != 0 ? padding / childCount : 0;
            if (layoutType == LayoutType.Horizontal)
            {
                foreach (LayoutContainer child in children)
                {
                    Size2 childSize = child.DesiredSize;
                    Size2 actualSize = new Size2(WorkingSize.Width / children.Count - paddingOffset, WorkingSize.Height);
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
                foreach (LayoutContainer child in children)
                {
                    Size2 childSize = child.DesiredSize;
                    Size2 actualSize = new Size2(WorkingSize.Width, WorkingSize.Height / children.Count - paddingOffset);
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

        public LayoutType Layout
        {
            get
            {
                return layoutType;
            }
        }

        /// <summary>
        /// This method sets the child up to be a child of this container.
        /// </summary>
        /// <param name="child">The child to setup.</param>
        private void setChildProperties(LayoutContainer child)
        {
            child.SuppressLayout = true;
            child.Visible = visible;
            child.setAlpha(alpha);
            child._setParent(this);
            child.SuppressLayout = false;
            separatorWidgets.Add(gui.createWidgetT("Widget", "MDISeparator", 0, 0, padding, padding, Align.Left | Align.Top, "Main", ""));
        }
    }
}
