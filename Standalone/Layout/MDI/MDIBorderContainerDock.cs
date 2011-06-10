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
        private MDILayoutContainer layoutContainer;
        private Widget separator;
        private int separatorSecondSize = 5;
        private Size2 size = new Size2();

        public MDIBorderContainerDock(MDILayoutContainer layoutContainer)
            :base(layoutContainer.CurrentDockLocation)
        {
            this.layoutContainer = layoutContainer;
            layoutContainer._setParent(this);
            separator = Gui.Instance.createWidgetT("Widget", "MDISeparator", 0, 0, separatorSecondSize, separatorSecondSize, Align.Left | Align.Top, "Back", "");
            separator.MouseDrag += separator_MouseDrag;
            separator.MouseButtonPressed += separator_MouseButtonPressed;
            separator.MouseButtonReleased += separator_MouseButtonReleased;
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
                    separator.setPosition((int)(Location.x + WorkingSize.Width - separatorSecondSize), (int)Location.y);
                    separator.setSize(separatorSecondSize, (int)WorkingSize.Height);
                    layoutContainer.Location = Location;
                    layoutContainer.WorkingSize = new Size2(WorkingSize.Width - separatorSecondSize, WorkingSize.Height);
                    break;
                case DockLocation.Right:
                    separator.setPosition((int)Location.x, (int)Location.y);
                    separator.setSize(separatorSecondSize, (int)WorkingSize.Height);
                    layoutContainer.Location = new Vector2(Location.x + separatorSecondSize, Location.y);
                    layoutContainer.WorkingSize = new Size2(WorkingSize.Width - separatorSecondSize, WorkingSize.Height);
                    break;
                case DockLocation.Top:
                    separator.setPosition((int)Location.x, (int)(Location.y + WorkingSize.Height - separatorSecondSize));
                    separator.setSize((int)WorkingSize.Width, separatorSecondSize);
                    layoutContainer.Location = Location;
                    layoutContainer.WorkingSize = new Size2(WorkingSize.Width, WorkingSize.Height - separatorSecondSize);
                    break;
                case DockLocation.Bottom:
                    separator.setPosition((int)Location.x, (int)(Location.y));
                    separator.setSize((int)WorkingSize.Width, separatorSecondSize);
                    layoutContainer.Location = new Vector2(Location.x, Location.y + separatorSecondSize);
                    layoutContainer.WorkingSize = new Size2(WorkingSize.Width, WorkingSize.Height - separatorSecondSize);
                    break;
                default:
                    layoutContainer.Location = Location;
                    layoutContainer.WorkingSize = WorkingSize;
                    break;
            }
            layoutContainer.layout();
        }

        public override Size2 DesiredSize
        {
            get
            {
                return layoutContainer.HasChildren ? size : new Size2();
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
            if (!layoutContainer.HasChildren)
            {
                size = window.DesiredSize;
            }
            layoutContainer.addChild(window);
        }

        public override void addChild(MDIWindow window, MDIWindow previous, WindowAlignment alignment)
        {
            if (!layoutContainer.HasChildren)
            {
                size = window.DesiredSize;
            }
            layoutContainer.addChild(window, previous, alignment);
        }

        public override void removeChild(MDIWindow window)
        {
            layoutContainer.removeChild(window);
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
            if (!layoutContainer.HasChildren)
            {
                size = child.DesiredSize;
            }
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
                    separator.setPosition(me.Position.x, separator.Top);
                    size = new Size2(separator.Left, 10);
                    invalidate();
                    break;
                case DockLocation.Right:
                    separator.setPosition(me.Position.x, separator.Top);
                    size = new Size2(TopmostWorkingSize.Width - separator.Left, 10);
                    invalidate();
                    break;
                case DockLocation.Top:
                    separator.setPosition(separator.Left, me.Position.y);
                    size = new Size2(10, separator.Top);
                    invalidate();
                    break;
                case DockLocation.Bottom:
                    separator.setPosition(separator.Left, me.Position.y);
                    size = new Size2(10, TopmostWorkingSize.Height - separator.Top);
                    invalidate();
                    break;
            }
            //if (dragLowChild != null)
            //{
            //    MouseEventArgs me = e as MouseEventArgs;
            //    Vector2 offset = me.Position - dragStartPosition - parentContainer.Location;

            //    if (parentContainer.Layout == MDILayoutContainer.LayoutType.Horizontal)
            //    {
            //        dragLowChild.Scale = dragLowScaleStart + offset.x / dragScaleArea.Width * dragTotalScale;
            //        dragHighChild.Scale = dragHighScaleStart - offset.x / dragScaleArea.Width * dragTotalScale;
            //    }
            //    else
            //    {
            //        dragLowChild.Scale = dragLowScaleStart + offset.y / dragScaleArea.Height * dragTotalScale;
            //        dragHighChild.Scale = dragHighScaleStart - offset.y / dragScaleArea.Height * dragTotalScale;
            //    }

            //    //Bounds checking
            //    if (dragLowChild.Scale < 0)
            //    {
            //        dragLowChild.Scale = 0.0f;
            //        dragHighChild.Scale = dragTotalScale;
            //    }
            //    else if (dragHighChild.Scale < 0)
            //    {
            //        dragLowChild.Scale = dragTotalScale;
            //        dragHighChild.Scale = 0.0f;
            //    }

            //    parentContainer.invalidate();
            //}
        }

        void separator_MouseButtonPressed(Widget source, EventArgs e)
        {

        }

        void separator_MouseButtonReleased(Widget source, EventArgs e)
        {
            //dragLowChild = null;
            //dragHighChild = null;
        }
    }
}
