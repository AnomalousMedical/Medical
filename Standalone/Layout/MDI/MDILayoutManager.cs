using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.Controller
{
    public enum WindowAlignment
    {
        Left,
        Right,
        Top,
        Bottom,
    }

    public class MDILayoutManager : LayoutContainer, IDisposable
    {
        public event EventHandler ActiveWindowChanged;

        private const int PADDING_SIZE = 5;

        private List<MDIWindow> windows = new List<MDIWindow>();
        private LayoutContainer rootContainer = null;
        private List<MDILayoutContainer> childContainers = new List<MDILayoutContainer>();
        private MDIWindow activeWindow = null;

        public MDILayoutManager()
        {

        }

        public void Dispose()
        {
            foreach (MDILayoutContainer child in childContainers)
            {
                child.Dispose();
            }
        }

        public void addWindow(MDIWindow window)
        {
            setWindowProperties(window);
            //Normal operation where the root is the MDILayoutContainer as expected
            if (rootContainer is MDILayoutContainer)
            {
                MDILayoutContainer mdiRoot = rootContainer as MDILayoutContainer;
                mdiRoot.addChild(window);
            }
            //If no other containers have been added, use the root window directly.
            else if (rootContainer == null)
            {
                rootContainer = window;
                window._setParent(this);
                ActiveWindow = window;
            }
            //If one other container has been added, create a horizontal alignment and readd both containers to it
            else if (rootContainer is MDIWindow)
            {
                MDILayoutContainer horizRoot = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, PADDING_SIZE, this);
                horizRoot._setParent(this);
                childContainers.Add(horizRoot);
                MDIWindow oldRoot = rootContainer as MDIWindow;
                horizRoot.addChild(oldRoot);
                rootContainer = horizRoot;

                horizRoot.addChild(window);
            }
            invalidate();
        }

        public void addWindow(MDIWindow window, MDIWindow previous, WindowAlignment alignment)
        {
            if (previous == null)
            {
                throw new MDIException("Previous window cannot be null.");
            }
            setWindowProperties(window);
            switch (alignment)
            {
                case WindowAlignment.Left:
                    if (previous._CurrentContainer == null && previous == rootContainer)
                    {
                        MDILayoutContainer parentContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, PADDING_SIZE, this);
                        parentContainer._setParent(this);
                        parentContainer.addChild(previous);
                        parentContainer.addChild(window);
                        rootContainer = parentContainer;
                    }
                    else if (previous._CurrentContainer.Layout == MDILayoutContainer.LayoutType.Horizontal)
                    {
                        previous._CurrentContainer.insertChild(window, previous, true);
                    }
                    else
                    {
                        MDILayoutContainer newContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, PADDING_SIZE, this);
                        MDILayoutContainer parentContainer = previous._CurrentContainer;
                        parentContainer.swapAndRemove(newContainer, previous);
                        newContainer.addChild(previous);
                        newContainer.addChild(window);
                    }
                    break;
                case WindowAlignment.Right:
                    if (previous._CurrentContainer == null && previous == rootContainer)
                    {
                        MDILayoutContainer parentContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, PADDING_SIZE, this);
                        parentContainer._setParent(this);
                        parentContainer.addChild(window);
                        parentContainer.addChild(previous);
                        rootContainer = parentContainer;
                    }
                    else if (previous._CurrentContainer.Layout == MDILayoutContainer.LayoutType.Horizontal)
                    {
                        previous._CurrentContainer.insertChild(window, previous, false);
                    }
                    else
                    {
                        MDILayoutContainer newContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, PADDING_SIZE, this);
                        MDILayoutContainer parentContainer = previous._CurrentContainer;
                        parentContainer.swapAndRemove(newContainer, previous);
                        newContainer.addChild(window);
                        newContainer.addChild(previous);
                    }
                    break;
                case WindowAlignment.Top:
                    if (previous._CurrentContainer == null && previous == rootContainer)
                    {
                        MDILayoutContainer parentContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Vertical, PADDING_SIZE, this);
                        parentContainer._setParent(this);
                        parentContainer.addChild(window);
                        parentContainer.addChild(previous);
                        rootContainer = parentContainer;
                    }
                    else if (previous._CurrentContainer.Layout == MDILayoutContainer.LayoutType.Vertical)
                    {
                        previous._CurrentContainer.insertChild(window, previous, false);
                    }
                    else
                    {
                        MDILayoutContainer newContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Vertical, PADDING_SIZE, this);
                        MDILayoutContainer parentContainer = previous._CurrentContainer;
                        parentContainer.swapAndRemove(newContainer, previous);
                        newContainer.addChild(window);
                        newContainer.addChild(previous);
                    }
                    break;
                case WindowAlignment.Bottom:
                    if (previous._CurrentContainer == null && previous == rootContainer)
                    {
                        MDILayoutContainer parentContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Vertical, PADDING_SIZE, this);
                        parentContainer._setParent(this);
                        parentContainer.addChild(previous);
                        parentContainer.addChild(window);
                        rootContainer = parentContainer;
                    }
                    else if (previous._CurrentContainer.Layout == MDILayoutContainer.LayoutType.Vertical)
                    {
                        previous._CurrentContainer.insertChild(window, previous, true);
                    }
                    else
                    {
                        MDILayoutContainer newContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Vertical, PADDING_SIZE, this);
                        MDILayoutContainer parentContainer = previous._CurrentContainer;
                        parentContainer.swapAndRemove(newContainer, previous);
                        newContainer.addChild(previous);
                        newContainer.addChild(window);
                    }
                    break;
            }
            invalidate();
        }

        public void removeWindow(MDIWindow window)
        {
            if (!windows.Contains(window))
            {
                throw new MDIException("Attempted to remove a window that is not part of this MDILayoutManager.");
            }
            windows.Remove(window);
            //Check to see if this window was the active window.
            if (window == ActiveWindow)
            {
                //If there are windows left use the first one as the new active window, otherwise set the active window to null.
                if (windows.Count > 0)
                {

                    ActiveWindow = windows[0];
                }
                else
                {
                    ActiveWindow = null;
                }
            }
            window._CurrentContainer.removeChild(window);
        }

        public override void bringToFront()
        {
            if (rootContainer != null)
            {
                rootContainer.bringToFront();
            }
        }

        public override void setAlpha(float alpha)
        {
            if (rootContainer != null)
            {
                rootContainer.setAlpha(alpha);
            }
        }

        public override void layout()
        {
            if (rootContainer != null)
            {
                rootContainer.WorkingSize = WorkingSize;
                rootContainer.Location = Location;
                rootContainer.layout();
            }
        }

        public override Size2 DesiredSize
        {
            get 
            {
                if (rootContainer != null)
                {
                    return rootContainer.DesiredSize;
                }
                return new Size2();
            }
        }

        private bool visible = true;

        public override bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
                if (rootContainer != null)
                {
                    rootContainer.Visible = value;
                }
            }
        }

        public MDIWindow ActiveWindow
        {
            get
            {
                return activeWindow;
            }
            set
            {
                if (activeWindow != value)
                {
                    if (activeWindow != null)
                    {
                        activeWindow._doSetActive(false);
                    }
                    activeWindow = value;
                    if (activeWindow != null)
                    {
                        activeWindow._doSetActive(true);
                    }
                    if (ActiveWindowChanged != null)
                    {
                        ActiveWindowChanged.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void setWindowProperties(MDIWindow window)
        {
            window._setMDILayoutManager(this);
            windows.Add(window);
        }
    }
}
