using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Reflection;

namespace Medical
{
    /// <summary>
    /// This class provides a base for a container for clipboard objects. It
    /// will emply a ReflectedSaver to save the subclasses with reflection,
    /// which simplifies subclass programming. There is no need to use this
    /// class as anything that is Saveable can be put into the clipboard.
    /// </summary>
    public class SaveableClipboardContainer : Saveable
    {
        private static FilteredMemberScanner scanner;

        static SaveableClipboardContainer()
        {
            scanner = new FilteredMemberScanner(new SaveableClipboardContainerMemberFilter());
            scanner.ProcessProperties = false;
        }

        public SaveableClipboardContainer()
        {

        }

        public SaveableClipboardContainer(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info, scanner);
        }

        public virtual void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info, scanner);
        }
    }
}
