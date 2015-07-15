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
        private List<VTexPage> oversubscribedPages; //The llist of pages that could not load because we were oversubscribed

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

        private List<StagingImage> stagingImages = new List<StagingImage>(4);
        private TextureCache textureCache = new TextureCache();

        public TextureLoader(VirtualTextureManager virtualTextureManager, IntSize2 physicalTextureSize, int textelsPerPage, int padding)
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
            oversubscribedPages = new List<VTexPage>(10);

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

            for (int i = 0; i < stagingImages.Capacity; ++i)
            {
                stagingImages.Add(new StagingImage(textelsPerPhysicalPage, virtualTextureManager.PhysicalTextureFormat));
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
            try
            {
                System.Threading.Monitor.Enter(this);
                if(cancelBackgroundLoad) //We have a lock now, are we still ok to load?
                {
                    throw new CancelThreadException();
                }

                PerformanceMonitor.start("updatePagesFromRequests remove");
                //Remove pages
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
                        oversubscribedPages.Remove(page);
                    }
                }
                PerformanceMonitor.stop("updatePagesFromRequests remove");

                PerformanceMonitor.start("updatePagesFromRequests oversubscribedPages");
                oversubscribedPages.Sort(VTexPage.Sort);

                for (int i = 0; i < oversubscribedPages.Count; )
                {
                    var page = oversubscribedPages[i];
                    if (processPage(page, false))
                    {
                        oversubscribedPages.RemoveAt(i);
                    }
                    else
                    {
                        ++i;
                    }
                }
                PerformanceMonitor.stop("updatePagesFromRequests oversubscribedPages");

                PerformanceMonitor.start("updatePagesFromRequests add");
                //Add Pages
                var added = addedPages.ToList(); //OMG the garbage
                added.Sort(VTexPage.Sort);
                foreach (var page in added)
                {
                    processPage(page, true);
                }
                PerformanceMonitor.stop("updatePagesFromRequests add");
            }
            finally
            {
                System.Threading.Monitor.Exit(this);
            }
        }

        private bool processPage(VTexPage page, bool addToOversubscribe)
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
            else if (addToOversubscribe)
            {
                oversubscribedPages.Add(page);
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
            int i = 0;
            bool usedPhysicalPage = false;
            IndirectionTexture indirectionTexture;
            if (virtualTextureManager.getIndirectionTexture(page.indirectionTexId, out indirectionTexture))
            {
                foreach (var textureUnit in indirectionTexture.OriginalTextures)
                {
                    //Load or grab from cache
                    String textureName = String.Format("{0}_{1}", textureUnit.Value, indirectionTexture.RealTextureSize.Width >> page.mip);
                    Image image;
                    if (!textureCache.TryGetValue(textureName, out image))
                    {
                        //Try to direct load smaller version
                        String file = textureUnit.Value;
                        String extension = Path.GetExtension(file);
                        String directFile = textureUnit.Value.Substring(0, file.Length - extension.Length);
                        directFile = String.Format("{0}_{1}{2}", directFile, indirectionTexture.RealTextureSize.Width >> page.mip, extension);
                        if (VirtualFileSystem.Instance.exists(directFile))
                        {
                            //Logging.Log.Debug("Loading image {0}", directFile);
                            doLoadImage(textureName, extension, directFile, out image);
                        }
                        else
                        {
                            //Try to get full size image from cache
                            String fullSizeName = String.Format("{0}_{1}", textureUnit.Value, indirectionTexture.RealTextureSize.Width);
                            if (!textureCache.TryGetValue(fullSizeName, out image))
                            {
                                doLoadImage(fullSizeName, extension, textureUnit.Value, out image);
                                //Logging.Log.Debug("Loading image {0}", textureUnit.Value);
                                //image = new Image();
                                //using (Stream stream = VirtualFileSystem.Instance.openStream(textureUnit.Value, Engine.Resources.FileMode.Open))
                                //{
                                //    if (extension.Length > 0)
                                //    {
                                //        extension = extension.Substring(1);
                                //    }
                                //    image.load(stream, extension);
                                //}
                                //textureCache.Add(fullSizeName, image);
                            }

                            //If we aren't mip 0 resize accordingly
                            if (page.mip != 0)
                            {
                                Image original = image;
                                image = new Image(image.Width >> page.mip, image.Height >> page.mip, original.Depth, original.Format, original.NumFaces, original.NumMipmaps);
                                using (var src = original.getPixelBox())
                                {
                                    using (var dest = image.getPixelBox())
                                    {
                                        Image.Scale(src, dest, Image.Filter.FILTER_BILINEAR);
                                    }
                                }
                                textureCache.Add(textureName, image);
                            }
                        }
                    }

                    //Blit
                    int mipCount = image.NumMipmaps;
                    if (mipCount == 0) //We always have to take from the largest size
                    {
                        IntSize2 largestSupportedPageIndex = indirectionTexture.NumPages;
                        largestSupportedPageIndex.Width >>= page.mip;
                        largestSupportedPageIndex.Height >>= page.mip;
                        using (PixelBox sourceBox = image.getPixelBox(0, 0))
                        {
                            if (page.x != 0 && page.y != 0 && page.x + 1 != largestSupportedPageIndex.Width && page.y + 1 != largestSupportedPageIndex.Height)
                            {
                                sourceBox.Rect = new IntRect(page.x * textelsPerPage - padding, page.y * textelsPerPage - padding, textelsPerPage + padding2, textelsPerPage + padding2);
                            }
                            else
                            {
                                sourceBox.Rect = new IntRect(page.x * textelsPerPage, page.y * textelsPerPage, textelsPerPage, textelsPerPage);
                            }
                            stagingImages[i].setData(sourceBox, virtualTextureManager.getPhysicalTexture(textureUnit.Key), padding);
                            usedPhysicalPage = true; //We finish marking the physical page used below, this part loops multiple times
                        }
                    }
                    ++i;
                }
                ThreadManager.invoke(() => //We are safe not to wait on this invoke since we know we will be waiting in processpage
                    {
                        var dest = new IntRect(pTexPage.x, pTexPage.y, textelsPerPhysicalPage, textelsPerPhysicalPage);
                        for (int u = 0; u < i; ++u)
                        {
                            stagingImages[u].copyToGpu(dest);
                        }
                    });
            }
            return usedPhysicalPage;
        }

        private void doLoadImage(String cachedName, String extension, String file, out Image image)
        {
            sw.Reset();
            sw.Start();
            image = new Image();
            using (Stream stream = VirtualFileSystem.Instance.openStream(file, Engine.Resources.FileMode.Open))
            {
                if (extension.Length > 0)
                {
                    extension = extension.Substring(1);
                }
                image.load(stream, extension);
            }
            textureCache.Add(cachedName, image);
            sw.Stop();
            Logging.Log.Debug("Loaded image {0} in {1} ms", file, sw.ElapsedMilliseconds);
        }
    }
}
