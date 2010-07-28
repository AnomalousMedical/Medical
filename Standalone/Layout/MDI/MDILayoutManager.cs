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
        private List<MDIWindow> windows = new List<MDIWindow>();
        private LayoutContainer rootContainer = null;
        private List<MDILayoutContainer> childContainers = new List<MDILayoutContainer>();

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
            //Normal operation where the root is the MDILayoutContainer as expected
            if (rootContainer is MDILayoutContainer)
            {
                MDILayoutContainer mdiRoot = rootContainer as MDILayoutContainer;
                mdiRoot.addChild(window);
                window._CurrentContainer = mdiRoot;
            }
            //If no other containers have been added, use the root window directly.
            else if (rootContainer == null)
            {
                rootContainer = window;
                window._CurrentContainer = null;
                window._setParent(this);
            }
            //If one other container has been added, create a horizontal alignment and readd both containers to it
            else if (rootContainer is MDIWindow)
            {
                MDILayoutContainer horizRoot = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, 5);
                horizRoot._setParent(this);
                childContainers.Add(horizRoot);
                MDIWindow oldRoot = rootContainer as MDIWindow;
                horizRoot.addChild(oldRoot);
                oldRoot._CurrentContainer = horizRoot;
                rootContainer = horizRoot;

                horizRoot.addChild(window);
                window._CurrentContainer = horizRoot;
            }
            invalidate();
        }

        public void addWindow(MDIWindow window, MDIWindow previous, bool after)
        {
            previous._CurrentContainer.insertChild(window, previous, after);
            window._CurrentContainer = previous._CurrentContainer;
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
    }
}
