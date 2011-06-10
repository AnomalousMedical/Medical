using System;
namespace Medical.GUI
{
    interface DialogEntry
    {
        bool CurrentlyVisible { get; set; }
        void deserialize(Engine.ConfigFile file);
        void ensureVisible();
        void restoreState();
        void serialize(Engine.ConfigFile file);
        void tempClose();
    }
}
