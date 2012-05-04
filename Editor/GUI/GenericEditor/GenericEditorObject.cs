using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving.XMLSaver;
using Engine.Saving;
using System.Xml;
using System.IO;

namespace Medical.GUI
{
    interface GenericEditorObject
    {
        void createNew();

        EditInterface getEditInterface();

        void save(Stream stream);

        bool load(Stream stream);

        String ObjectTypeName { get; }

        String FileWildcard { get; }

        MedicalUICallback UICallback { get; }
    }
}
