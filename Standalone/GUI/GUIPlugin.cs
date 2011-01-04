using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Standalone;

namespace Medical.GUI
{
    public interface GUIPlugin : IDisposable
    {
        void createDialogs(StandaloneController standaloneController, DialogManager dialogManager);

        void addToTaskbar(Taskbar taskbar);
        
    }
}
