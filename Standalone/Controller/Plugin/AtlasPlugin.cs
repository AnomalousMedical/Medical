using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Medical.GUI;

namespace Medical
{
    public interface AtlasPlugin : IDisposable
    {
        void initializeGUI(StandaloneController standaloneController, GUIManager guiManager);

        void createDialogs(DialogManager dialogManager);

        void addToTaskbar(Taskbar taskbar);

        void finishInitialization();

        void sceneLoaded(SimScene scene);

        void sceneUnloading(SimScene scene);

        void setMainInterfaceEnabled(bool enabled);

        void createMenuBar(NativeMenuBar menu);
    }
}
