using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.GUI;
using Engine;

namespace Medical.Controller
{
    class MDIBorderContainerSeparator : IDisposable
    {
        private MDILayoutContainer layoutContainer;
        private Widget separator;

        public MDIBorderContainerSeparator(MDILayoutContainer layoutContainer)
        {
            this.layoutContainer = layoutContainer;
            separator = Gui.Instance.createWidgetT("Widget", "MDISeparator", 0, 0, 10, 10, Align.Left | Align.Top, "Back", "");
            separator.MouseDrag += separator_MouseDrag;
            separator.MouseButtonPressed += separator_MouseButtonPressed;
            separator.MouseButtonReleased += separator_MouseButtonReleased;
            switch(layoutContainer.CurrentDockLocation)
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
        }

        public bool Visible
        {
            get
            {
                return separator.Visible;
            }
            set
            {
                separator.Visible = value;
            }
        }

        public Size2 ContainerSize
        {
            get
            {
                return layoutContainer.ChildCount > 0 ? new Size2(300, 300) : new Size2();
            }
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
