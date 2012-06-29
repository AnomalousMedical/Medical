using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.GUI;

namespace Medical.Controller
{
    public class SeparatorWidgetManagerScale : SeparatorWidgetManager
    {
        public SeparatorWidgetManagerScale(MDILayoutContainer parentContainer)
            :base(parentContainer)
        {
            
        }

        private IntVector2 dragStartPosition;
        private IntSize2 dragScaleArea;
        private MDIContainerBase dragLowChild;
        private float dragLowScaleStart;
        private MDIContainerBase dragHighChild;
        private float dragHighScaleStart;
        private float dragTotalScale;

        protected override void separator_MouseDrag(Widget source, EventArgs e)
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

        protected override void separator_MouseButtonPressed(Widget source, EventArgs e)
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

        protected override void separator_MouseButtonReleased(Widget source, EventArgs e)
        {
            dragLowChild = null;
            dragHighChild = null;
        }
    }
}
