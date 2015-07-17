using Engine;
using Engine.Threads;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class TextureLoader : IDisposable
    {
        private HashSet<VTexPage> addedPages;
        private List<VTexPage> removedPages;

        private List<PTexPage> physicalPageQueue; //FIFO queue for used pages, allows us to reuse pages if they are requested again quickly and keep track of what parts of the physical texture we can use
        private Dictionary<VTexPage, PTexPage> physicalPagePool;
        private Dictionary<VTexPage, PTexPage> usedPhysicalPages;

        private VirtualTextureManager virtualTextureManager;
        private int maxPages;
        private int textelsPerPage;
        private int padding;
        private int padding2;
        private int textelsPerPhysicalPage;
        private bool cancelBackgroundLoad = false;

        private int stagingImageCount = 0;
        private StagingImage[] stagingImages;
        private Task<bool>[] copyTostagingImageTasks;
        private TextureCache textureCache = new TextureCache();

        Task loadingTask;
        bool stopLoading = false;
        List<VTexPage> pagesToLoad = new List<VTexPage>();

        public TextureLoader(VirtualTextureManager virtualTextureManager, IntSize2 physicalTextureSize, int textelsPerPage, int padding, int stagingImageCapacity)
        {
            this.virtualTextureManager = virtualTextureManager;
            IntSize2 pageTableSize = physicalTextureSize / textelsPerPage;
            this.maxPages = pageTableSize.Width * pageTableSize.Height;
            this.textelsPerPage = textelsPerPage;
            this.padding = padding;
            this.padding2 = padding * 2;
            this.textelsPerPhysicalPage = textelsPerPage + padding2;

            addedPages = new HashSet<VTexPage>();
            removedPages = new List<VTexPage>(10);
            pagesToLoad = new List<VTexPage>(10);

            stagingImages = new StagingImage[stagingImageCapacity];
            copyTostagingImageTasks = new Task<bool>[stagingImageCapacity];

            float scale = (float)textelsPerPage / textelsPerPhysicalPage;
            PagePaddingScale = new Vector2(scale, scale);

            scale = (float)padding / textelsPerPhysicalPage;
            PagePaddingOffset = new Vector2(scale, scale);

            //Build pool
            int x = 0;
            int y = 0;
            int pageX = 0;
            int pageY = 0;
            physicalPageQueue = new List<PTexPage>(maxPages);
            physicalPagePool = new Dictionary<VTexPage, PTexPage>();
            usedPhysicalPages = new Dictionary<VTexPage, PTexPage>();
            for(int i = 0 ; i < maxPages; ++i)
            {
                physicalPageQueue.Add(new PTexPage(x, y, pageX, pageY));
                x += textelsPerPhysicalPage;
                ++pageX;
                if (x + textelsPerPhysicalPage >= physicalTextureSize.Width)
                {
                    x = 0;
                    y += textelsPerPhysicalPage;
                    pageX = 0;
                    ++pageY;
                    if (y + textelsPerPhysicalPage > physicalTextureSize.Height)
                    {
                        break;
                    }
                }
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                cancelBackgroundLoad = true;
                textureCache.Dispose();
                foreach (var stagingImage in stagingImages)
                {
                    stagingImage.Dispose();
                }
            }
        }

        public void addedPhysicalTexture(PhysicalTexture physicalTexture)
        {
            if(stagingImageCount < stagingImages.Length)
            {
                stagingImages[stagingImageCount++] = new StagingImage(textelsPerPhysicalPage, physicalTexture.TextureFormat);
            }
        }

        public void beginPageUpdate()
        {
            //Note clearing these forces us to make sure we fully update each frame, or copy all this to another processing queue
            addedPages.Clear();
            removedPages.Clear();
        }

        public void addRequestedPage(VTexPage page)
        {
            addedPages.Add(page);
        }

        public void removeRequestedPage(VTexPage page)
        {
            addedPages.Remove(page);
            removedPages.Add(page);
        }

        public void updatePagesFromRequests()
        {
            //If there are no added pages, there is nothing to do with this call, just return
            if(addedPages.Count == 0)
            {
                return;
            }

            //Finish any current image loading and wait until that is complete
            if (loadingTask != null)
            {
                stopLoading = true;
                loadingTask.Wait();
            }

            //Careful with order, we want to make sure the loadingtask is done before getting a lock again since that function locks for its duration.
            lock (this)
            {
                //We have a lock now, are we still ok to load?
                if (cancelBackgroundLoad)
                {
                    throw new CancelThreadException();
                }
            }

            //Reset
            stopLoading = false;

            //Remove pages
            PerformanceMonitor.start("updatePagesFromRequests remove");
            foreach (var page in removedPages)
            {
                PTexPage pTexPage;
                if (usedPhysicalPages.TryGetValue(page, out pTexPage))
                {
                    physicalPageQueue.Add(pTexPage);
                    physicalPagePool.Add(page, pTexPage);
                    usedPhysicalPages.Remove(page);
                }
                else
                {
                    pagesToLoad.Remove(page);
                }
            }
            PerformanceMonitor.stop("updatePagesFromRequests remove");

            //Start loading task again
            pagesToLoad.AddRange(addedPages.Where(p => !usedPhysicalPages.ContainsKey(p) && !pagesToLoad.Contains(p))); //Add all new pages that are not already used, could potentailly be slow, can second check be replaced by a hash map
            pagesToLoad.Sort((v1, v2) => v1.GetHashCode() - v2.GetHashCode()); //When sorting can we prioritize pages that have already been loaded?
            loadingTask = Task.Run(() =>
                {
                    lock(this)
                    {
                        PerformanceMonitor.start("updatePagesFromRequests processing pages");
                        for (int i = pagesToLoad.Count - 1; i > -1; --i) //Process backwards, try to avoid as many collection element shifts as possible
                        {
                            if (processPage(pagesToLoad[i]))
                            {
                                pagesToLoad.RemoveAt(i);
                            }
                            if (stopLoading)
                            {
                                break;
                            }
                        }
                        PerformanceMonitor.stop("updatePagesFromRequests processing pages");
                    }
                });
        }

        private bool processPage(VTexPage page)
        {
            bool added = false;
            //First see if we still have that page in our virtual texture pool
            PTexPage pTexPage;
            if (physicalPagePool.TryGetValue(page, out pTexPage))
            {
                physicalPageQueue.Remove(pTexPage);
                physicalPagePool.Remove(page);
                usedPhysicalPages.Add(page, pTexPage);
                added = true;
            }
            else if (physicalPageQueue.Count > 0) //Do we have pages available
            {
                pTexPage = physicalPageQueue[0]; //The physical page candidate, do not modify before usedPhysicalPages if statement below
                if (loadImages(page, pTexPage))
                {
                    //Alert old texture of removal if there was one, Do not modify pTexPage above this if block, we need the old data
                    IndirectionTexture oldIndirectionTexture = null;
                    if (pTexPage.VirtualTexturePage != null)
                    {
                        if (virtualTextureManager.getIndirectionTexture(pTexPage.VirtualTexturePage.indirectionTexId, out oldIndirectionTexture))
                        {
                            oldIndirectionTexture.removePhysicalPage(pTexPage);
                        }

                        physicalPagePool.Remove(pTexPage.VirtualTexturePage); //Be sure to remove the page from the pool if it was used previously
                    }

                    physicalPageQueue.RemoveAt(0);
                    pTexPage.VirtualTexturePage = page;
                    usedPhysicalPages.Add(page, pTexPage);

                    //Add to new indirection texture
                    IndirectionTexture newIndirectionTex;
                    if (virtualTextureManager.getIndirectionTexture(page.indirectionTexId, out newIndirectionTex))
                    {
                        newIndirectionTex.addPhysicalPage(pTexPage);

                        //Very important to wait here, we don't want to update any buffers multiple times
                        //Also note that we give up our lock here, which allows this class to be disposed if appropriate
                        System.Threading.Monitor.Exit(this);
                        ThreadManager.invokeAndWait(() => 
                            {
                                if(oldIndirectionTexture != null) //If we changed the old texture
                                {
                                    oldIndirectionTexture.uploadPageChanges();
                                }
                                if (oldIndirectionTexture != newIndirectionTex) //If the old texture and new texture are not the same
                                {
                                    newIndirectionTex.uploadPageChanges();
                                }
                            });
                        System.Threading.Monitor.Enter(this);
                        if(cancelBackgroundLoad) //Reaquired lock, are we still active
                        {
                            throw new CancelThreadException();
                        }
                    }
                    added = true;
                }
            }
            return added;
        }

        internal Vector2 PagePaddingScale { get; private set; }

        internal Vector2 PagePaddingOffset { get; private set; }

        Stopwatch sw = new Stopwatch();

        /// <summary>
        /// Load the given image. Note that pTexPage is constant for the duration of this function call
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pTexPage"></param>
        /// <returns></returns>
        private bool loadImages(VTexPage page, PTexPage pTexPage)
        {
            int stagingImageIndex = 0;
            bool usedPhysicalPage = false;
            IndirectionTexture indirectionTexture;
            if (virtualTextureManager.getIndirectionTexture(page.indirectionTexId, out indirectionTexture))
            {
                //Fire off image loading and blitting tasks
                foreach (var textureUnit in indirectionTexture.OriginalTextures)
                {
                    copyTostagingImageTasks[stagingImageIndex] = fireCopyToStaging(page, stagingImageIndex, indirectionTexture, textureUnit);
                    ++stagingImageIndex;
                }
                //Wait for results
                for (int i = 0; i < stagingImageIndex; ++i)
                {
                    copyTostagingImageTasks[i].Wait();
                    if (copyTostagingImageTasks[i].Result)
                    {
                        usedPhysicalPage = true;
                    }
                }

                //Single threaded
                //foreach (var textureUnit in indirectionTexture.OriginalTextures)
                //{
                //    if (copyToStaging(page, stagingImageIndex++, indirectionTexture, textureUnit))
                //    {
                //        usedPhysicalPage = true;
                //    }
                //}

                //Sync back to main thread
                ThreadManager.invoke(() => //We are safe not to wait on this invoke since we know we will be waiting in processpage
                    {
                        var dest = new IntRect(pTexPage.x, pTexPage.y, textelsPerPhysicalPage, textelsPerPhysicalPage);
                        for (int u = 0; u < stagingImageIndex; ++u)
                        {
                            stagingImages[u].copyToGpu(dest);
                        }
                    });
            }
            return usedPhysicalPage;
        }

        private Task<bool> fireCopyToStaging(VTexPage page, int stagingImageIndex, IndirectionTexture indirectionTexture, OriginalTextureInfo textureUnit)
        {
            return Task.Run(() => copyToStaging(page, stagingImageIndex, indirectionTexture, textureUnit));
        }

        private bool copyToStaging(VTexPage page, int stagingImageIndex, IndirectionTexture indirectionTexture, OriginalTextureInfo textureUnit)
        {
            bool usedPhysicalPage = false;

            if (page.mip >= textureUnit.MipOffset)
            {
                //Load or grab from cache
                String textureName = String.Format("{0}_{1}", textureUnit.TextureFileName, indirectionTexture.RealTextureSize.Width >> page.mip);
                using (TextureCacheHandle cacheHandle = getImage(page, indirectionTexture, textureUnit, textureName))
                {
                    //Blit
                    PixelBox sourceBox = null;
                    try
                    {
                        int mipCount = cacheHandle.Image.NumMipmaps;
                        if (mipCount == 0) //We always have to take from the largest size
                        {
                            sourceBox = cacheHandle.Image.getPixelBox(0, 0);
                        }
                        else
                        {
                            sourceBox = cacheHandle.Image.getPixelBox(0, (uint)(page.mip - textureUnit.MipOffset));
                        }
                        IntSize2 largestSupportedPageIndex = indirectionTexture.NumPages;
                        largestSupportedPageIndex.Width >>= page.mip;
                        largestSupportedPageIndex.Height >>= page.mip;
                        if (page.x != 0 && page.y != 0 && page.x + 1 != largestSupportedPageIndex.Width && page.y + 1 != largestSupportedPageIndex.Height)
                        {
                            sourceBox.Rect = new IntRect(page.x * textelsPerPage - padding, page.y * textelsPerPage - padding, textelsPerPage + padding2, textelsPerPage + padding2);
                        }
                        else
                        {
                            sourceBox.Rect = new IntRect(page.x * textelsPerPage, page.y * textelsPerPage, textelsPerPage, textelsPerPage);
                        }
                        stagingImages[stagingImageIndex].setData(sourceBox, virtualTextureManager.getPhysicalTexture(textureUnit.TextureUnit), padding);
                        usedPhysicalPage = true;
                    }
                    finally
                    {
                        if (sourceBox != null)
                        {
                            sourceBox.Dispose();
                        }
                    }
                }
            }
            return usedPhysicalPage;
        }

        private TextureCacheHandle getImage(VTexPage page, IndirectionTexture indirectionTexture, OriginalTextureInfo textureUnit, String textureName)
        {
            TextureCacheHandle cacheHandle;
            if (!textureCache.TryGetValue(textureName, out cacheHandle))
            {
                //Try to direct load smaller version
                String file = textureUnit.TextureFileName;
                String extension = Path.GetExtension(file);
                String directFile = textureUnit.TextureFileName.Substring(0, file.Length - extension.Length);
                directFile = String.Format("{0}_{1}{2}", directFile, indirectionTexture.RealTextureSize.Width >> page.mip, extension);
                if (VirtualFileSystem.Instance.exists(directFile))
                {
                    cacheHandle = doLoadImage(textureName, extension, directFile);
                }
                else
                {
                    //Try to get full size image from cache
                    String fullSizeName = String.Format("{0}_{1}", textureUnit.TextureFileName, indirectionTexture.RealTextureSize.Width);
                    if (!textureCache.TryGetValue(fullSizeName, out cacheHandle))
                    {
                        cacheHandle = doLoadImage(fullSizeName, extension, textureUnit.TextureFileName);
                    }

                    //If we aren't mip 0 resize accordingly
                    if (page.mip > cacheHandle.Image.NumMipmaps && page.mip != 0)
                    {
                        using (TextureCacheHandle originalHandle = cacheHandle)
                        {
                            Image original = originalHandle.Image;
                            Image image = new Image(original.Width >> page.mip, original.Height >> page.mip, original.Depth, original.Format, original.NumFaces, original.NumMipmaps);
                            using (var src = original.getPixelBox())
                            {
                                using (var dest = image.getPixelBox())
                                {
                                    Image.Scale(src, dest, Image.Filter.FILTER_BILINEAR);
                                }
                            }
                            cacheHandle = textureCache.Add(textureName, image);
                        }
                    }
                }
            }
            return cacheHandle;
        }

        private TextureCacheHandle doLoadImage(String cachedName, String extension, String file)
        {
            sw.Reset();
            sw.Start();
            var image = new Image();
            using (Stream stream = VirtualFileSystem.Instance.openStream(file, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
            {
                if (extension.Length > 0)
                {
                    extension = extension.Substring(1);
                }
                image.load(stream, extension);
            }
            var handle = textureCache.Add(cachedName, image);
            sw.Stop();
            Logging.Log.Debug("Loaded image {0} in {1} ms", file, sw.ElapsedMilliseconds);
            return handle;
        }
    }
}
