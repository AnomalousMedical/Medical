using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    interface ViewHostComponent : IDisposable
    {
        void topLevelResized();

        void opening();

        void closing();

        MyGUIViewHost ViewHost { get; }

        Widget Widget { get; }
    }
}
