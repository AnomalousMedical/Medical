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
            IntVector2 currentLocation = Location;
            int i = 0;
            int childCount = children.Count - 1;
            if (layoutType == LayoutType.Horizontal)
            {
                float sizeWithoutPadding = WorkingSize.Width - padding * childCount;
                foreach (MDIContainerBase child in children)
                {
                    IntSize2 childSize = child.DesiredSize;
                    IntSize2 actualSize = new IntSize2((int)(sizeWithoutPadding * (child.Scale / totalScale)), WorkingSize.Height);
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
                    IntSize2 childSize = child.DesiredSize;
                    IntSize2 actualSize = new IntSize2(WorkingSize.Width, (int)(sizeWithoutPadding * (child.Scale / totalScale)));
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

        internal StoredMDILayoutContainer storeCurrentLayout()
        {
            StoredMDILayoutContainer storedLayout = new StoredMDILayoutContainer();
            this.storeLayoutInfo(storedLayout, null, layoutType);
            return storedLayout;
        }

        internal void restoreLayout(StoredMDILayoutContainer storedLayout)
        {
            storedLayout.restoreWindows();
        }

        internal override void storeLayoutDetails(StoredMDILayoutContainer storedLayout)
        {
            //All dialogs on a given level have the same alignment and can have the dialog found before it as the parent with "right" or "bottom" alignment
            //A dialog on a child level will have the opposite alignment of its parent (that is why it is a child)

            //so you need a function store level details, which takes the alignment to the parent and the parent dialog
            //assign the first dialog to have the parent and parent alignment, any other dialogs on that level parent to the previous found dialog and have right or bottom alignment

            //Search level for the first dialog, store all elements up till the dialog
            //if a dialog is found
                //parent the dialog to the parent passed to the function with the alignment passed in
                //add child level elements to the "right" or "top" from the list of stored dialogs (calling the recursive func)
                //new loop
                    //add remaining child elements to the "left" or "bottom" of the current window
                    //if another window is found, change currentwindow
                    //parent the newly found window to the currentwindow with "left" or "bottom" alignment
            //if no dialog is found it means we are at the top level and the default order was changed, no other levels should be able to be completely empty, in this case flip
            //the alignment and call again with no parent dialog (this will cause the first found dialog to be the parent and its sibling will get the correct flipped alignment

            //NONE OF THE WRITTEN CODE DOES THE ABOVE, needs to be written

            //storedLayout.startLevel(layoutType == LayoutType.Horizontal ? WindowAlignment.Right : WindowAlignment.Bottom);
            foreach (MDIContainerBase container in children)
            {
                container.storeLayoutDetails(storedLayout);
            }
            storedLayout.endLevel();
        }

        //All dialogs on a given level have the same alignment and can have the dialog found before it as the parent with "right" or "bottom" alignment
        //A dialog on a child level will have the opposite alignment of its parent (that is why it is a child)

        //so you need a function store level details, which takes the alignment to the parent and the parent dialog
        //assign the first dialog to have the parent and parent alignment, any other dialogs on that level parent to the previous found dialog and have right or bottom alignment
        private void storeLayoutInfo(StoredMDILayoutContainer storedLayout, MDIWindow parent, LayoutType parentLayoutType)
        {
            bool foundWindow = false;
            int i = 0;
            for (i = 0; i < children.Count; ++i)
            {
                //Search level for the first dialog, store all elements up till the dialog
                //if a dialog is found
                MDIWindow currentWindow = children[i] as MDIWindow;
                if (currentWindow != null)
                {
                    foundWindow = true;
                    //parent the dialog to the parent passed to the function with the alignment passed in
                    //add child level elements to the "left" or "top" from the list of stored dialogs (calling the recursive func)
                    storedLayout.addWindowEntry(new WindowEntry(currentWindow, parentLayoutType == LayoutType.Horizontal ? WindowAlignment.Left : WindowAlignment.Top, parent));
                    foundWindow = true;
                    for (int j = 0; j < i; ++j)
                    {
                        ((MDILayoutContainer)children[j]).storeLayoutInfo(storedLayout, currentWindow, layoutType);
                    }
                    //add remaining child elements to the "right" or "bottom" of the current window
                    //if another window is found, change currentwindow
                    //parent the newly found window to the currentwindow with "left" or "bottom" alignment
                    MDIWindow previousWindow = currentWindow;
                    for (++i; i < children.Count; ++i)
                    {
                        currentWindow = children[i] as MDIWindow;
                        if (currentWindow != null)
                        {
                            storedLayout.addWindowEntry(new WindowEntry(currentWindow, layoutType == LayoutType.Horizontal ? WindowAlignment.Right : WindowAlignment.Bottom, previousWindow));
                            previousWindow = currentWindow;
                        }
                        else
                        {
                            ((MDILayoutContainer)children[i]).storeLayoutInfo(storedLayout, currentWindow, layoutType);
                        }
                    }
                }
            }
            if (!foundWindow)
            {
                //if no dialog is found it means we are at the top level and the default order was changed, no other levels should be able to be completely empty, in this case flip
                //the alignment and call again with no parent dialog (this will cause the first found dialog to be the parent and its sibling will get the correct flipped alignment
                foreach (MDILayoutContainer child in children)
                {
                    child.storeLayoutInfo(storedLayout, parent, child.layoutType);
                }
            }
        }

        /// <summary>
        /// LayoutContainer propery
        /// </summary>
        public override IntSize2 DesiredSize
        {
            get 
            {
                IntSize2 desiredSize = new IntSize2();
                if (layoutType == LayoutType.Horizontal)
                {
                    foreach (MDIContainerBase child in children)
                    {
                        IntSize2 childSize = child.DesiredSize;
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
                        IntSize2 childSize = child.DesiredSize;
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
