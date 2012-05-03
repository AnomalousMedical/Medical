using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    public interface ViewHost
    {
        LayoutContainer Container { get; }

        bool _RequestClosed { get; set; }

        void _animationCallback(LayoutContainer oldChild);
    }
}
