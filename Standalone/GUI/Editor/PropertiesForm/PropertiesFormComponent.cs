﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical.GUI
{
    public interface PropertiesFormComponent : IDisposable
    {
        void refreshData();

        LayoutContainer Container { get; }

        EditableProperty Property { get; }
    }
}
