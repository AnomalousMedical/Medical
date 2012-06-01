using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.GUI;

namespace Medical.Controller
{
    class SeparatorWidgetManager : IDisposable
    {
        private List<Widget> separatorWidgets = new List<Widget>();
        private Gui gui = Gui.Instance;
        private MDILayoutContainer parentContainer;

        public SeparatorWidgetManager(MDILayoutContainer parentContainer)
        {
            this.parentContainer = parentContainer;
        }

        public void Dispose()
        {
            foreach (Widget widget in separatorWidgets)
            {
                gui.destroyWidget(widget);
            }
        }

        public void createSeparator()
        {
            //Show the previous last separator if avaliable
            if (separatorWidgets.Count > 0)
            {
                separatorWidgets[separatorWidgets.Count - 1].Visible = true;
            }

            Widget separator = gui.createWidgetT("Widget", "MDISeparator", 0, 0, 10, 10, Align.Left | Align.Top, "Back", "");
            separatorWidgets.Add(separator);
            separator.MouseDrag += separator_MouseDrag;
            separator.MouseButtonPressed += separator_MouseButtonPressed;
            separator.MouseButtonReleased += separator_MouseButtonReleased;
            if (parentContainer.Layout == MDILayoutContainer.LayoutType.Horizontal)
            {
                separator.Pointer = MainWindow.SIZE_HORZ;
            }
            else
            {
                separator.Pointer = MainWindow.SIZE_VERT;
            }
            separator.Visible = false;
        }

        public void removeSeparator()
        {
            Widget separator = separatorWidgets[separatorWidgets.Count - 1];
            gui.destroyWidget(separator);
            separatorWidgets.RemoveAt(separatorWidgets.Count - 1);

            //Hide the new last separator
            if (separatorWidgets.Count > 0)
            {
                separatorWidgets[separatorWidgets.Count - 1].Visible = false;
            }
        }

        public void setSeparatorCoord(int index, int x, int y, int width, int height)
        {
            separatorWidgets[index].setCoord(x, y, width, height);
        }

        private IntVector2 dragStartPosition;
        private IntSize2 dragScaleArea;
        private MDIContainerBase dragLowChild;
        private float dragLowScaleStart;
        private MDIContainerBase dragHighChild;
        private float dragHighScaleStart;
        private float dragTotalScale;

        void separator_MouseDrag(Widget source, EventArgs e)
        {
            if (dragLowChild != null)
            {
                MouseEventArgs me = e as MouseEventArgs;
                IntVector2 offset = me.Position - dragStartPosition - parentContainer.Location;

                if (parentContainer.Layout == MDILayoutContainer.LayoutType.Horizontal)
                {
                    dragLowChild.Scale = dragLowScaleStart + (float)offset.x / dragScaleArea.Width * dragTotalScale;
                    dragHighChild.Scale = dragHighScaleStart - (float)offset.x / dragScaleArea.Width * dragTotalScale;
                }
                else
                {
                    dragLowChild.Scale = dragLowScaleStart + (float)offset.y / dragScaleArea.Height * dragTotalScale;
                    dragHighChild.Scale = dragHighScaleStart - (float)offset.y / dragScaleArea.Height * dragTotalScale;
                }

                //Bounds checking
                if (dragLowChild.Scale < 0)
                {
                    dragLowChild.Scale = 0.0f;
                    dragHighChild.Scale = dragTotalScale;
                }
                else if (dragHighChild.Scale < 0)
                {
                    dragLowChild.Scale = dragTotalScale;
                    dragHighChild.Scale = 0.0f;
                }

                parentContainer.invalidate();
            }
        }

        void separator_MouseButtonPressed(Widget source, EventArgs e)
        {
            int sepIndex = separatorWidgets.IndexOf(source);
            //ignore the last separator and do not allow the drag to happen if it is clicked.
            if (sepIndex != separatorWidgets.Count - 1)
            {
                dragStartPosition = ((MouseEventArgs)e).Position - parentContainer.Location;
                dragLowChild = parentContainer.getChild(sepIndex);
                dragLowScaleStart = dragLowChild.Scale;
                dragHighChild = parentContainer.getChild(sepIndex + 1);
                dragHighScaleStart = dragHighChild.Scale;
                dragTotalScale = dragLowScaleStart + dragHighScaleStart;
                dragScaleArea = (IntSize2)(dragTotalScale / parentContainer.TotalScale * parentContainer.WorkingSize);
            }
        }

        void separator_MouseButtonReleased(Widget source, EventArgs e)
        {
            dragLowChild = null;
            dragHighChild = null;
        }
    }
}
