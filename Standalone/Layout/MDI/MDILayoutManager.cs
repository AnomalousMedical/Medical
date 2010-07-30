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
        private MDILayoutContainer rootContainer;
        private List<MDILayoutContainer> childContainers = new List<MDILayoutContainer>();
        private MDIWindow activeWindow = null;

        public MDILayoutManager()
        {
            rootContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, PADDING_SIZE);
            rootContainer._setParent(this);
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
            rootContainer.addChild(window);
            invalidate();
        }

        public void addWindow(MDIWindow window, MDIWindow previous, WindowAlignment alignment)
        {
            if (previous == null)
            {
                throw new MDIException("Previous window cannot be null.");
            }
            setWindowProperties(window);
            previous._CurrentContainer.addChild(window, previous, alignment);
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
            rootContainer.bringToFront();
        }

        public override void setAlpha(float alpha)
        {
            rootContainer.setAlpha(alpha);
        }

        public override void layout()
        {
            rootContainer.WorkingSize = WorkingSize;
            rootContainer.Location = Location;
            rootContainer.layout();
        }

        public override Size2 DesiredSize
        {
            get 
            {
                return rootContainer.DesiredSize;
            }
        }

        public override bool Visible
        {
            get
            {
                return rootContainer.Visible;
            }
            set
            {
                rootContainer.Visible = value;
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
