using Engine;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Medical
{
    class IndirectionTexture : IDisposable
    {
        static int currentId = 0;
        static int maxId = 254;
        static HashSet<int> usedIds = new HashSet<int>();
        static int generateId()
        {
            lock (usedIds)
            {
                while (usedIds.Contains(currentId))
                {
                    incrementCurrentId();
                }
                int retVal = currentId;
                incrementCurrentId();
                return retVal;
            }
        }

        static void incrementCurrentId()
        {
            currentId = (currentId + 1) % maxId;
        }

        private int id = generateId();
        private IntSize2 realTextureSize;
        private TexturePtr indirectionTexture;
        private VirtualTextureManager virtualTextureManager;
        private IntSize2 numPages;
        private int highestMip = 0; //The highest mip level that does not fall below one page in size
        private FreeImageAPI.FreeImageBitmap[] fiBitmap; //Can we do this without this bitmap? (might be ok to keep, but will be using 2x as much memory, however, allows for background modification, could even double buffer)
        private HardwarePixelBufferSharedPtr[] buffer;
        private PixelBox[] pixelBox;

        private List<VTexPage> activePages = new List<VTexPage>();
        private List<VTexPage> visibleThisUpdate = new List<VTexPage>();
        private List<VTexPage> removedPages = new List<VTexPage>();
        private List<VTexPage> addedPages = new List<VTexPage>();
        private bool updateTextureOnApply = false;
        private Dictionary<String, String> originalTextureUnits;

        public IndirectionTexture(String materialSetKey, IntSize2 realTextureSize, int texelsPerPage, VirtualTextureManager virtualTextureManager)
        {
            this.virtualTextureManager = virtualTextureManager;
            this.realTextureSize = realTextureSize;
            numPages = realTextureSize / texelsPerPage;
            for (highestMip = 0; realTextureSize.Width >> highestMip > texelsPerPage && realTextureSize.Height >> highestMip > texelsPerPage; ++highestMip) { }
            indirectionTexture = TextureManager.getInstance().createManual(String.Format("{0}_IndirectionTexture_{1}", materialSetKey, id), VirtualTextureManager.ResourceGroup, TextureType.TEX_TYPE_2D,
                (uint)numPages.Width, (uint)numPages.Height, 1, highestMip, PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, null, false, 0);

            fiBitmap = new FreeImageAPI.FreeImageBitmap[highestMip];
            buffer = new HardwarePixelBufferSharedPtr[highestMip];
            pixelBox = new PixelBox[highestMip];

            for (int i = 0; i < highestMip; ++i)
            {
                fiBitmap[i] = new FreeImageAPI.FreeImageBitmap((int)indirectionTexture.Value.Width >> i, (int)indirectionTexture.Value.Height >> i, FreeImageAPI.PixelFormat.Format32bppArgb);
                buffer[i] = indirectionTexture.Value.getBuffer(0, (uint)i);
                unsafe
                {
                    pixelBox[i] = new PixelBox(0, 0, fiBitmap[i].Width, fiBitmap[i].Height, OgreDrawingUtility.getOgreFormat(fiBitmap[i].PixelFormat), fiBitmap[i].GetScanlinePointer(0).ToPointer());
                }
            }
        }

        public void Dispose()
        {
            indirectionTexture.Dispose();
            for (int i = 0; i < highestMip; ++i )
            {
                pixelBox[i].Dispose();
                buffer[i].Dispose();
                fiBitmap[i].Dispose();
            }
        }

        public void reconfigureTechnique(Technique mainTechnique, Technique feedbackTechnique)
        {
            bool setupOriginalTextureUnits = originalTextureUnits == null;
            if(setupOriginalTextureUnits)
            {
                originalTextureUnits = new Dictionary<string, string>();
            }
            int numPasses = mainTechnique.getNumPasses();
            for(ushort i = 0; i < numPasses; ++i)
            {
                var pass = mainTechnique.getPass(i);
                ushort numTextureUnits = pass.getNumTextureUnitStates();
                for(ushort t = 0; t < numTextureUnits; ++t)
                {
                    var texUnit = pass.getTextureUnitState(t);
                    if (setupOriginalTextureUnits)
                    {
                        originalTextureUnits[texUnit.Name] = texUnit.TextureName;
                    }
                    texUnit.TextureName = virtualTextureManager.getPhysicalTexture(texUnit.Name).TextureName;
                    texUnit.setFilteringOptions(FilterOptions.Point, FilterOptions.Point, FilterOptions.None);
                }
                var indirectionTexturePass = pass.createTextureUnitState(indirectionTexture.Value.getName()); //Add indirection texture
                indirectionTexturePass.setFilteringOptions(FilterOptions.Point, FilterOptions.Point, FilterOptions.None);

                if (numTextureUnits > 0) //If this pass uses textures adjust its fragment program
                {
                    using (var gpuParams = pass.getFragmentProgramParameters())
                    {
                        if (gpuParams.Value.hasNamedConstant("pageTableSize"))
                        {
                            gpuParams.Value.setNamedConstant("pageTableSize", new Vector2(numPages.Width, numPages.Height));
                            gpuParams.Value.setNamedConstant("physicalSizeRecip", virtualTextureManager.PhysicalSizeRecrip);
                            gpuParams.Value.setNamedConstant("pageSizeLog2", new Vector2(virtualTextureManager.TexelsPerPageLog2, virtualTextureManager.TexelsPerPageLog2));
                            gpuParams.Value.setNamedConstant("atlasScale", virtualTextureManager.AtlasScale);
                        }
                        else
                        {
                            Logging.Log.Debug("page table size varaible missing");
                        }
                    }
                }
                else
                {
                    Logging.Log.Debug("No Textures");
                }
            }

            numPasses = feedbackTechnique.getNumPasses();
            for (ushort i = 0; i < numPasses; ++i)
            {
                var pass = feedbackTechnique.getPass(i);
                using (var gpuParams = pass.getFragmentProgramParameters())
                {
                    gpuParams.Value.setNamedConstant("virtTexSize", new Vector2(realTextureSize.Width, realTextureSize.Height));
                    gpuParams.Value.setNamedConstant("mipSampleBias", 0.0f);
                    gpuParams.Value.setNamedConstant("spaceId", (float)id);
                }
            }
        }

        public int Id
        {
            get
            {
                return id;
            }
        }

        internal void beginPageUpdate()
        {
            visibleThisUpdate.Clear();
            removedPages.Clear();
            addedPages.Clear();
        }

        internal void processPage(float u, float v, int mip)
        {
            VTexPage page;
            if(mip >= highestMip)
            {
                page = new VTexPage(0, 0, highestMip - 1, id);
            }
            else
            {
                IntSize2 mipLevelNumPages = numPages / (1 << mip);
                int x = (int)(u * mipLevelNumPages.Width);
                int y = (int)(v * mipLevelNumPages.Height);
                if (x == mipLevelNumPages.Width)
                {
                    --x;
                }
                if (y == mipLevelNumPages.Height)
                {
                    --y;
                }
                page = new VTexPage(x, y, mip, id);
            }
            if(!activePages.Contains(page))
            {
                if (!addedPages.Contains(page))
                {
                    addedPages.Add(page);
                }
                else
                {
                    //Logging.Log.Debug("Skipped duplicate {0}", page);
                }
            }
            else
            {
                visibleThisUpdate.Add(page);
                //Logging.Log.Debug("Rejected page {0} {1}", page.x, page.y);
            }
        }

        internal void finishPageUpdate()
        {
            foreach(var page in activePages)
            {
                if(!visibleThisUpdate.Contains(page))
                {
                    removedPages.Add(page);
                }
            }

            if (addedPages.Count > 0 || removedPages.Count > 0)
            {
                foreach (var page in removedPages)
                {
                    activePages.Remove(page);
                    //Logging.Log.Debug("Removed page {0} for {1}", page, indirectionTexture.Value.Name);
                }
                foreach (var page in addedPages)
                {
                    activePages.Add(page);
                    //Logging.Log.Debug("Added page {0} for {1}", page, indirectionTexture.Value.Name);
                }
            }

            //Sort active pages by mip level
            //activePages.Sort(.OrderBy(p => p.mip);
            activePages.Sort((x, y) => y.mip - x.mip); //Probably don't need the sort if we are going to load through another class, keeping for now
        }

        /// <summary>
        /// Apply page changes to the texture, this writes to the gpu so it must be called from the render thread.
        /// </summary>
        internal void applyPageChanges()
        {
            if (updateTextureOnApply)
            {
                for (int i = 0; i < highestMip; ++i)
                {
                    //Debug corner
                    //var color = new FreeImageAPI.Color();
                    //color.A = 255; //Using this for now for page enabled (255) / disabled (0)
                    ////Reverse the mip level (0 becomes highest level (least texels) and highesetMip becomes the lowest level (most texels, full size)
                    //color.B = 0; //Typecast bad, try changing the type in the struct to byte
                    //color.R = 0;
                    //color.G = 255;

                    //fiBitmap[i].SetPixel(fiBitmap[i].Width - 1, fiBitmap[i].Height - 1, color);

                    //Save freeimage bitmaps
                    using (var stream = System.IO.File.Open(indirectionTexture.Value.Name + "_FreeImage_" + i + ".bmp", System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite))
                    {
                        fiBitmap[i].Save(stream, FreeImageAPI.FREE_IMAGE_FORMAT.FIF_BMP);
                    }

                    buffer[i].Value.blitFromMemory(pixelBox[i]); //Need this line
                    
                    //Save render target
                    buffer[i].Value.getRenderTarget().writeContentsToFile(indirectionTexture.Value.Name + "_" + i + ".bmp");
                }
                updateTextureOnApply = false;
            }
        }

        internal void addPhysicalPage(PTexPage pTexPage)
        {
            updateTextureOnApply = true;
            //Store 1x1 as mip 0, 2x2 as 1 4x4 as 2 etc, this way we can directly shift the decimal place
            //Then we will take fract from that
            //Store the page address as bytes
            var vTextPage = pTexPage.VirtualTexturePage;
            var color = new FreeImageAPI.Color();
            color.A = 255; //Using this for now for page enabled (255) / disabled (0)
            //Reverse the mip level (0 becomes highest level (least texels) and highesetMip becomes the lowest level (most texels, full size)
            color.B = (byte)(highestMip - vTextPage.mip); //Typecast bad, try changing the type in the struct to byte
            color.R = (byte)vTextPage.x;
            color.G = (byte)vTextPage.y;

            //if (vTextPage.mip == 0)
            //{
            //    color.B = 255;
            //}

            //if (vTextPage.mip == 1)
            //{
            //    color.B = 50;
            //}

            //if (vTextPage.mip == 2)
            //{
            //    color.B = 150;
            //}

            fiBitmap[vTextPage.mip].SetPixel(vTextPage.x, vTextPage.y, color);

            //Fill in lower (more textels) mip levels
            int x = vTextPage.x;
            int y = vTextPage.y;
            int w = 1;
            int h = 1;
            for (int i = vTextPage.mip - 1; i >= 0; --i)
            {
                //color.B = (byte)(255 - (byte)((i / 5.0f) * 255));
                //This is probably really slow
                x = x << 1;
                y = y << 1;
                w = w << 1;
                h = h << 1;
                var mipLevelBitmap = fiBitmap[i];
                for (int xi = 0; xi < w; ++xi)
                {
                    for (int yi = 0; yi < h; ++yi)
                    {
                        var readPixel = mipLevelBitmap.GetPixel(x + xi, y + yi);
                        if (color.B > readPixel.B) //If the replacement mip level is greater than the current one (remember this is a shift mip inverted from normal mip)
                        {
                            mipLevelBitmap.SetPixel(x + xi, y + yi, color);
                        }
                    }
                }
            }
        }

        internal void removePhysicalPage(PTexPage pTexPage)
        {
            updateTextureOnApply = true;
            var vTextPage = pTexPage.VirtualTexturePage;
            //Replace color with the one on the higher mip level
            FreeImageAPI.Color color;
            if (vTextPage.mip + 1 < highestMip)
            {
                color = fiBitmap[vTextPage.mip + 1].GetPixel(vTextPage.x >> 1, vTextPage.y >> 1);
            }
            else
            {
                color = new FreeImageAPI.Color();
            }
            fiBitmap[vTextPage.mip].SetPixel(vTextPage.x, vTextPage.y, color);
        }

        public String TextureName
        {
            get
            {
                return indirectionTexture.Value.Name;
            }
        }

        internal IReadOnlyList<VTexPage> ActivePages
        {
            get
            {
                return activePages;
            }
        }

        internal IReadOnlyDictionary<String, String> OriginalTextures
        {
            get
            {
                return originalTextureUnits;
            }
        }
    }
}
