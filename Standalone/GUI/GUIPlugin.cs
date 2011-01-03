using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Standalone;

namespace Medical.GUI
{
    abstract class GUIPlugin : IDisposable
    {
        public GUIPlugin(StandaloneController standaloneController)
        {

        }

        public virtual void Dispose()
        {

        }

        public virtual void addToTaskbar(Taskbar taskbar)
        {

        }
    }
}
