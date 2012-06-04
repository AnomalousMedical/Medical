using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.GUI;
using Engine;

namespace Medical.Controller
{
    class MDIBorderContainerDock : MDIChildContainerBase, IDisposable
    {
        private const int MIN_PIXEL_SIZE = 35;

        private MDILayoutContainer layoutContainer;
        private Widget separator;
        private int separatorSecondSize = 5;
        private IntSize2 size = new IntSize2();

        public MDIBorderContainerDock(MDILayoutContainer layoutContainer)
            :base(layoutContainer.CurrentDockLocation)
        {
            this.layoutContainer = layoutContainer;
            layoutContainer._setParent(this);
            separator = Gui.Instance.createWidgetT("Widget", "MDISeparator", 0, 0, separatorSecondSize, separatorSecondSize, Align.Left | Align.Top, "Back", "");
            separator.MouseDrag += separator_MouseDrag;
            switch(CurrentDockLocation)
            {
                case DockLocation.Left:
                    separator.Pointer = MainWindow.SIZE_HORZ;
                    break;
                case DockLocation.Right:
                    separator.Pointer = MainWindow.SIZE_HORZ;
                    break;
                case DockLocation.Top:
                    separator.Pointer = MainWindow.SIZE_VERT;
                    break;
                case DockLocation.Bottom:
                    separator.Pointer = MainWindow.SIZE_VERT;
                    break;
            }
            //separator.Visible = false;
        }

        public void Dispose()
        {
            Gui.Instance.destroyWidget(separator);
            layoutContainer.Dispose();
        }

        public override void layout()
        {
            switch(CurrentDockLocation)
            {
                case DockLocation.Left:
                    separator.setPosition((Location.x + WorkingSize.Width - separatorSecondSize), Location.y);
                    separator.setSize(separatorSecondSize, WorkingSize.Height);
                    layoutContainer.Location = Location;
                    layoutContainer.WorkingSize = new IntSize2(WorkingSize.Width - separatorSecondSize, WorkingSize.Height);
                    break;
                case DockLocation.Right:
                    separator.setPosition(Location.x, Location.y);
                    separator.setSize(separatorSecondSize, WorkingSize.Height);
                    layoutContainer.Location = new IntVector2(Location.x + separatorSecondSize, Location.y);
                    layoutContainer.WorkingSize = new IntSize2(WorkingSize.Width - separatorSecondSize, WorkingSize.Height);
                    break;
                case DockLocation.Top:
                    separator.setPosition(Location.x, (Location.y + WorkingSize.Height - separatorSecondSize));
                    separator.setSize(WorkingSize.Width, separatorSecondSize);
                    layoutContainer.Location = Location;
                    layoutContainer.WorkingSize = new IntSize2(WorkingSize.Width, WorkingSize.Height - separatorSecondSize);
                    break;
                case DockLocation.Bottom:
                    separator.setPosition(Location.x, (Location.y));
                    separator.setSize(WorkingSize.Width, separatorSecondSize);
                    layoutContainer.Location = new IntVector2(Location.x, Location.y + separatorSecondSize);
                    layoutContainer.WorkingSize = new IntSize2(WorkingSize.Width, WorkingSize.Height - separatorSecondSize);
                    break;
                default:
                    layoutContainer.Location = Location;
                    layoutContainer.WorkingSize = WorkingSize;
                    break;
            }
            layoutContainer.layout();
        }

        public override IntSize2 DesiredSize
        {
            get
            {
                return layoutContainer.HasChildren ? size : new IntSize2();
            }
        }

        public override bool Visible
        {
            get
            {
                return layoutContainer.Visible;
            }
            set
            {
                layoutContainer.Visible = value;
            }
        }

        public override MDIWindow findWindowAtPosition(float mouseX, float mouseY)
        {
            return layoutContainer.findWindowAtPosition(mouseX, mouseY);
        }

        public override void bringToFront()
        {
            layoutContainer.bringToFront();
        }

        public override void setAlpha(float alpha)
        {
            layoutContainer.setAlpha(alpha);
        }

        public override void addChild(MDIWindow window)
        {
            setFirstWindowSize(window);
            layoutContainer.addChild(window);
        }

        public override void addChild(MDIWindow window, MDIWindow previous, WindowAlignment alignment)
        {
            setFirstWindowSize(window);
            layoutContainer.addChild(window, previous, alignment);
        }

        public override void removeChild(MDIWindow window)
        {
            layoutContainer.removeChild(window);
        }

        internal StoredBorderContainerDock storeCurrentLayout()
        {
            StoredBorderContainerDock storedDock = new StoredBorderContainerDock(size);
            storedDock.MDILayoutContainer = layoutContainer.storeCurrentLayout();
            return storedDock;
        }

        internal void restoreLayout(StoredBorderContainerDock storedDock)
        {
            layoutContainer.restoreLayout(storedDock.MDILayoutContainer);
            this.size = storedDock.Size;
        }

        internal override MDILayoutContainer.LayoutType Layout
        {
            get
            {
                return layoutContainer.Layout;
            }
        }

        internal override void insertChild(MDIWindow child, MDIWindow previous, bool after)
        {
            setFirstWindowSize(child);
            layoutContainer.insertChild(child, previous, after);
        }

        internal override void swapAndRemove(MDIContainerBase newChild, MDIContainerBase oldChild)
        {
            layoutContainer.swapAndRemove(newChild, oldChild);
        }

        internal override void promoteChild(MDIContainerBase mdiContainerBase, MDILayoutContainer mdiLayoutContainer)
        {
            layoutContainer.promoteChild(mdiContainerBase, mdiLayoutContainer);
        }

        void separator_MouseDrag(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            switch (CurrentDockLocation)
            {
                case DockLocation.Left:
                    size = new IntSize2(me.Position.x - (int)Location.x, MIN_PIXEL_SIZE);
                    break;
                case DockLocation.Right:
                    size = new IntSize2((int)(Location.x + WorkingSize.Width) - me.Position.x, MIN_PIXEL_SIZE);
                    break;
                case DockLocation.Top:
                    size = new IntSize2(MIN_PIXEL_SIZE, me.Position.y - (int)Location.y);
                    break;
                case DockLocation.Bottom:
                    size = new IntSize2(MIN_PIXEL_SIZE, (int)(Location.y + WorkingSize.Height) - me.Position.y);
                    break;
            }
            if (size.Width < MIN_PIXEL_SIZE)
            {
                size.Width = MIN_PIXEL_SIZE;
            }
            if (size.Height < MIN_PIXEL_SIZE)
            {
                size.Height = MIN_PIXEL_SIZE;
            }
            invalidate();
        }

        private void setFirstWindowSize(MDIWindow child)
        {
            if (!layoutContainer.HasChildren)
            {
                size = child.DesiredSize + new IntSize2(separatorSecondSize, separatorSecondSize);
            }
        }
    }
}
