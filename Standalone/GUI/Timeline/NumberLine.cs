using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class NumberLine : IActionViewExtension
    {
        private ScrollView numberlineScroller;
        private int pixelsPerSecond = 100;

        public NumberLine(ScrollView numberlineScroller)
        {
            this.numberlineScroller = numberlineScroller;
        }

        public int PixelsPerSecond
        {
            get
            {
                return pixelsPerSecond;
            }
            set
            {
                pixelsPerSecond = value;
            }
        }

        #region IActionViewExtension Members

        public void pixelsPerSecondChanged(int pixelsPerSecond)
        {
            
        }

        public void scrollPositionChanged(Vector2 position)
        {
            
        }

        public void scrollCanvasSizeChanged(Size2 size)
        {
            
        }

        #endregion
    }
}
