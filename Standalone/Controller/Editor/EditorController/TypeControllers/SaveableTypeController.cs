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
    public abstract class SaveableTypeController<T> : EditorTypeController
        where T : Saveable
    {
        public event Action<String, T> OpenEditor;

        protected SaveableCachedResource<T> currentCachedResource;

        public SaveableTypeController(String extension, EditorController editorController)
            :base(extension, editorController)
        {
            
        }

        public override void closeFile(string file)
        {
            EditorController.ResourceProvider.ResourceCache.forceCloseResourceFile(file);
        }

        public override void openEditor(string file)
        {
            if (OpenEditor != null)
            {
                OpenEditor.Invoke(file, loadObject(file));
            }
        }

        public T loadObject(String filename)
        {
            //Check the cahce
            SaveableCachedResource<T> cachedResource = EditorController.ResourceProvider.ResourceCache[filename] as SaveableCachedResource<T>;
            if (cachedResource == null)
            {
                //Missed open real file
                using (XmlTextReader xmlReader = new XmlTextReader(EditorController.ResourceProvider.openFile(filename)))
                {
                    cachedResource = new SaveableTypeControllerCachedResource<T>(filename, (T)EditorController.XmlSaver.restoreObject(xmlReader), this);
                    EditorController.ResourceProvider.ResourceCache.add(cachedResource);
                }
            }
            changeCachedResource(cachedResource);
            return cachedResource.Saveable;
        }

        public void saveObject(String filename, T saveable)
        {
            using (XmlTextWriter writer = new XmlTextWriter(EditorController.ResourceProvider.openWriteStream(filename), Encoding.Default))
            {
                writer.Formatting = Formatting.Indented;
                EditorController.XmlSaver.saveObject(saveable, writer);
            }
            EditorController.ResourceProvider.ResourceCache.closeResource(filename);
        }

        public T CurrentObject
        {
            get
            {
                if (currentCachedResource != null)
                {
                    return currentCachedResource.Saveable;
                }
                return default(T);
            }
        }

        public String CurrentFile
        {
            get
            {
                if (currentCachedResource != null)
                {
                    return currentCachedResource.File;
                }
                return null;
            }
        }

        protected void creatingNewFile(String filePath)
        {
            if (currentCachedResource != null && currentCachedResource.isSameFile(filePath))
            {
                closeCurrentCachedResource();
            }
        }

        protected void closeCurrentCachedResource()
        {
            if(currentCachedResource != null && EditorController.ResourceProvider != null)
            {
                currentCachedResource.AllowClose = true;
                EditorController.ResourceProvider.ResourceCache.closeResource(currentCachedResource.File);
                changeCachedResource(null);
            }
        }

        private void changeCachedResource(SaveableCachedResource<T> newCachedResource)
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
