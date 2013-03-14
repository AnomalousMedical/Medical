using System;
namespace Medical.GUI
{
    interface DialogEntry
    {
        void deserialize(Engine.ConfigFile file);
        void ensureVisible();
        void openMainGUIDialog();
        void serialize(Engine.ConfigFile file);
        void closeMainGUIDialog();
        void disposeDialog();
    }
}
