using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    abstract class TextTypeController : EditorTypeController
    {
        private EditorController editorController;
        protected TextCachedResource currentCachedResource;

        public TextTypeController(String extension, EditorController editorController)
            :base(extension)
        {
            this.editorController = editorController;
        }

        public override void closeFile(string file)
        {
            editorController.ResourceProvider.ResourceCache.forceCloseResourceFile(file);
        }

        public String loadText(String filename)
        {
            //Check the cahce
            TextCachedResource cachedResource = editorController.ResourceProvider.ResourceCache[filename] as TextCachedResource;
            if (cachedResource == null)
            {
                //Missed open real file
                using (StreamReader stringReader = new StreamReader(editorController.ResourceProvider.openFile(filename)))
                {
                    cachedResource = new TextTypeControllerCachedResource(filename, stringReader.ReadToEnd(), this);
                    editorController.ResourceProvider.ResourceCache.add(cachedResource);
                }
            }
            changeCachedResource(cachedResource);
            return cachedResource.CachedString;
        }

        public void saveText(string file, string text)
        {
            updateCachedText(file, text);

            //Save the file
            using (StreamWriter streamWriter = new StreamWriter(editorController.ResourceProvider.openWriteStream(file)))
            {
                streamWriter.Write(text);
            }
            editorController.ResourceProvider.ResourceCache.closeResource(file);
        }

        public void updateCachedText(String file, String text)
        {
            //Update the cached string
            TextCachedResource cachedResource = editorController.ResourceProvider.ResourceCache[file] as TextCachedResource;
            if (cachedResource != null)
            {
                cachedResource.CachedString = text;
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
            if(currentCachedResource != null)
            {
                currentCachedResource.AllowClose = true;
                editorController.ResourceProvider.ResourceCache.closeResource(currentCachedResource.File);
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
