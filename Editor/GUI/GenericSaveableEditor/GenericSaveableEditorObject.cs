using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving.XMLSaver;
using Engine.Saving;
using System.Xml;

namespace Medical.GUI
{
    interface GenericSaveableEditorObject
    {
        void createNew();

        EditInterface getEditInterface();

        void save(XmlSaver saver, XmlTextWriter xmlWriter);

        bool load(XmlSaver saver, XmlReader xmlReader);

        void unloaded();

        String ObjectTypeName { get; }
    }
}
