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

        public MDIBorderContainerDock(MDILayoutContainer layoutContainer)
            :base(layoutContainer.CurrentDockLocation)
        {
            this.layoutContainer = layoutContainer;
            layoutContainer._setParent(this);
            separator = Gui.Instance.createWidgetT("Widget", "MDISeparator", 0, 0, 10, 10, Align.Left | Align.Top, "Back", "");
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
            separator.Visible = false;
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
                    //separator.setPosition(layoutContainer.
                    //separator.setSize(10, layoutContainer.WorkingSize.Height);
                    break;
            }
            layoutContainer.Location = Location;
            layoutContainer.WorkingSize = WorkingSize;
            layoutContainer.layout();
        }

        public override Size2 DesiredSize
        {
            get
            {
                return layoutContainer.ChildCount > 0 ? new Size2(300, 300) : new Size2();
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
            layoutContainer.addChild(window);
        }

        public override void addChild(MDIWindow window, MDIWindow previous, WindowAlignment alignment)
        {
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
            //int sepIndex = separatorWidgets.IndexOf(source);
            ////ignore the last separator and do not allow the drag to happen if it is clicked.
            //if (sepIndex != separatorWidgets.Count - 1)
            //{
            //    dragStartPosition = ((MouseEventArgs)e).Position - parentContainer.Location;
            //    dragLowChild = parentContainer.getChild(sepIndex);
            //    dragLowScaleStart = dragLowChild.Scale;
            //    dragHighChild = parentContainer.getChild(sepIndex + 1);
            //    dragHighScaleStart = dragHighChild.Scale;
            //    dragTotalScale = dragLowScaleStart + dragHighScaleStart;
            //    dragScaleArea = dragTotalScale / parentContainer.TotalScale * parentContainer.WorkingSize;
            //}
        }

        void separator_MouseButtonReleased(Widget source, EventArgs e)
        {
            //dragLowChild = null;
            //dragHighChild = null;
        }
    }
}
