using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// This class helps manage a handle to a resource in the TextureCache. This uses reference counting,
    /// which is incremented internally for the checkout and is decremented on dispose. The idea is to
    /// allow for a using block when using these handles.
    /// </summary>
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

        /// <summary>
        /// Dispose, call when finished with a checked out handle. Destroys
        /// the image if there are no more outstanding resources and the cahce requested it.
        /// </summary>
        public void Dispose()
        {
            --numCheckouts;
            if(destroyOnNoRef && numCheckouts == 0)
            {
                image.Dispose();
            }
        }

        /// <summary>
        /// The image for this handle.
        /// </summary>
        public Image Image
        {
            get
            {
                return image;
            }
        }

        /// <summary>
        /// The size of the image.
        /// </summary>
        public ulong Size
        {
            get
            {
                return image.Size;
            }
        }

        /// <summary>
        /// Internal function to increment the counter.
        /// </summary>
        internal void checkout()
        {
            ++numCheckouts;
        }

        /// <summary>
        /// Internal management function to destroy the image if possible.
        /// </summary>
        internal void destroyIfPossible()
        {
            destroyOnNoRef = true;
            if(numCheckouts == 0)
            {
                image.Dispose();
            }
        }
    }

    /// <summary>
    /// A texture cache that textures can be added to, it will automatically flush itself as items are added
    /// that make it too large.
    /// </summary>
    class TextureCache : IDisposable
    {
        private LinkedList<String> lastAccessedOrder = new LinkedList<string>();
        private Dictionary<String, TextureCacheHandle> loadedImages = new Dictionary<string, TextureCacheHandle>();
        private UInt64 maxCacheSize;
        private UInt64 currentCacheSize;
        private Object syncObject = new object();

        public TextureCache(UInt64 maxCacheSize)
        {
            this.maxCacheSize = maxCacheSize;
        }

        public void Dispose()
        {
            clear();
        }

        /// <summary>
        /// Get a texture from the cache, you need to dispose the TextureCacheHandle that is
        /// returned through the image out variable.
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
        /// when you aren't using it anymore.
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

        internal void clear()
        {
            foreach (var image in loadedImages.Values)
            {
                image.destroyIfPossible();
            }
            loadedImages.Clear();
            lastAccessedOrder.Clear();
            currentCacheSize = 0;
        }
    }
}
