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
        private float totalScale = 0.0f;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="layoutType">The alignment of the container.</param>
        /// <param name="padding">The amount of padding between elements.</param>
        public MDILayoutContainer(LayoutType layoutType, int padding)
        {
            this.layoutType = layoutType;
            this.padding = padding;
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        public void Dispose()
        {
            foreach (Widget widget in separatorWidgets)
            {
                gui.destroyWidget(widget);
            }
        }

        /// <summary>
        /// Add a child in a simple way by just appending it to the end of the child list.
        /// </summary>
        /// <param name="child">The child to add.</param>
        public void addChild(MDIWindow child)
        {
            setChildProperties(child);
            createSeparatorWidget();
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
        public void addChild(MDIWindow child, MDIWindow previous, WindowAlignment alignment)
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
                        MDILayoutContainer newContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, Padding);
                        MDILayoutContainer parentContainer = previous._ParentContainer;
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
                        MDILayoutContainer newContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, Padding);
                        MDILayoutContainer parentContainer = previous._ParentContainer;
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
                        MDILayoutContainer newContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Vertical, Padding);
                        MDILayoutContainer parentContainer = previous._ParentContainer;
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
                        MDILayoutContainer newContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Vertical, Padding);
                        MDILayoutContainer parentContainer = previous._ParentContainer;
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
        public void removeChild(MDIWindow child)
        {
            if (children.Contains(child))
            {
                Widget separator = separatorWidgets[separatorWidgets.Count - 1];
                Gui.Instance.destroyWidget(separator);
                separatorWidgets.RemoveAt(separatorWidgets.Count - 1);
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
                    Size2 actualSize = new Size2(WorkingSize.Width, sizeWithoutPadding * (child.Scale / totalScale));
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
            child._ParentContainer = this;
            child.SuppressLayout = false;
        }

        /// <summary>
        /// Helper method to add a child before or after a given element.
        /// </summary>
        /// <param name="child">The child window to add.</param>
        /// <param name="previous">The window to add child relative to.</param>
        /// <param name="after">True to add child after previous. False to add child before previous.</param>
        private void insertChild(MDIWindow child, MDIWindow previous, bool after)
        {
            int index = children.IndexOf(previous);
            if (index == -1)
            {
                throw new MDIException("Attempted to add a MDIWindow with a previous window that does not exist in this collection.");
            }
            setChildProperties(child);
            createSeparatorWidget();
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
        private void swapAndRemove(MDIContainerBase newChild, MDIContainerBase oldChild)
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
        private void promoteChild(MDIContainerBase child, MDILayoutContainer formerParent)
        {
            swapAndRemove(child, formerParent);
            formerParent.Dispose();
            invalidate();
        }

        private void createSeparatorWidget()
        {
            Widget separator = gui.createWidgetT("Widget", "MDISeparator", 0, 0, padding, padding, Align.Left | Align.Top, "Back", "");
            separatorWidgets.Add(separator);
            separator.MouseDrag += separator_MouseDrag;
            separator.MouseButtonPressed += separator_MouseButtonPressed;
            separator.MouseButtonReleased += separator_MouseButtonReleased;
            if (layoutType == LayoutType.Horizontal)
            {
                separator.Pointer = CursorManager.SIZE_HORZ;
            }
            else
            {
                separator.Pointer = CursorManager.SIZE_VERT;
            }
        }

        private Vector2 dragStartPosition;
        private Size2 dragScaleArea;
        private MDIContainerBase dragLowChild;
        private float dragLowScaleStart;
        private MDIContainerBase dragHighChild;
        private float dragHighScaleStart;
        private float dragTotalScale;

        void separator_MouseDrag(Widget source, EventArgs e)
        {
            if (dragLowChild != null)
            {
                MouseEventArgs me = e as MouseEventArgs;
                Vector2 offset = me.Position - dragStartPosition - Location;

                if (Layout == LayoutType.Horizontal)
                {
                    dragLowChild.Scale = dragLowScaleStart + offset.x / dragScaleArea.Width * dragTotalScale;
                    dragHighChild.Scale = dragHighScaleStart - offset.x / dragScaleArea.Width * dragTotalScale;
                }
                else
                {
                    dragLowChild.Scale = dragLowScaleStart + offset.y / dragScaleArea.Height * dragTotalScale;
                    dragHighChild.Scale = dragHighScaleStart - offset.y / dragScaleArea.Height * dragTotalScale;
                }

                //Bounds checking
                if (dragLowChild.Scale < 0)
                {
                    dragLowChild.Scale = 0.0f;
                    dragHighChild.Scale = dragTotalScale;
                }
                else if (dragHighChild.Scale < 0)
                {
                    dragLowChild.Scale = dragTotalScale;
                    dragHighChild.Scale = 0.0f;
                }

                invalidate();
            }
        }

        void separator_MouseButtonPressed(Widget source, EventArgs e)
        {
            int sepIndex = separatorWidgets.IndexOf(source);
            //ignore the last separator and do not allow the drag to happen if it is clicked.
            if (sepIndex != separatorWidgets.Count - 1)
            {
                dragStartPosition = ((MouseEventArgs)e).Position - Location;
                dragLowChild = children[sepIndex];
                dragLowScaleStart = dragLowChild.Scale;
                dragHighChild = children[sepIndex + 1];
                dragHighScaleStart = dragHighChild.Scale;
                dragTotalScale = dragLowScaleStart + dragHighScaleStart;
                dragScaleArea = dragTotalScale / totalScale * WorkingSize;
            }
        }

        void separator_MouseButtonReleased(Widget source, EventArgs e)
        {
            dragLowChild = null;
            dragHighChild = null;
        }
    }
}
