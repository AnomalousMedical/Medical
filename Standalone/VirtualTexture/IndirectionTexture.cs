﻿using Engine;
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
        static byte currentId = 0;
        static byte maxId = 254;
        static HashSet<byte> usedIds = new HashSet<byte>();
        static byte generateId()
        {
            lock (usedIds)
            {
                while (usedIds.Contains(currentId))
                {
                    incrementCurrentId();
                }
                byte retVal = currentId;
                incrementCurrentId();
                return retVal;
            }
        }

        static void incrementCurrentId()
        {
            currentId = (byte)((currentId + 1) % maxId);
        }

        private byte id = generateId();
        private IntSize2 realTextureSize;
        private TexturePtr indirectionTexture;
        private VirtualTextureManager virtualTextureManager;
        private IntSize2 numPages;
        private byte highestMip = 0; //The highest mip level that does not fall below one page in size
        private FreeImageAPI.FreeImageBitmap[] fiBitmap; //Can we do this without this bitmap? (might be ok to keep, but will be using 2x as much memory, however, allows for background modification, could even double buffer)
        private HardwarePixelBufferSharedPtr[] buffer;
        private PixelBox[] pixelBox;

        private HashSet<VTexPage> activePages = new HashSet<VTexPage>();
        private HashSet<VTexPage> visibleThisUpdate = new HashSet<VTexPage>();
        private List<VTexPage> removedPages = new List<VTexPage>();
        private HashSet<VTexPage> addedPages = new HashSet<VTexPage>();
        private bool updateTextureOnApply = false;
        private Dictionary<String, String> originalTextureUnits;

        public IndirectionTexture(String materialSetKey, IntSize2 realTextureSize, int texelsPerPage, VirtualTextureManager virtualTextureManager)
        {
            this.virtualTextureManager = virtualTextureManager;
            this.realTextureSize = realTextureSize;
            numPages = realTextureSize / texelsPerPage;
            for (highestMip = 0; realTextureSize.Width >> highestMip >= texelsPerPage && realTextureSize.Height >> highestMip >= texelsPerPage; ++highestMip) { }
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

            addedPages.Add(new VTexPage(0, 0, (byte)(highestMip - 1), id));
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
            bool foundTextures = true; //Mostly for debugging
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
                    //texUnit.setFilteringOptions(FilterOptions.Point, FilterOptions.Point, FilterOptions.None);
                }
                var indirectionTexturePass = pass.createTextureUnitState(indirectionTexture.Value.getName()); //Add indirection texture
                indirectionTexturePass.setFilteringOptions(FilterOptions.Point, FilterOptions.Point, FilterOptions.None);

                if (numTextureUnits > 0) //If this pass uses textures adjust its fragment program
                {
                    using (var gpuParams = pass.getFragmentProgramParameters())
                    {
                        if (gpuParams.Value.hasNamedConstant("physicalSizeRecip"))
                        {
                            gpuParams.Value.setNamedConstant("physicalSizeRecip", virtualTextureManager.PhysicalSizeRecrip);
                            gpuParams.Value.setNamedConstant("pageSizeLog2", new Vector2(virtualTextureManager.TexelsPerPageLog2, virtualTextureManager.TexelsPerPageLog2));
                            gpuParams.Value.setNamedConstant("pagePaddingScale", virtualTextureManager.TextureLoader.PagePaddingScale);
                            gpuParams.Value.setNamedConstant("pagePaddingOffset", virtualTextureManager.TextureLoader.PagePaddingOffset);
                        }
                        else
                        {
                            Logging.Log.Debug("physicalSizeRecip varaible missing {0}", mainTechnique.getParent().Name);
                        }
                    }
                }
            }

            if(!foundTextures)
            {
                Logging.Log.Debug("Found no textures in any passes {0}", mainTechnique.getParent().Name);
            }

            numPasses = feedbackTechnique.getNumPasses();
            for (ushort i = 0; i < numPasses; ++i)
            {
                var pass = feedbackTechnique.getPass(i);
                using (var gpuParams = pass.getFragmentProgramParameters())
                {
                    gpuParams.Value.setNamedConstant("virtTexSize", new Vector2(realTextureSize.Width, realTextureSize.Height));
                    gpuParams.Value.setNamedConstant("mipSampleBias", -3.0f);
                    gpuParams.Value.setNamedConstant("spaceId", (float)id);
                }
            }
        }

        public byte Id
        {
            get
            {
                return id;
            }
        }

        internal void processPage(float u, float v, byte mip)
        {
            VTexPage page;
            if (mip >= highestMip)
            {
                page = new VTexPage(0, 0, (byte)(highestMip - 1), id);
            }
            else
            {
                IntSize2 mipLevelNumPages = numPages / (1 << mip);
                byte x = (byte)(u * mipLevelNumPages.Width);
                byte y = (byte)(v * mipLevelNumPages.Height);
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
            if (activePages.Contains(page))
            {
                visibleThisUpdate.Add(page);
            }
            else
            {
                addedPages.Add(page);
            }
        }

        internal void finishPageUpdate()
        {
            foreach(var page in activePages)
            {
                if(!visibleThisUpdate.Contains(page) && page.mip != highestMip - 1)
                {
                    removedPages.Add(page);
                }
            }

            if (addedPages.Count > 0 || removedPages.Count > 0)
            {
                foreach (var page in removedPages)
                {
                    virtualTextureManager.TextureLoader.removeRequestedPage(page);
                    activePages.Remove(page);
                }
                foreach (var page in addedPages)
                {
                    virtualTextureManager.TextureLoader.addRequestedPage(page);
                    activePages.Add(page);
                }
            } 
            
            visibleThisUpdate.Clear();
            removedPages.Clear();
            addedPages.Clear();
        }

        /// <summary>
        /// Apply page changes to the texture, this writes to the gpu so it must be called from the render thread.
        /// </summary>
        internal void uploadPageChanges()
        {
            if (updateTextureOnApply)
            {
                for (int i = 0; i < highestMip; ++i)
                {
                    buffer[i].Value.blitFromMemory(pixelBox[i]); //Need this line
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
            color.B = (byte)(highestMip - vTextPage.mip - 1); //Typecast bad, try changing the type in the struct to byte
            color.R = (byte)pTexPage.pageX;
            color.G = (byte)pTexPage.pageY;

            fiBitmap[vTextPage.mip].SetPixel(vTextPage.x, vTextPage.y, color);
            fillOutLowerMips(vTextPage, color, (c1, c2) => c1.B - c2.B >= 0);
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
                color.B = (byte)(highestMip - vTextPage.mip - 1);
            }
            byte replacementMipLevel = (byte)(highestMip - vTextPage.mip - 1);
            fiBitmap[vTextPage.mip].SetPixel(vTextPage.x, vTextPage.y, color);
            fillOutLowerMips(vTextPage, color, (c1, c2) => c2.B == replacementMipLevel);
        }

        private void fillOutLowerMips(VTexPage vTextPage, FreeImageAPI.Color color, Func<FreeImageAPI.Color, FreeImageAPI.Color, bool> writePixel)
        {
            //Fill in lower (more textels) mip levels
            int x = vTextPage.x;
            int y = vTextPage.y;
            int w = 1;
            int h = 1;
            for (int i = vTextPage.mip - 1; i >= 0; --i)
            {
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
                        if (writePixel.Invoke(color, readPixel))
                        {
                            mipLevelBitmap.SetPixel(x + xi, y + yi, color);
                        }
                    }
                }
            }
        }

        public String TextureName
        {
            get
            {
                return indirectionTexture.Value.Name;
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
