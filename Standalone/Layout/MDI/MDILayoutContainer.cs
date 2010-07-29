using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;
using Logging;

namespace Medical.Controller
{
    public class MDILayoutContainer : MDIContainerBase, IDisposable
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

        private List<MDIContainerBase> children = new List<MDIContainerBase>();

        private Gui gui = Gui.Instance;
        private List<Widget> separatorWidgets = new List<Widget>();

        public MDILayoutContainer(LayoutType layoutType, int padding, MDILayoutManager layoutManager)
        {
            this.layoutType = layoutType;
            this.padding = padding;
            this.layoutManager = layoutManager;
        }

        public void Dispose()
        {
            foreach (Widget widget in separatorWidgets)
            {
                gui.destroyWidget(widget);
            }
        }

        public void addChild(MDIContainerBase child)
        {
            setChildProperties(child);
            separatorWidgets.Add(gui.createWidgetT("Widget", "MDISeparator", 0, 0, padding, padding, Align.Left | Align.Top, "Main", ""));
            children.Add(child);
            invalidate();
        }

        public void insertChild(MDIContainerBase child, MDIContainerBase previous, bool after)
        {
            int index = children.IndexOf(previous);
            if (index == -1)
            {
                throw new MDIException("Attempted to add a MDIWindow with a previous window that does not exist in this collection.");
            }
            setChildProperties(child);
            separatorWidgets.Add(gui.createWidgetT("Widget", "MDISeparator", 0, 0, padding, padding, Align.Left | Align.Top, "Main", ""));
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

        public void swapAndRemove(MDIContainerBase newChild, MDIContainerBase oldChild)
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

        public void removeChild(MDIContainerBase child)
        {
            if (children.Contains(child))
            {
                Widget separator = separatorWidgets[separatorWidgets.Count - 1];
                Gui.Instance.destroyWidget(separator);
                separatorWidgets.RemoveAt(separatorWidgets.Count - 1);
                children.Remove(child);
                if (children.Count == 0)
                {
                    //All children are deleted remove this container too
                    if (_CurrentContainer != null)
                    {
                        _CurrentContainer.removeChild(this);
                    }
                }
                else
                {
                    invalidate();
                }
            }
        }

        public void clearChildren()
        {
            children.Clear();
            foreach (Widget separator in separatorWidgets)
            {
                Gui.Instance.destroyWidget(separator);
            }
            separatorWidgets.Clear();
        }

        public override void bringToFront()
        {
            foreach (MDIContainerBase child in children)
            {
                child.bringToFront();
            }
        }

        public override void setAlpha(float alpha)
        {
            this.alpha = alpha;
            foreach (MDIContainerBase child in children)
            {
                child.setAlpha(alpha);
            }
        }

        public override void layout()
        {
            Vector2 currentLocation = Location;
            int i = 0;
            int childCount = children.Count - 1;            
            if (layoutType == LayoutType.Horizontal)
            {
                float sizeWithoutPadding = WorkingSize.Width - padding * childCount;
                foreach (MDIContainerBase child in children)
                {
                    Size2 childSize = child.DesiredSize;
                    Size2 actualSize = new Size2(sizeWithoutPadding / children.Count, WorkingSize.Height);
                    if (i == childCount) //Make sure to stretch the last child out completely, sometimes there is an extra pixel. This stops unsightly flickering.
                    {
                        actualSize.Width = WorkingSize.Width + Location.x - currentLocation.x;
                    }
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
                float sizeWithoutPadding = WorkingSize.Height - padding * childCount;
                foreach (MDIContainerBase child in children)
                {
                    Size2 childSize = child.DesiredSize;
                    Size2 actualSize = new Size2(WorkingSize.Width, sizeWithoutPadding / children.Count);
                    if (i == childCount) //Make sure to stretch the last child out completely, sometimes there is an extra pixel. This stops unsightly flickering.
                    {
                        actualSize.Height = WorkingSize.Height + Location.y - currentLocation.y;
                    }
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
                    foreach (MDIContainerBase child in children)
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
                    foreach (MDIContainerBase child in children)
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
                foreach (MDIContainerBase child in children)
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
        private void setChildProperties(MDIContainerBase child)
        {
            child.SuppressLayout = true;
            child.Visible = visible;
            child.setAlpha(alpha);
            child._setParent(this);
            child._CurrentContainer = this;
            child.SuppressLayout = false;
        }
    }
}
