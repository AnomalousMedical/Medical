using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class TextureCache : IDisposable
    {
        private LinkedList<String> lastAccessedOrder = new LinkedList<string>();
        private Dictionary<String, Image> loadedImages = new Dictionary<string, Image>();
        private UInt64 maxCacheSize = 500 * 1024 * 1024;
        private UInt64 currentCacheSize;

        public TextureCache()
        {

        }

        public void Dispose()
        {
            foreach(var image in loadedImages.Values)
            {
                image.Dispose();
            }
        }

        /// <summary>
        /// Get a texture from the cache, you should finish your work with it before calling
        /// add again, since it could be destroyed at that time.
        /// </summary>
        /// <param name="textureName"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        internal bool TryGetValue(string textureName, out Image image)
        {
            bool ret = loadedImages.TryGetValue(textureName, out image);
            if(ret)
            {
                lastAccessedOrder.Remove(textureName);
                lastAccessedOrder.AddFirst(textureName);
            }
            return ret;
        }

        /// <summary>
        /// Add a texture to the cache, be careful if you have outstanding pointers to objects in the
        /// cache as they could be destroyed during this call.
        /// </summary>
        /// <param name="textureName"></param>
        /// <param name="image"></param>
        internal void Add(string textureName, Image image)
        {
            UInt64 imageSize = image.Size;
            if (imageSize < maxCacheSize) //Image itself can fit
            {
                while(currentCacheSize + imageSize > maxCacheSize && lastAccessedOrder.Last != null)
                {
                    //Drop oldest images until there is enough space
                    String last = lastAccessedOrder.Last.Value;
                    lastAccessedOrder.RemoveLast();
                    var destroyImage = loadedImages[last];
                    loadedImages.Remove(last);
                    currentCacheSize -= destroyImage.Size;
                    destroyImage.Dispose();
                }
                currentCacheSize += image.Size;
                loadedImages.Add(textureName, image);
                lastAccessedOrder.AddFirst(textureName);
            }
        }
    }
}
