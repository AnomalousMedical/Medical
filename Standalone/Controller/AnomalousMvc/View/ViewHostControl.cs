using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    /// <summary>
    /// Provides an interface to a control that is part of a view host.
    /// </summary>
    public interface ViewHostControl
    {
        void focus();

        void blur();

        String Value { get; set; }
    }
}
