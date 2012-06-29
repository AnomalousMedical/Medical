using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.GUI;

namespace Medical.Controller
{
    public class SeparatorWidgetManagerAbsolute : SeparatorWidgetManager
    {
        public SeparatorWidgetManagerAbsolute(MDILayoutContainer parentContainer)
            :base(parentContainer)
        {
            
        }

        private IntVector2 dragLastPosition;
        private MDIContainerBase dragLowChild;
        private MDIContainerBase dragHighChild;
        private IntSize2 dragTotalSize;

        protected override void separator_MouseDrag(Widget source, EventArgs e)
        {
            if (dragLowChild != null)
            {
                MouseEventArgs me = e as MouseEventArgs;
                IntVector2 offset = me.Position - dragLastPosition;

                if (parentContainer.Layout == MDILayoutContainer.LayoutType.Horizontal)
                {
                    IntSize2 lowSize = dragLowChild.ActualSize;
                    lowSize.Width += offset.x;

                    IntSize2 highSize = dragHighChild.ActualSize;
                    highSize.Width -= offset.x;

                    if (lowSize.Width < 0)
                    {
                        lowSize.Width = 0;
                        highSize.Width = dragTotalSize.Width;
                    }
                    else if (highSize.Width < 0)
                    {
                        highSize.Width = 0;
                        lowSize.Width = dragTotalSize.Width;
                    }

                    dragLowChild.ActualSize = lowSize;
                    dragHighChild.ActualSize = highSize;
                }
                else
                {
                    IntSize2 lowSize = dragLowChild.ActualSize;
                    lowSize.Height += offset.y;

                    IntSize2 highSize = dragHighChild.ActualSize;
                    highSize.Height -= offset.y;

                    if (lowSize.Height < 0)
                    {
                        lowSize.Height = 0;
                        highSize.Height = dragTotalSize.Height;
                    }
                    else if (highSize.Height < 0)
                    {
                        highSize.Height = 0;
                        lowSize.Height = dragTotalSize.Height;
                    }

                    dragLowChild.ActualSize = lowSize;
                    dragHighChild.ActualSize = highSize;
                }

                dragLastPosition = me.Position;

                parentContainer.invalidate();
            }
        }

        protected override void separator_MouseButtonPressed(Widget source, EventArgs e)
        {
            int sepIndex = separatorWidgets.IndexOf(source);
            //ignore the last separator and do not allow the drag to happen if it is clicked.
            if (sepIndex != separatorWidgets.Count - 1)
            {
                dragLastPosition = ((MouseEventArgs)e).Position;
                dragLowChild = parentContainer.getChild(sepIndex);
                dragHighChild = parentContainer.getChild(sepIndex + 1);
                dragTotalSize = dragLowChild.ActualSize + dragHighChild.ActualSize;
            }
        }

        protected override void separator_MouseButtonReleased(Widget source, EventArgs e)
        {
            dragLowChild = null;
            dragHighChild = null;
        }
    }
}
