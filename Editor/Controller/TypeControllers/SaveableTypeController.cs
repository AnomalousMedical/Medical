using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using System.Xml;

namespace Medical
{
    /// <summary>
    /// A base class for stuff that manages saveable objects.
    /// </summary>
    public abstract class SaveableTypeController : EditorTypeController
    {
        private EditorController editorController;

        public SaveableTypeController(String extension, EditorController editorController)
            :base(extension)
        {
            this.editorController = editorController;
        }

        /// <summary>
        /// Helper function to load a saveable object. If the object is saved in
        /// the cache as a SaveableCachedResource it will return the cached
        /// version. This function does nothing to add the object to the cache,
        /// that must be done by the subclass.
        /// </summary>
        /// <param name="filename">The filename of the saveable object.</param>
        /// <returns>The cached object</returns>
        protected Saveable loadObject(String filename)
        {
            //Check the cahce
            SaveableCachedResource cachedResource = editorController.ResourceProvider.ResourceCache[filename] as SaveableCachedResource;
            if (cachedResource != null)
            {
                return cachedResource.Saveable;
            }

            //Missed open real file
            using (XmlTextReader xmlReader = new XmlTextReader(editorController.ResourceProvider.openFile(filename)))
            {
                return (Saveable)EditorController.XmlSaver.restoreObject(xmlReader);
            }
        }

        /// <summary>
        /// Helper function to save a saveable object. This does nothing to
        /// managed the cached status of the object, that is up to the subclass.
        /// </summary>
        /// <param name="filename">The filename of the saveable object.</param>
        /// <param name="saveable">The object to save</param>
        protected void saveObject(String filename, Saveable saveable)
        {
            using (XmlTextWriter writer = new XmlTextWriter(editorController.ResourceProvider.openWriteStream(filename), Encoding.Default))
            {
                writer.Formatting = Formatting.Indented;
                EditorController.XmlSaver.saveObject(saveable, writer);
            }
        }
    }
}
