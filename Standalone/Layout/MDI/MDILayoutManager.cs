using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using MyGUIPlugin;

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

        private Widget windowTargetWidget;

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
            AllowActiveWindowChanges = true;
            windowTargetWidget = Gui.Instance.createWidgetT("Widget", "Panel", 0, 0, 10, 10, Align.Left | Align.Top, "Overlapped", "");
            windowTargetWidget.Visible = false;
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        public void Dispose()
        {
            Gui.Instance.destroyWidget(windowTargetWidget);
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
        /// Inject a mouse button down event.
        /// </summary>
        /// <param name="absMouse">The location of the event.</param>
        /// <param name="mouseButton">The mouse button that was pressed.</param>
        /// <param name="mouseAlreadyHandled">True if some other subsystem already did some mouse input.</param>
        public void injectMouseDown(Vector3 absMouse, MouseButtonCode mouseButton, bool mouseAlreadyHandled)
        {
            
        }

        /// <summary>
        /// Inject a mouse button up event.
        /// </summary>
        /// <param name="absMouse">The location of the event.</param>
        /// <param name="mouseButton">The mouse button that was pressed.</param>
        /// <param name="mouseAlreadyHandled">True if some other subsystem already did some mouse input.</param>
        public void injectMouseUp(Vector3 absMouse, MouseButtonCode mouseButton, bool mouseAlreadyHandled)
        {
            if (!mouseAlreadyHandled && mouseButton == MouseButtonCode.MB_BUTTON0)
            {
                foreach (MDIWindow window in windows)
                {
                    Vector2 topLeft = window.Location;
                    Vector2 bottomRight = new Vector2(window.WorkingSize.Width + topLeft.x, window.WorkingSize.Height + topLeft.y);
                    if (topLeft.x < absMouse.x && topLeft.y < absMouse.y && bottomRight.x > absMouse.x && bottomRight.y > absMouse.y)
                    {
                        ActiveWindow = window;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Inject the mouse location.
        /// </summary>
        /// <param name="absMouse">The location of the mouse cursor.</param>
        public void injectMouseLocation(Vector3 absMouse)
        {

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
                if (activeWindow != value && AllowActiveWindowChanges)
                {
                    if (activeWindow != null)
                    {
                        activeWindow._doSetActive(false);
                        activeWindow.MouseDragStarted -= activeWindow_MouseDragStarted;
                        activeWindow.MouseDrag -= activeWindow_MouseDrag;
                        activeWindow.MouseDragFinished -= activeWindow_MouseDragFinished;
                    }
                    activeWindow = value;
                    if (activeWindow != null)
                    {
                        activeWindow._doSetActive(true);
                        activeWindow.MouseDragStarted += activeWindow_MouseDragStarted;
                        activeWindow.MouseDrag += activeWindow_MouseDrag;
                        activeWindow.MouseDragFinished += activeWindow_MouseDragFinished;
                    }
                    if (ActiveWindowChanged != null)
                    {
                        ActiveWindowChanged.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        private MDIWindow dragSourceWindow;
        private MDIWindow dragTargetWindow;
        private WindowAlignment finalWindowAlignment = WindowAlignment.Right;

        void activeWindow_MouseDragStarted(MDIWindow source, float mouseX, float mouseY)
        {
            dragSourceWindow = source;
            dragTargetWindow = null;
        }

        void activeWindow_MouseDrag(MDIWindow source, float mouseX, float mouseY)
        {
            dragTargetWindow = rootContainer.findWindowAtPosition(mouseX, mouseY);
            if (dragTargetWindow != null)
            {
                if (source != dragTargetWindow)
                {
                    float top = dragTargetWindow.Location.y;
                    float bottom = top + dragTargetWindow.WorkingSize.Height;
                    float left = dragTargetWindow.Location.x;
                    float right = left + dragTargetWindow.WorkingSize.Width;

                    float topDelta = mouseY - top;
                    float bottomDelta = bottom - mouseY;
                    float leftDelta = mouseX - left;
                    float rightDelta = right - mouseX;
                    if (topDelta < bottomDelta && topDelta < leftDelta && topDelta < rightDelta)
                    {
                        finalWindowAlignment = WindowAlignment.Top;
                        windowTargetWidget.setCoord((int)left, (int)top, (int)dragTargetWindow.WorkingSize.Width, (int)(dragTargetWindow.WorkingSize.Height * 0.5f));
                    }
                    else if (bottomDelta < topDelta && bottomDelta < leftDelta && bottomDelta < rightDelta)
                    {
                        finalWindowAlignment = WindowAlignment.Bottom;
                        windowTargetWidget.setCoord((int)left, (int)(top + dragTargetWindow.WorkingSize.Height * 0.5f), (int)dragTargetWindow.WorkingSize.Width, (int)(dragTargetWindow.WorkingSize.Height * 0.5f));
                    }
                    else if (leftDelta < topDelta && leftDelta < bottomDelta && leftDelta < rightDelta)
                    {
                        finalWindowAlignment = WindowAlignment.Left;
                        windowTargetWidget.setCoord((int)left, (int)top, (int)(dragTargetWindow.WorkingSize.Width * 0.5f), (int)dragTargetWindow.WorkingSize.Height);
                    }
                    else if (rightDelta < leftDelta && rightDelta < bottomDelta && rightDelta < topDelta)
                    {
                        finalWindowAlignment = WindowAlignment.Right;
                        windowTargetWidget.setCoord((int)(left + dragTargetWindow.WorkingSize.Width * 0.5f), (int)top, (int)(dragTargetWindow.WorkingSize.Width * 0.5f), (int)dragTargetWindow.WorkingSize.Height);
                    }

                    windowTargetWidget.Visible = true;
                }
                else
                {
                    windowTargetWidget.Visible = false;
                }
            }
        }

        void activeWindow_MouseDragFinished(MDIWindow source, float mouseX, float mouseY)
        {
            windowTargetWidget.Visible = false;
            if (dragTargetWindow != dragSourceWindow && dragTargetWindow != null)
            {
                dragSourceWindow._ParentContainer.removeChild(dragSourceWindow);
                dragTargetWindow._ParentContainer.addChild(dragSourceWindow, dragTargetWindow, finalWindowAlignment);
                rootContainer.layout();
            }
        }

        /// <summary>
        /// True if the ActiveWindow can change, false to block changes. False
        /// will lock the active window and it will not be able to be changed by
        /// the user or code.
        /// </summary>
        public bool AllowActiveWindowChanges { get; set; }

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
