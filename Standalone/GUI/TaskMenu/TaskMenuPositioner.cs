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
                if (left + width > Gui.Instance.getViewWidth())
                {
                    left -= left + width - Gui.Instance.getViewWidth();
                }
                if (top + height > Gui.Instance.getViewHeight())
                {
                    top -= top + height - Gui.Instance.getViewHeight();
                }
                return new IntVector2(left, top);
            }
            return new IntVector2();
        }

        public ButtonGridItem CurrentItem { get; set; }
    }
}
