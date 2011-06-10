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
                Vector2 topLeft = window.Location;
                Vector2 bottomRight = new Vector2(window.WorkingSize.Width + topLeft.x, window.WorkingSize.Height + topLeft.y);
                if (topLeft.x < mouseX && topLeft.y < mouseY && bottomRight.x > mouseX && bottomRight.y > mouseY)
                {
                    return window.findWindowAtPosition(mouseX, mouseY);
                }
            }
            return null;
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

        public override Engine.Size2 DesiredSize
        {
            get 
            {
                return new Size2(0, 0);
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
    }
}
