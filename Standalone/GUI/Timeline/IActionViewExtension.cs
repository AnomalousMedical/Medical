using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    interface IActionViewExtension
    {
        void pixelsPerSecondChanged(int pixelsPerSecond);

        void scrollPositionChanged(Vector2 position);

        void scrollCanvasSizeChanged(Size2 size);
    }
}
