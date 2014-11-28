using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.Controller
{
    class FloatingWindowContainer : MDIChildContainerBase
    {
        private List<MDIWindow> windows = new List<MDIWindow>();

        public FloatingWindowContainer()
            :base(DockLocation.Floating)
        {

        }

        public override void addChild(MDIWindow window)
        {
            window.SuppressLayout = true;
            window.CurrentDockLocation = DockLocation.Floating;
            window._setParent(this);
            window._ParentContainer = this;
            window.SuppressLayout = false;
            windows.Add(window);
        }

        public override MDIWindow findWindowAtPosition(float mouseX, float mouseY)
        {
            foreach (MDIWindow window in windows)
            {
                IntVector2 topLeft = window.Location;
                IntVector2 bottomRight = new IntVector2(window.WorkingSize.Width + topLeft.x, window.WorkingSize.Height + topLeft.y);
                if (topLeft.x < mouseX && topLeft.y < mouseY && bottomRight.x > mouseX && bottomRight.y > mouseY)
                {
                    return window.findWindowAtPosition(mouseX, mouseY);
                }
            }
            return null;
        }

        protected internal override bool isControlWidgetAtPosition(int mouseX, int mouseY)
        {
            return false;
        }

        public override void bringToFront()
        {
            
        }

        public override void setAlpha(float alpha)
        {
            
        }

        public override void layout()
        {
            
        }

        public override Engine.IntSize2 DesiredSize
        {
            get 
            {
                return new IntSize2(0, 0);
            }
        }

        public override bool Visible
        {
            get
            {
                return true;
            }
            set
            {
                
            }
        }

        public override IntSize2 ActualSize
        {
            get
            {
                //These will not be used for now, so don't worry about them
                return new IntSize2();
            }
            set
            {
                //These will not be used for now, so don't worry about them
            }
        }

        public override void addChild(MDIWindow window, MDIWindow previous, WindowAlignment alignment)
        {
            addChild(window);
        }

        public override void removeChild(MDIWindow window)
        {
            int index = windows.IndexOf(window);
            if (index != -1)
            {
                windows.RemoveAt(index);
                window._setParent(null);
                window._ParentContainer = null;
            }
        }

        /* This class has to subclass MDIChildContainerBase so it can have stuff added and removed, 
         * but these methods will never be called as this container is used in parallel to the rest 
         * of them.*/

        internal override MDILayoutContainer.LayoutType Layout
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        internal override void insertChild(MDIWindow child, MDIWindow previous, bool after)
        {
            throw new NotSupportedException();
        }

        internal override void swapAndRemove(MDIContainerBase newChild, MDIContainerBase oldChild)
        {
            throw new NotSupportedException();
        }

        internal override void promoteChild(MDIContainerBase mdiContainerBase, MDILayoutContainer mdiLayoutContainer)
        {
            throw new NotSupportedException();
        }

        internal StoredFloatingWindows storeCurrentLayout()
        {
            StoredFloatingWindows floatingWindows = new StoredFloatingWindows();
            foreach (MDIWindow window in windows)
            {
                floatingWindows.addFloatingWindow(window);
            }
            return floatingWindows;
        }

        internal void restoreLayout(StoredFloatingWindows floatingWindows)
        {
            floatingWindows.restoreWindows();
        }
    }
}
