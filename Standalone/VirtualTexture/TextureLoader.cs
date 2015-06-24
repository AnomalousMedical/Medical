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
        private HashSet<VTexPage> requestedPages;
        private HashSet<VTexPage> addedPages;
        private List<VTexPage> removedPages;
        private HashSet<VTexPage> visitedPages;
        private HashSet<VTexPage> loadedPages;
        private HashSet<VTexPage> unloadedPages;

        private List<PTexPage> pooledPhysicalPages; //FIFO queue for used pages, allows us to reuse pages if they are requested again quickly and keep track of what parts of the physical texture we can use
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

            loadedPages = new HashSet<VTexPage>();
            requestedPages = new HashSet<VTexPage>();
            addedPages = new HashSet<VTexPage>();
            removedPages = new List<VTexPage>();
            visitedPages = new HashSet<VTexPage>();
            unloadedPages = new HashSet<VTexPage>();

            //Build pool
            int x = 0;
            int y = 0;
            int pageX = 0;
            int pageY = 0;
            pooledPhysicalPages = new List<PTexPage>(maxPages);
            usedPhysicalPages = new Dictionary<VTexPage, PTexPage>();
            for(int i = 0 ; i < maxPages; ++i)
            {
                pooledPhysicalPages.Add(new PTexPage(x, y, pageX, pageY));
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

        public void findNewPages()
        {
            addedPages.Clear();
            removedPages.Clear();
            visitedPages.Clear();
            unloadedPages.Clear();

            //Find all active pages
            foreach (var indirectionTex in virtualTextureManager.IndirectionTextures)
            {
                foreach (var page in indirectionTex.ActivePages)
                {
                    if(!requestedPages.Contains(page))
                    {
                        addedPages.Add(page);
                    }
                    else
                    {
                        visitedPages.Add(page);
                    }
                }
            }

            //Update our buffers
            foreach (var page in requestedPages)
            {
                if (!visitedPages.Contains(page))
                {
                    removedPages.Add(page);
                }
            }

            foreach (var page in removedPages)
            {
                requestedPages.Remove(page);
            }

            foreach (var page in addedPages)
            {
                requestedPages.Add(page);
            }

            //Unload pages that are no longer requested
            foreach (var page in loadedPages)
            {
                if (!requestedPages.Contains(page))
                {
                    unloadedPages.Add(page);
                }
            }

            foreach(var page in unloadedPages)
            {
                //Unload the page, all we do is mark it unused, we might need it again soon
                loadedPages.Remove(page);
                PTexPage pTexPage;
                if(usedPhysicalPages.TryGetValue(page, out pTexPage))
                {
                    pooledPhysicalPages.Add(pTexPage);
                    usedPhysicalPages.Remove(page);
                }
                //for(int i = 0; i < usedPhysicalPages.Count; ++i)
                //{
                //    if(usedPhysicalPages[i].VirtualTexturePage == page)
                //    {
                //        pooledPhysicalPages.Add(usedPhysicalPages[i]);
                //        usedPhysicalPages.RemoveAt(i);
                //        break; //Get out of loop
                //    }
                //}
            }

            //Load in new requested pages
            foreach(var page in requestedPages)
            {
                if(!loadedPages.Contains(page))
                {
                    //Load page into texture
                    loadedPages.Add(page);
                    //First see if we still have that page in our pool
                    bool needsLoad = true;
                    for(int i = 0; i < pooledPhysicalPages.Count; ++i)
                    {
                        if(pooledPhysicalPages[i].VirtualTexturePage == page)
                        {
                            needsLoad = false;
                            usedPhysicalPages.Add(page, pooledPhysicalPages[i]);
                            pooledPhysicalPages.RemoveAt(i);
                            break;
                        }
                    }
                    if (needsLoad) //We do need to load
                    {
                        if (pooledPhysicalPages.Count > 0) //Do we have pages available
                        {
                            bool usedPhysicalPage = false;
                            var pTexPage = pooledPhysicalPages[0]; //The physical page candidate, do not modify before usedPhysicalPages if statement below
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
                                            }
                                        }
                                    }
                                }
                            }
                            if(usedPhysicalPage)
                            {
                                IndirectionTexture indirectionTex;
                                //Alert old texture of removal if there was one, Do not modify pTexPage above this if block, we need the old data
                                if (pTexPage.VirtualTexturePage != null && virtualTextureManager.getIndirectionTexture(pTexPage.VirtualTexturePage.indirectionTexId, out indirectionTex))
                                {
                                    indirectionTex.removePhysicalPage(pTexPage);
                                }

                                pTexPage.VirtualTexturePage = page;
                                pooledPhysicalPages.RemoveAt(0);
                                usedPhysicalPages.Add(page, pTexPage);

                                //Add to new indirection texture
                                if(virtualTextureManager.getIndirectionTexture(page.indirectionTexId, out indirectionTex))
                                {
                                    indirectionTex.addPhysicalPage(pTexPage);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
