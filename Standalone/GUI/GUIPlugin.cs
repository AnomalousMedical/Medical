using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Standalone;
using Engine.ObjectManagement;

namespace Medical.GUI
{
    public interface GUIPlugin : IDisposable
    {
        void createDialogs(StandaloneController standaloneController, DialogManager dialogManager);

        void addToTaskbar(Taskbar taskbar);

        void sceneLoaded(SimScene scene);

        void sceneUnloading(SimScene scene);
    }
}
