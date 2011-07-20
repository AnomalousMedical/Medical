using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public interface FullscreenGUIPopup
    {
        void setSize(int width, int height);

        void setPosition(int x, int y);
    }
}
