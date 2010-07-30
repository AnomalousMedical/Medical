using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.Controller
{
    /// <summary>
    /// Descibes the alignment of one window to another.
    /// </summary>
    public enum WindowAlignment
    {
        Left,
        Right,
        Top,
        Bottom,
    }

    /// <summary>
    /// This class provides a MDI-like interface for hosting multiple windows at once.
    /// </summary>
    public class MDILayoutManager : LayoutContainer, IDisposable
    {
        public event EventHandler ActiveWindowChanged;

        private List<MDIWindow> windows = new List<MDIWindow>();
        private MDILayoutContainer rootContainer;
        private MDIWindow activeWindow = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public MDILayoutManager()
            :this(5)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="padding">The amount of padding between elements.</param>
        public MDILayoutManager(int padding)
        {
            rootContainer = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, padding);
            rootContainer._setParent(this);
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        public void Dispose()
        {
            rootContainer.Dispose();
        }

        /// <summary>
        /// Show a window where the location does not matter.
        /// </summary>
        /// <param name="window">The window to add.</param>
        public void showWindow(MDIWindow window)
        {
            setWindowProperties(window);
            rootContainer.addChild(window);
            invalidate();
        }

        /// <summary>
        /// Show a window relative to another window.
        /// </summary>
        /// <param name="window">The window to show.</param>
        /// <param name="previous">The window to show window relative to.</param>
        /// <param name="alignment">The alignemnt of window to previous.</param>
        public void showWindow(MDIWindow window, MDIWindow previous, WindowAlignment alignment)
        {
            if (previous == null)
            {
                throw new MDIException("Previous window cannot be null.");
            }
            setWindowProperties(window);
            previous._ParentContainer.addChild(window, previous, alignment);
            invalidate();
        }

        /// <summary>
        /// Close a window.
        /// </summary>
        /// <param name="window"></param>
        public void closeWindow(MDIWindow window)
        {
            if (!windows.Contains(window))
            {
                throw new MDIException("Attempted to close a window that is not part of this MDILayoutManager.");
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
            window._ParentContainer.removeChild(window);
        }

        /// <summary>
        /// LayoutContainer function
        /// </summary>
        public override void bringToFront()
        {
            rootContainer.bringToFront();
        }

        /// <summary>
        /// LayoutContainer function
        /// </summary>
        public override void setAlpha(float alpha)
        {
            rootContainer.setAlpha(alpha);
        }

        /// <summary>
        /// LayoutContainer function
        /// </summary>
        public override void layout()
        {
            rootContainer.WorkingSize = WorkingSize;
            rootContainer.Location = Location;
            rootContainer.layout();
        }

        /// <summary>
        /// LayoutContainer function
        /// </summary>
        public override Size2 DesiredSize
        {
            get 
            {
                return rootContainer.DesiredSize;
            }
        }

        /// <summary>
        /// LayoutContainer function
        /// </summary>
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

        /// <summary>
        /// Get or set the "Active" window in the MDI interface. This is basicly which window is focused.
        /// </summary>
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

        /// <summary>
        /// Helper function to intialize windows that are added to the MDILayoutManager.
        /// </summary>
        /// <param name="window">The window to initialize.</param>
        private void setWindowProperties(MDIWindow window)
        {
            if (windows.Count == 0)
            {
                ActiveWindow = window;
            }
            window._setMDILayoutManager(this);
            windows.Add(window);
        }
    }
}
