using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class TextureCacheHandle : IDisposable
    {
        private int numCheckouts = 0;
        private bool destroyOnNoRef = false;
        private Image image;

        public TextureCacheHandle(Image image, bool destroyOnNoRef)
        {
            this.image = image;
            this.destroyOnNoRef = destroyOnNoRef;
        }

        public void Dispose()
        {
            --numCheckouts;
            if(destroyOnNoRef && numCheckouts == 0)
            {
                image.Dispose();
            }
        }

        public Image Image
        {
            get
            {
                return image;
            }
        }

        public ulong Size
        {
            get
            {
                return image.Size;
            }
        }

        internal void checkout()
        {
            ++numCheckouts;
        }

        internal void destroyIfPossible()
        {
            destroyOnNoRef = true;
            if(numCheckouts == 0)
            {
                image.Dispose();
            }
        }
    }

    class TextureCache : IDisposable
    {
        private LinkedList<String> lastAccessedOrder = new LinkedList<string>();
        private Dictionary<String, TextureCacheHandle> loadedImages = new Dictionary<string, TextureCacheHandle>();
        private UInt64 maxCacheSize = 500 * 1024 * 1024;
        private UInt64 currentCacheSize;
        private Object syncObject = new object();

        public TextureCache()
        {

        }

        public void Dispose()
        {
            foreach(var image in loadedImages.Values)
            {
                image.destroyIfPossible();
            }
        }

        /// <summary>
        /// Get a texture from the cache, you need to dispose the TextureCacheHandle that is
        /// returned from this function.
        /// </summary>
        /// <param name="textureName"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        internal bool TryGetValue(string textureName, out TextureCacheHandle image)
        {
            lock (syncObject)
            {
                bool ret = loadedImages.TryGetValue(textureName, out image);
                if (ret)
                {
                    lastAccessedOrder.Remove(textureName);
                    lastAccessedOrder.AddFirst(textureName);
                    image.checkout();
                }
                return ret;
            }
        }

        /// <summary>
        /// Add a texture to the cache, this will return a TextureCacheHandle that you MUST dispose
        /// when you aren't using it anymore. If 
        /// </summary>
        /// <param name="textureName"></param>
        /// <param name="image"></param>
        internal TextureCacheHandle Add(string textureName, Image image)
        {
            lock (syncObject)
            {
                TextureCacheHandle handle;
                UInt64 imageSize = image.Size;
                if (imageSize < maxCacheSize) //Image itself can fit
                {
                    while (currentCacheSize + imageSize > maxCacheSize && lastAccessedOrder.Last != null)
                    {
                        //Drop oldest images until there is enough space
                        String last = lastAccessedOrder.Last.Value;
                        lastAccessedOrder.RemoveLast();
                        var destroyImage = loadedImages[last];
                        loadedImages.Remove(last);
                        currentCacheSize -= destroyImage.Size;
                        destroyImage.destroyIfPossible();
                    }
                    currentCacheSize += image.Size;
                    handle = new TextureCacheHandle(image, false);
                    loadedImages.Add(textureName, handle);
                    lastAccessedOrder.AddFirst(textureName);
                }
                else
                {
                    handle = new TextureCacheHandle(image, true);
                }
                handle.checkout();
                return handle;
            }
        }
    }
}
