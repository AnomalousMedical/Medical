using Engine;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class TextureLoader
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

        public TextureLoader(VirtualTextureManager virtualTextureManager, IntSize2 physicalTextureSize, int textelsPerPage, int padding)
        {
            this.virtualTextureManager = virtualTextureManager;
            IntSize2 pageTableSize = physicalTextureSize / textelsPerPage;
            maxPages = pageTableSize.Width * pageTableSize.Height;
            this.textelsPerPage = textelsPerPage;
            this.padding = padding;
            this.padding2 = padding * 2;
            this.textelsPerPhysicalPage = textelsPerPage + padding2;

            addedPages = new HashSet<VTexPage>();
            removedPages = new List<VTexPage>();

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
            PerformanceMonitor.start("updatePagesFromRequests remove");
            //Remove pages
            foreach(var page in removedPages)
            {
                PTexPage pTexPage;
                if (usedPhysicalPages.TryGetValue(page, out pTexPage))
                {
                    physicalPageQueue.Add(pTexPage);
                    physicalPagePool.Add(page, pTexPage);
                    usedPhysicalPages.Remove(page);
                }
            }
            PerformanceMonitor.stop("updatePagesFromRequests remove");

            PerformanceMonitor.start("updatePagesFromRequests add");
            //Add Pages
            foreach(var page in addedPages)
            {
                //First see if we still have that page in our virtual texture pool
                PTexPage pTexPage;
                if(physicalPagePool.TryGetValue(page, out pTexPage))
                {
                    physicalPageQueue.Remove(pTexPage);
                    physicalPagePool.Remove(page);
                    usedPhysicalPages.Add(page, pTexPage);
                }
                else if (physicalPageQueue.Count > 0) //Do we have pages available
                {
                    pTexPage = physicalPageQueue[0]; //The physical page candidate, do not modify before usedPhysicalPages if statement below
                    if (loadImage(page, pTexPage))
                    {
                        IndirectionTexture indirectionTex;
                        //Alert old texture of removal if there was one, Do not modify pTexPage above this if block, we need the old data
                        if (pTexPage.VirtualTexturePage != null)
                        {
                            if (virtualTextureManager.getIndirectionTexture(pTexPage.VirtualTexturePage.indirectionTexId, out indirectionTex))
                            {
                                indirectionTex.removePhysicalPage(pTexPage);
                            }

                            physicalPagePool.Remove(pTexPage.VirtualTexturePage); //Be sure to remove the page from the pool if it was used previously
                        }

                        physicalPageQueue.RemoveAt(0);
                        pTexPage.VirtualTexturePage = page;
                        usedPhysicalPages.Add(page, pTexPage);

                        //Add to new indirection texture
                        if (virtualTextureManager.getIndirectionTexture(page.indirectionTexId, out indirectionTex))
                        {
                            indirectionTex.addPhysicalPage(pTexPage);
                        }
                    }
                }
                else
                {
                    Logging.Log.Debug("Ran out of texture space");
                }
            }
            PerformanceMonitor.stop("updatePagesFromRequests add");
        }

        /// <summary>
        /// Load the given image. Note that pTexPage is constant for the duration of this function call
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pTexPage"></param>
        /// <returns></returns>
        private bool loadImage(VTexPage page, PTexPage pTexPage)
        {
            bool usedPhysicalPage = false;
            IndirectionTexture indirectionTexture;
            if (virtualTextureManager.getIndirectionTexture(page.indirectionTexId, out indirectionTexture))
            {
                foreach (var textureUnit in indirectionTexture.OriginalTextures)
                {
                    using (var originalTexture = TextureManager.getInstance().getByName(textureUnit.Value))
                    {
                        int mipCount = originalTexture.Value.NumMipmaps;
                        var srcRect = new IntRect(page.x * textelsPerPage, page.y * textelsPerPage, textelsPerPage, textelsPerPage);
                        uint mipWidth = originalTexture.Value.Width >> page.mip;
                        uint mipHeight = originalTexture.Value.Height >> page.mip;
                        if (page.mip < mipCount && srcRect.Right <= mipWidth && srcRect.Bottom <= mipHeight)
                        {
                            using (var sourceBuffer = originalTexture.Value.getBuffer(0, (uint)page.mip))
                            {
                                if (srcRect.Right + padding2 < mipWidth && srcRect.Bottom + padding2 < mipHeight)
                                {
                                    //Can expand to fit, most common case
                                    if (srcRect.Left > 0)
                                    {
                                        srcRect.Left -= 1;
                                    }
                                    if (srcRect.Top > 0)
                                    {
                                        srcRect.Top -= 1;
                                    }
                                    srcRect.Width = textelsPerPhysicalPage;
                                    srcRect.Height = textelsPerPhysicalPage;

                                    var physicalTexture = virtualTextureManager.getPhysicalTexture(textureUnit.Key);
                                    physicalTexture.addPage(sourceBuffer, srcRect, new IntRect(pTexPage.x, pTexPage.y, textelsPerPhysicalPage, textelsPerPhysicalPage));
                                    usedPhysicalPage = true; //We finish marking the physical page used below, this part loops multiple times
                                }
                                else if (srcRect.Width == mipWidth && srcRect.Height == mipHeight)
                                {
                                    //Mip level is the same as the page size, pad the left and right side with duplicate data
                                    var physicalTexture = virtualTextureManager.getPhysicalTexture(textureUnit.Key);

                                    //Write center
                                    IntRect centerDest = new IntRect(pTexPage.x + padding, pTexPage.y + padding, textelsPerPage, textelsPerPage);
                                    physicalTexture.addPage(sourceBuffer, srcRect, centerDest);

                                    //Write left padding
                                    IntRect paddingStripSrc = new IntRect(srcRect.Left, srcRect.Top, padding, textelsPerPage);
                                    IntRect paddingStripDest = new IntRect(pTexPage.x, pTexPage.y + 1, padding, textelsPerPage);
                                    physicalTexture.addPage(sourceBuffer, paddingStripSrc, paddingStripDest);

                                    //Write right padding
                                    paddingStripSrc.Left = srcRect.Right - padding;
                                    paddingStripDest.Left = pTexPage.x + textelsPerPhysicalPage - padding;
                                    physicalTexture.addPage(sourceBuffer, paddingStripSrc, paddingStripDest);

                                    //Write Top padding
                                    paddingStripSrc.Left = srcRect.Left;
                                    paddingStripSrc.Width = textelsPerPage;
                                    paddingStripSrc.Height = padding;

                                    paddingStripDest.Left = pTexPage.x + 1;
                                    paddingStripDest.Top = 0;
                                    paddingStripDest.Width = textelsPerPage;
                                    paddingStripDest.Height = padding;
                                    physicalTexture.addPage(sourceBuffer, paddingStripSrc, paddingStripDest);

                                    //Write bottom padding
                                    paddingStripSrc.Top = srcRect.Bottom - padding;
                                    paddingStripDest.Top = pTexPage.y + textelsPerPhysicalPage - padding;
                                    physicalTexture.addPage(sourceBuffer, paddingStripSrc, paddingStripDest);

                                    usedPhysicalPage = true; //We finish marking the physical page used below, this part loops multiple times
                                }
                                else
                                {
                                    Logging.Log.Debug("Cannot load page {0}.", page);
                                }
                            }
                        }
                    }
                }
            }
            return usedPhysicalPage;
        }
    }
}
