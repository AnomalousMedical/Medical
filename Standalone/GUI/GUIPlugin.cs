using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;

namespace Medical.GUI
{
    public interface GUIPlugin : IDisposable
    {
        void initializeGUI(StandaloneController standaloneController, GUIManager guiManager);

        void createDialogs(DialogManager dialogManager);

        void addToTaskbar(Taskbar taskbar);

        void sceneLoaded(SimScene scene);

        void sceneUnloading(SimScene scene);

        void setMainInterfaceEnabled(bool enabled);

        void createMenuBar(MenuBar menu);
    }
}
