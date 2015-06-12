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
        private FreeImageAPI.FreeImageBitmap fiBitmap; //Can we do this without this bitmap? (might be ok to keep, but will be using 2x as much memory, however, allows for background modification, could even double buffer)
        private HardwarePixelBufferSharedPtr buffer;
        private PixelBox pixelBox;

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
            indirectionTexture = TextureManager.getInstance().createManual(String.Format("{0}_IndirectionTexture_{1}", materialSetKey, id), VirtualTextureManager.ResourceGroup, TextureType.TEX_TYPE_2D, (uint)numPages.Width, (uint)numPages.Height, 1, 0, PixelFormat.PF_A8R8G8B8, TextureUsage.TU_DYNAMIC_WRITE_ONLY_DISCARDABLE, null, false, 0);
            fiBitmap = new FreeImageAPI.FreeImageBitmap((int)indirectionTexture.Value.Width, (int)indirectionTexture.Value.Height, FreeImageAPI.PixelFormat.Format32bppArgb);
            buffer = indirectionTexture.Value.getBuffer();
            unsafe
            {
                pixelBox = new PixelBox(0, 0, fiBitmap.Width, fiBitmap.Height, OgreDrawingUtility.getOgreFormat(fiBitmap.PixelFormat), fiBitmap.GetScanlinePointer(0).ToPointer());
            }

            //temp, always want to force lowest mip level for now
            highestMip = 0;
        }

        public void Dispose()
        {
            pixelBox.Dispose();
            buffer.Dispose();
            indirectionTexture.Dispose();
            fiBitmap.Dispose();
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
                }
                pass.createTextureUnitState(indirectionTexture.Value.getName()); //Add indirection texture
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
            //if(mip > highestMip)
            //{
            //    return;
            //}
            IntSize2 mipLevelNumPages = numPages / (1 << mip);
            VTexPage page;
            if(mipLevelNumPages == new IntSize2())
            {
                page = new VTexPage(0, 0, mip, id);
            }
            else
            {
                page = new VTexPage((int)(u * mipLevelNumPages.Width), (int)(v * mipLevelNumPages.Height), mip, id);
                if (page.x == mipLevelNumPages.Width)
                {
                    --page.x;
                }
                if (page.y == mipLevelNumPages.Height)
                {
                    --page.y;
                }
            }
            if(!activePages.Contains(page))
            {
                if (!addedPages.Contains(page))
                {
                    addedPages.Add(page);
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
                updateTextureOnApply = true;

                var activeColor = new FreeImageAPI.Color();
                activeColor.R = 0;
                activeColor.G = 255;
                activeColor.B = 0;
                activeColor.A = 255;

                var inactiveColor = new FreeImageAPI.Color();
                inactiveColor.R = 255;
                inactiveColor.G = 0;
                inactiveColor.B = 0;
                inactiveColor.A = 255;

                foreach (var page in removedPages)
                {
                    activePages.Remove(page);
                    //Logging.Log.Debug("Removed page {0} for {1}", page, indirectionTexture.Value.Name);
                    fiBitmap.SetPixel(page.x, page.y, inactiveColor);
                }
                foreach (var page in addedPages)
                {
                    activePages.Add(page);
                    //Logging.Log.Debug("Added page {0} for {1}", page, indirectionTexture.Value.Name);
                    fiBitmap.SetPixel(page.x, page.y, activeColor);
                }
            }

            //Sort active pages by mip level
            //activePages.Sort(.OrderBy(p => p.mip);
            activePages.Sort((x, y) => y.mip - x.mip);
        }

        /// <summary>
        /// Apply page changes to the texture, this writes to the gpu so it must be called from the render thread.
        /// </summary>
        internal void applyPageChanges()
        {
            if (updateTextureOnApply)
            {
                buffer.Value.blitFromMemory(pixelBox);
                updateTextureOnApply = false;
            }
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
