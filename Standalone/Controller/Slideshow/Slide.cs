using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;

namespace Medical
{
    public interface Slide : Saveable
    {
        View createView(String name);

        MvcController createController(String name, String viewName, ResourceProvider resourceProvider);

        String UniqueName { get; }
    }
}
