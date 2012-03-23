using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class TaskMenuPositioner : TaskPositioner
    {
        public TaskMenuPositioner()
        {

        }

        public IntVector2 findGoodWindowPosition(int width, int height)
        {
            if (CurrentItem != null)
            {
                int left = CurrentItem.AbsoluteLeft;
                int top = CurrentItem.AbsoluteTop + CurrentItem.Height;
                int viewWidth = RenderManager.Instance.ViewWidth;
                int viewHeight = RenderManager.Instance.ViewHeight;
                if (left + width > viewWidth)
                {
                    left -= left + width - viewWidth;
                }
                if (top + height > viewHeight)
                {
                    top -= top + height - viewHeight;
                }
                return new IntVector2(left, top);
            }
            return new IntVector2();
        }

        public ButtonGridItem CurrentItem { get; set; }
    }
}
