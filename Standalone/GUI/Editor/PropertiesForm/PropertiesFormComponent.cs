using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical.GUI
{
    interface PropertiesFormComponent : IDisposable
    {
        LayoutContainer Container { get; }

        EditableProperty Property { get; }
    }
}
