using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    public abstract class TextTypeController : EditorTypeController
    {
        protected TextCachedResource currentCachedResource;

        public event Action<String> ItemOpened;

        public TextTypeController(String extension, EditorController editorController)
            :base(extension, editorController)
        {
            
        }

        public override void closeFile(string file)
        {
            EditorController.ResourceProvider.ResourceCache.forceCloseResourceFile(file);
        }

        public override void openFile(string file)
        {
            if (ItemOpened != null)
            {
                ItemOpened.Invoke(file);
            }
        }

        public String loadText(String filename)
        {
            //Check the cahce
            TextCachedResource cachedResource = EditorController.ResourceProvider.ResourceCache[filename] as TextCachedResource;
            if (cachedResource == null)
            {
                //Missed open real file
                using (StreamReader stringReader = new StreamReader(EditorController.ResourceProvider.openFile(filename)))
                {
                    cachedResource = new TextTypeControllerCachedResource(filename, stringReader.ReadToEnd(), this);
                    EditorController.ResourceProvider.ResourceCache.add(cachedResource);
                }
            }
            changeCachedResource(cachedResource);
            return cachedResource.CachedString;
        }

        public void saveText(string file, string text)
        {
            updateCachedText(file, text);

            //Save the file
            using (StreamWriter streamWriter = new StreamWriter(EditorController.ResourceProvider.openWriteStream(file)))
            {
                streamWriter.Write(text);
            }
            EditorController.ResourceProvider.ResourceCache.closeResource(file);
        }

        public void updateCachedText(String file, String text)
        {
            if (EditorController.ResourceProvider != null)
            {
                //Update the cached string
                TextCachedResource cachedResource = EditorController.ResourceProvider.ResourceCache[file] as TextCachedResource;
                if (cachedResource != null)
                {
                    cachedResource.CachedString = text;
                }
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

        private void changeCachedResource(TextCachedResource newCachedResource)
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
