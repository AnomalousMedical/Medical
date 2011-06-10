using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;
using Logging;

namespace Medical.Controller
{
    /// <summary>
    /// This class acts as a container inside of a MDI Layout. It will make the
    /// windows nest and do what they need to do to layout logically.
    /// </summary>
    public class MDILayoutContainer : MDIChildContainerBase, IDisposable
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
        private SeparatorWidgetManager separatorWidgetManager;

        private Gui gui = Gui.Instance;
        private float totalScale = 0.0f;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="layoutType">The alignment of the container.</param>
        /// <param name="padding">The amount of padding between elements.</param>
        public MDILayoutContainer(LayoutType layoutType, int padding, DockLocation dockLocation)
            :base(dockLocation)
        {
            this.layoutType = layoutType;
            this.padding = padding;
            separatorWidgetManager = new SeparatorWidgetManager(this);
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        public void Dispose()
        {
            separatorWidgetManager.Dispose();
        }

        /// <summary>
        /// Add a child in a simple way by just appending it to the end of the child list.
        /// </summary>
        /// <param name="child">The child to add.</param>
        public override void addChild(MDIWindow child)
        {
            setChildProperties(child);
            separatorWidgetManager.createSeparator();
            children.Add(child);
            totalScale += child.Scale;
            invalidate();
        }

        /// <summary>
        /// Add a child to a specific place with a specific alignment to the
        /// container. This method will create subcontainers as needed to get
        /// the layout desired.
        /// </summary>
        /// <param name="child">The child to add.</param>
        /// <param name="previous">The window to add the child relative to.</param>
        /// <param name="alignment">The alignment of child to previous.</param>
        public override void addChild(MDIWindow child, MDIWindow previous, WindowAlignment alignment)
        {
            switch (alignment)
            {
                case WindowAlignment.Left:
                    if (previous._ParentContainer.Layout == MDILayoutContainer.LayoutType.Horizontal)
                    {
                        //The child can simply be appended.
                        previous._ParentContainer.insertChild(child, previous, false);
                    }
                    else
                    {
                        //The child needs a new subcontainer created.
                        MDILayoutContainer newContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, Padding, CurrentDockLocation);
                        MDIChildContainerBase parentContainer = previous._ParentContainer;
                        parentContainer.swapAndRemove(newContainer, previous);
                        newContainer.addChild(child);
                        newContainer.addChild(previous);
                    }
                    break;
                case WindowAlignment.Right:
                    if (previous._ParentContainer.Layout == MDILayoutContainer.LayoutType.Horizontal)
                    {
                        //The child can simply be appended.
                        previous._ParentContainer.insertChild(child, previous, true);
                    }
                    else
                    {
                        //The child needs a new subcontainer created.
                        MDILayoutContainer newContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, Padding, CurrentDockLocation);
                        MDIChildContainerBase parentContainer = previous._ParentContainer;
                        parentContainer.swapAndRemove(newContainer, previous);
                        newContainer.addChild(previous);
                        newContainer.addChild(child);
                    }
                    break;
                case WindowAlignment.Top:
                    if (previous._ParentContainer.Layout == MDILayoutContainer.LayoutType.Vertical)
                    {
                        //The child can simply be appended.
                        previous._ParentContainer.insertChild(child, previous, false);
                    }
                    else
                    {
                        //The child needs a new subcontainer created.
                        MDILayoutContainer newContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Vertical, Padding, CurrentDockLocation);
                        MDIChildContainerBase parentContainer = previous._ParentContainer;
                        parentContainer.swapAndRemove(newContainer, previous);
                        newContainer.addChild(child);
                        newContainer.addChild(previous);
                    }
                    break;
                case WindowAlignment.Bottom:
                    if (previous._ParentContainer.Layout == MDILayoutContainer.LayoutType.Vertical)
                    {
                        //The child can simply be appended.
                        previous._ParentContainer.insertChild(child, previous, true);
                    }
                    else
                    {
                        //The child needs a new subcontainer created.
                        MDILayoutContainer newContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Vertical, Padding, CurrentDockLocation);
                        MDIChildContainerBase parentContainer = previous._ParentContainer;
                        parentContainer.swapAndRemove(newContainer, previous);
                        newContainer.addChild(previous);
                        newContainer.addChild(child);
                    }
                    break;
            }
        }

        /// <summary>
        /// This function removes a MDIWindow from this container. It will clean
        /// up any nested containers as required.
        /// </summary>
        /// <param name="child">The child window to remove.</param>
        public override void removeChild(MDIWindow child)
        {
            if (children.Contains(child))
            {
                separatorWidgetManager.removeSeparator();
                children.Remove(child);
                totalScale -= child.Scale;
                if (_ParentContainer != null && children.Count == 1)
                {
                    //Promote the child, which will destroy this container in the process as it is no longer needed.
                    _ParentContainer.promoteChild(children[0], this);
                }
                else
                {
                    invalidate();
                }
            }
        }

        public MDIContainerBase getChild(int index)
        {
            return children[index];
        }

        /// <summary>
        /// LayoutContainer function
        /// </summary>
        public override void bringToFront()
        {
            foreach (MDIContainerBase child in children)
            {
                child.bringToFront();
            }
        }

        /// <summary>
        /// LayoutContainer function
        /// </summary>
        public override void setAlpha(float alpha)
        {
            this.alpha = alpha;
            foreach (MDIContainerBase child in children)
            {
                child.setAlpha(alpha);
            }
        }

        /// <summary>
        /// LayoutContainer function
        /// </summary>
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
                    Size2 actualSize = new Size2(sizeWithoutPadding * (child.Scale / totalScale), WorkingSize.Height);
                    if (i == childCount) //Make sure to stretch the last child out completely, sometimes there is an extra pixel. This stops unsightly flickering.
                    {
                        actualSize.Width = WorkingSize.Width + Location.x - currentLocation.x;
                    }
                    child.WorkingSize = actualSize;
                    child.Location = currentLocation;
                    child.layout();
                    currentLocation.x += actualSize.Width;
                    separatorWidgetManager.setSeparatorCoord(i++, (int)currentLocation.x, (int)currentLocation.y, padding, (int)actualSize.Height);
                    currentLocation.x += padding;
                }
            }
            else
            {
                float sizeWithoutPadding = WorkingSize.Height - padding * childCount;
                foreach (MDIContainerBase child in children)
                {
                    Size2 childSize = child.DesiredSize;
                    Size2 actualSize = new Size2(WorkingSize.Width, sizeWithoutPadding * (child.Scale / totalScale));
                    if (i == childCount) //Make sure to stretch the last child out completely, sometimes there is an extra pixel. This stops unsightly flickering.
                    {
                        actualSize.Height = WorkingSize.Height + Location.y - currentLocation.y;
                    }
                    child.WorkingSize = actualSize;
                    child.Location = currentLocation;
                    child.layout();
                    currentLocation.y += actualSize.Height;
                    separatorWidgetManager.setSeparatorCoord(i++, (int)currentLocation.x, (int)currentLocation.y, (int)actualSize.Width, padding);
                    currentLocation.y += padding;
                }
            }
        }

        public override MDIWindow findWindowAtPosition(float mouseX, float mouseY)
        {
            foreach (MDIContainerBase child in children)
            {
                float left = child.Location.x;
                float top = child.Location.y;
                float right = left + child.WorkingSize.Width;
                float bottom = top + child.WorkingSize.Height;
                if (mouseX > left && mouseX < right && 
                    mouseY > top && mouseY < bottom)
                {
                    return child.findWindowAtPosition(mouseX, mouseY);
                }
            }
            return null;
        }

        /// <summary>
        /// LayoutContainer propery
        /// </summary>
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

        /// <summary>
        /// LayoutContainer propery
        /// </summary>
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

        /// <summary>
        /// The amount of pixel spacing between elements in the container.
        /// </summary>
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

        /// <summary>
        /// The LayoutType of this container.
        /// </summary>
        internal override LayoutType Layout
        {
            get
            {
                return layoutType;
            }
        }

        public float TotalScale
        {
            get
            {
                return totalScale;
            }
        }

        public bool HasChildren
        {
            get
            {
                return children.Count > 0;
            }
        }

        /// <summary>
        /// This method sets the child up to be a child of this container.
        /// </summary>
        /// <param name="child">The child to setup.</param>
        private void setChildProperties(MDIContainerBase child)
        {
            child.SuppressLayout = true;
            child.CurrentDockLocation = CurrentDockLocation;
            child.Visible = visible;
            child.setAlpha(alpha);
            child._setParent(this);
            child._ParentContainer = this;
            child.SuppressLayout = false;
        }

        /// <summary>
        /// Helper method to add a child before or after a given element.
        /// </summary>
        /// <param name="child">The child window to add.</param>
        /// <param name="previous">The window to add child relative to.</param>
        /// <param name="after">True to add child after previous. False to add child before previous.</param>
        internal override void insertChild(MDIWindow child, MDIWindow previous, bool after)
        {
            int index = children.IndexOf(previous);
            if (index == -1)
            {
                throw new MDIException("Attempted to add a MDIWindow with a previous window that does not exist in this collection.");
            }
            setChildProperties(child);
            separatorWidgetManager.createSeparator();
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
            totalScale += child.Scale;
        }

        /// <summary>
        /// Swap one MDIContainerBase for another in the child list. The
        /// oldChild will be removed from the list.
        /// </summary>
        /// <param name="newChild">The child to add.</param>
        /// <param name="oldChild">The child to replace.</param>
        internal override void swapAndRemove(MDIContainerBase newChild, MDIContainerBase oldChild)
        {
            int index = children.IndexOf(oldChild);
            if (index == -1)
            {
                throw new MDIException("Attempted to swap a MDIWindow with a old child window that does not exist in this collection.");
            }
            setChildProperties(newChild);
            children.Insert(index, newChild);
            children.Remove(oldChild);
            newChild.Scale = oldChild.Scale;
        }

        /// <summary>
        /// This is a helper method to promote a child to a parent container. It
        /// will swap the child with the formerParent's location and then
        /// dispose the formerParent.
        /// </summary>
        /// <param name="child">The child to promote.</param>
        /// <param name="formerParent">The former parent of child that will be replaced.</param>
        internal override void promoteChild(MDIContainerBase child, MDILayoutContainer formerParent)
        {
            swapAndRemove(child, formerParent);
            formerParent.Dispose();
            invalidate();
        }
    }
}
