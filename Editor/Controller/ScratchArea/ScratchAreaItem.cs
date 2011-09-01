using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving.XMLSaver;
using Engine.Saving;
using System.IO;
using System.Xml;
using Logging;
using Engine.Editing;

namespace Medical
{
    partial class ScratchAreaItem
    {
        private static XmlSaver xmlSaver = new XmlSaver();

        private String name;
        private ScratchAreaFolder parent;

        public ScratchAreaItem(String name, ScratchAreaFolder parent)
        {
            this.name = name;
            this.parent = parent;
        }

        public void serialize(Saveable saveable)
        {
            try
            {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(FilesystemPath, Encoding.Default))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlSaver.saveObject(saveable, xmlWriter);
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not save ScratchAreaItem {0} of type {1} because {2}", name, saveable.ToString(), e.Message);
            }
        }

        public Saveable deserialize()
        {
            try
            {
                using (XmlTextReader xmlReader = new XmlTextReader(FilesystemPath))
                {
                    return (Saveable)xmlSaver.restoreObject(xmlReader);
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not load ScratchAreaItem {0} because {1}", name, e.Message);
            }
            return null;
        }

        public void deleteFile()
        {
            try
            {
                File.Delete(FilesystemPath);
            }
            catch (Exception e)
            {
                Log.Error("Could not delete ScratchAreaItem {0} at {1} because {2}", name, FilesystemPath, e.Message);
            }
        }

        public void renameFile(String newName)
        {
            try
            {
                String originalPath = FilesystemPath;
                String newPath = Path.Combine(parent.FilesystemPath, newName + ".sav");
                File.Copy(originalPath, newPath);
                File.Delete(FilesystemPath);
            }
            catch (Exception e)
            {
                Log.Error("Could not rename ScratchAreaItem {0} at {1} because {2}", name, FilesystemPath, e.Message);
            }
        }

        public String FilesystemPath
        {
            get
            {
                return Path.Combine(parent.FilesystemPath, name + ".sav");
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
        }
    }

    partial class ScratchAreaItem
    {
        private EditInterface editInterface = null;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = new EditInterface(Name);
                    editInterface.addCommand(new EditInterfaceCommand("Copy", copyCallback));
                }
                return editInterface;
            }
        }

        private void copyCallback(EditUICallback callback, EditInterfaceCommand caller)
        {
            SaveableClipboard clipboard = null;
            callback.runCustomQuery(ScratchAreaCustomQueries.GetClipboard, delegate(Object result, ref String errorMessage)
            {
                clipboard = result as SaveableClipboard;
                return true;
            });
            if (clipboard != null)
            {
                Saveable deserializedItem = deserialize();
                if(deserializedItem != null)
                {
                    clipboard.copyToSourceObject(deserializedItem);
                }
            }
        }
    }
}
