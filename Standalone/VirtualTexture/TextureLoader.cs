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
        private List<VTexPage> requestedPages;
        private List<VTexPage> addedPages;
        private List<VTexPage> removedPages;
        private List<VTexPage> visitedPages;
        private List<VTexPage> loadedPages;
        private List<VTexPage> unloadedPages;

        private List<PTexPage> pooledPhysicalPages; //FIFO queue for used pages, allows us to reuse pages if they are requested again quickly and keep track of what parts of the physical texture we can use
        private List<PTexPage> usedPhysicalPages;

        private VirtualTextureManager virtualTextureManager;
        private int maxPages;
        private int textelsPerPage;

        public TextureLoader(VirtualTextureManager virtualTextureManager, IntSize2 physicalTextureSize, int textelsPerPage)
        {
            this.virtualTextureManager = virtualTextureManager;
            IntSize2 pageTableSize = physicalTextureSize / textelsPerPage;
            maxPages = pageTableSize.Width * pageTableSize.Height;
            this.textelsPerPage = textelsPerPage;

            loadedPages = new List<VTexPage>(maxPages);
            requestedPages = new List<VTexPage>(maxPages);
            addedPages = new List<VTexPage>();
            removedPages = new List<VTexPage>();
            visitedPages = new List<VTexPage>();
            unloadedPages = new List<VTexPage>();

            //Build pool
            int x = 0;
            int y = 0;
            pooledPhysicalPages = new List<PTexPage>(maxPages);
            usedPhysicalPages = new List<PTexPage>(maxPages);
            for(int i = 0 ; i < maxPages; ++i)
            {
                pooledPhysicalPages.Add(new PTexPage(x, y));
                x += textelsPerPage;
                if(x >= physicalTextureSize.Width)
                {
                    x = 0;
                    y += textelsPerPage;
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
                        if (!addedPages.Contains(page))
                        {
                            addedPages.Add(page);
                        }
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
                for(int i = 0; i < usedPhysicalPages.Count; ++i)
                {
                    if(usedPhysicalPages[i].VirtualTexturePage == page)
                    {
                        pooledPhysicalPages.Add(usedPhysicalPages[i]);
                        usedPhysicalPages.RemoveAt(i);
                        break; //Get out of loop
                    }
                }
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
                            usedPhysicalPages.Add(pooledPhysicalPages[i]);
                            pooledPhysicalPages.RemoveAt(i);
                            break;
                        }
                    }
                    if (needsLoad) //We do need to load
                    {
                        if (pooledPhysicalPages.Count > 0) //Do we have pages available
                        {
                            bool usedPhysicalPage = false;
                            var pTexPage = pooledPhysicalPages[0];
                            IndirectionTexture indirectionTexture;
                            if (virtualTextureManager.getIndirectionTexture(page.indirectionTexId, out indirectionTexture))
                            {
                                foreach (var textureUnit in indirectionTexture.OriginalTextures)
                                {
                                    using (var originalTexture = TextureManager.getInstance().getByName(textureUnit.Value))
                                    {
                                        int mipCount = originalTexture.Value.NumMipmaps;
                                        var srcRect = new IntRect(page.x * textelsPerPage, page.y * textelsPerPage, textelsPerPage, textelsPerPage);
                                        if (page.mip < mipCount && srcRect.Right <= originalTexture.Value.Width >> page.mip && srcRect.Bottom <= originalTexture.Value.Height >> page.mip)
                                        {
                                            using (var sourceBuffer = originalTexture.Value.getBuffer(0, (uint)page.mip))
                                            {
                                                var physicalTexture = virtualTextureManager.getPhysicalTexture(textureUnit.Key);
                                                physicalTexture.addPage(sourceBuffer, srcRect, new IntRect(pTexPage.x, pTexPage.y, textelsPerPage, textelsPerPage));
                                                usedPhysicalPage = true; //We finish marking the physical page used below, this part loops multiple times
                                            }
                                        }
                                    }
                                }
                            }
                            if(usedPhysicalPage)
                            {
                                pTexPage.VirtualTexturePage = page;
                                pooledPhysicalPages.RemoveAt(0);
                                usedPhysicalPages.Add(pTexPage);
                                IndirectionTexture indirectionTex;
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
