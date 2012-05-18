using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using System.Xml;

namespace Medical
{
    /// <summary>
    /// A base class for stuff that manages saveable objects. It is designed to
    /// work with TypeControllers that only open one editor, want their objects
    /// cached and want the objects kept open until a new one is loaded.
    /// </summary>
    public abstract class SaveableTypeController : EditorTypeController
    {
        private EditorController editorController;
        protected SaveableCachedResource currentCachedResource;

        public SaveableTypeController(String extension, EditorController editorController)
            :base(extension)
        {
            this.editorController = editorController;
        }

        public Saveable loadObject(String filename)
        {
            //Check the cahce
            SaveableCachedResource cachedResource = editorController.ResourceProvider.ResourceCache[filename] as SaveableCachedResource;
            if (cachedResource == null)
            {
                //Missed open real file
                using (XmlTextReader xmlReader = new XmlTextReader(editorController.ResourceProvider.openFile(filename)))
                {
                    cachedResource = new SaveableTypeControllerCachedResource(filename, (Saveable)EditorController.XmlSaver.restoreObject(xmlReader), this);
                    editorController.ResourceProvider.ResourceCache.add(cachedResource);
                }
            }
            changeCachedResource(cachedResource);
            return cachedResource.Saveable;
        }

        public void saveObject(String filename, Saveable saveable)
        {
            using (XmlTextWriter writer = new XmlTextWriter(editorController.ResourceProvider.openWriteStream(filename), Encoding.Default))
            {
                writer.Formatting = Formatting.Indented;
                EditorController.XmlSaver.saveObject(saveable, writer);
            }
            editorController.ResourceProvider.ResourceCache.closeResource(filename);
        }

        protected void changeCachedResource(SaveableCachedResource newCachedResource)
        {
            if (currentCachedResource != null)
            {
                currentCachedResource.AllowClose = true;
            }
            currentCachedResource = newCachedResource;
            if (currentCachedResource != null)
            {
                currentCachedResource.AllowClose = false;
            }
        }
    }
}
