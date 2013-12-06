using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public abstract class SingleChildLayoutContainer : LayoutContainer
    {
        public abstract LayoutContainer Child { get; set; }
    }
}
