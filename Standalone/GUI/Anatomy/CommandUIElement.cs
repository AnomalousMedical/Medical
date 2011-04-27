using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    abstract class CommandUIElement : IDisposable
    {
        public const int SIDE_PADDING = 10;

        public abstract void Dispose();

        public LayoutContainer LayoutContainer { get; protected set; }
    }
}
