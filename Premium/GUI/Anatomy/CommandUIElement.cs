using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    abstract class CommandUIElement : LayoutContainer, IDisposable
    {
        public const int SIDE_PADDING = 10;

        public abstract void Dispose();

        public override void bringToFront()
        {
            
        }

        public override void setAlpha(float alpha)
        {
            
        }
    }
}
