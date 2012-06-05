using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.GUI;
using SoundPlugin;

namespace Medical.Controller
{
    internal delegate void MDIMouseEvent(MDIWindow source, float mouseX, float mouseY);

    /// <summary>
    /// The base class for windows in the MDILayoutManager.
    /// </summary>
    public abstract class MDIWindow : MDIContainerBase, IDisposable
    {
        internal event MDIMouseEvent MouseDragStarted;
        internal event MDIMouseEvent MouseDrag;
        internal event MDIMouseEvent MouseDragFinished;

        protected MDILayoutManager layoutManager;
        private bool activeWindow = false;

        public MDIWindow(DockLocation currentLocation)
            :base(currentLocation)
        {
            AllowedDockLocations = DockLocation.All;
        }

        public virtual void Dispose()
        {

        }

        /// <summary>
        /// Set the MDILayoutManager.
        /// Do not touch unless you are MDILayoutManager.
        /// </summary>
        internal void _setMDILayoutManager(MDILayoutManager layoutManager)
        {
            this.layoutManager = layoutManager;
        }

        /// <summary>
        /// Change the active status of this window.
        /// Do not touch unless you are MDILayoutManager.
        /// </summary>
        internal void _doSetActive(bool active)
        {
            activeWindow = active;
            activeStatusChanged(active);
        }

        internal abstract void restoreToMDILayout(MDIWindow mDIWindow, WindowAlignment windowAlignment);

        protected abstract void activeStatusChanged(bool active);

        /// <summary>
        /// True if this is the currently active window. Setting this to true
        /// will make the window active. Setting it to false does nothing.
        /// </summary>
        public bool Active
        {
            get
            {
                return activeWindow;
            }
            set
            {
                if (value)
                {
                    layoutManager.ActiveWindow = this;
                }
            }
        }

        public DockLocation AllowedDockLocations { get; set; }

        protected void fireMouseDragStarted(MouseEventArgs me)
        {
            if (MouseDragStarted != null)
            {
                MouseDragStarted.Invoke(this, me.Position.x, me.Position.y);
            }
        }

        protected void fireMouseDrag(MouseEventArgs me)
        {
            if (MouseDrag != null)
            {
                MouseDrag.Invoke(this, me.Position.x, me.Position.y);
            }
        }

        protected void fireMouseDragFinished(MouseEventArgs me)
        {
            if (MouseDragFinished != null)
            {
                MouseDragFinished.Invoke(this, me.Position.x, me.Position.y);
            }
        }
    }
}
