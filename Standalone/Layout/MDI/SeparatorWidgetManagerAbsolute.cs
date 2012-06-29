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

        private IntVector2 dragStartPosition;
        private MDIContainerBase dragLowChild;

        private MDIContainerBase dragHighChild;

        protected override void separator_MouseDrag(Widget source, EventArgs e)
        {
            if (dragLowChild != null)
            {
                MouseEventArgs me = e as MouseEventArgs;
                IntVector2 offset = me.Position - dragStartPosition;// -parentContainer.Location;
                IntSize2 actualSize;

                if (parentContainer.Layout == MDILayoutContainer.LayoutType.Horizontal)
                {
                    actualSize = dragLowChild.ActualSize;
                    actualSize.Width += offset.x;
                    dragLowChild.ActualSize = actualSize;

                    actualSize = dragHighChild.ActualSize;
                    actualSize.Width -= offset.x;
                    dragHighChild.ActualSize = actualSize;
                }
                else
                {
                    actualSize = dragLowChild.ActualSize;
                    actualSize.Height += offset.y;
                    dragLowChild.ActualSize = actualSize;

                    actualSize = dragHighChild.ActualSize;
                    actualSize.Height -= offset.y;
                    dragHighChild.ActualSize = actualSize;
                }

                //Bounds checking
                //if (dragLowChild.Scale < 0)
                //{
                //    dragLowChild.Scale = 0.0f;
                //    dragHighChild.Scale = dragTotalScale;
                //}
                //else if (dragHighChild.Scale < 0)
                //{
                //    dragLowChild.Scale = dragTotalScale;
                //    dragHighChild.Scale = 0.0f;
                //}

                dragStartPosition = me.Position;

                parentContainer.invalidate();
            }
        }

        protected override void separator_MouseButtonPressed(Widget source, EventArgs e)
        {
            int sepIndex = separatorWidgets.IndexOf(source);
            //ignore the last separator and do not allow the drag to happen if it is clicked.
            if (sepIndex != separatorWidgets.Count - 1)
            {
                dragStartPosition = ((MouseEventArgs)e).Position;// -parentContainer.Location;
                dragLowChild = parentContainer.getChild(sepIndex);
                dragHighChild = parentContainer.getChild(sepIndex + 1);
            }
        }

        protected override void separator_MouseButtonReleased(Widget source, EventArgs e)
        {
            dragLowChild = null;
            dragHighChild = null;
        }
    }
}
