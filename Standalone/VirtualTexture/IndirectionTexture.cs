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

        public IndirectionTexture(String materialSetKey, IntSize2 realTextureSize, int textelsPerPage, VirtualTextureManager virtualTextureManager)
        {
            this.virtualTextureManager = virtualTextureManager;
            this.realTextureSize = realTextureSize;
            numPages = realTextureSize / textelsPerPage;
            for (highestMip = 0; realTextureSize.Width >> highestMip > textelsPerPage && realTextureSize.Height >> highestMip > textelsPerPage; ++highestMip) { }
            indirectionTexture = TextureManager.getInstance().createManual(String.Format("{0}_IndirectionTexture_{1}", materialSetKey, id), VirtualTextureManager.ResourceGroup, TextureType.TEX_TYPE_2D, (uint)numPages.Width, (uint)numPages.Height, 1, 0, PixelFormat.PF_A8R8G8B8, TextureUsage.TU_DYNAMIC_WRITE_ONLY, null, false, 0);

            //temp, always want to force lowest mip level for now
            highestMip = 0;
        }

        public void Dispose()
        {
            indirectionTexture.Dispose();
        }

        public void reconfigureTechnique(Technique mainTechnique, Technique feedbackTechnique)
        {
            int numPasses = mainTechnique.getNumPasses();
            for(ushort i = 0; i < numPasses; ++i)
            {
                var pass = mainTechnique.getPass(i);
                ushort numTextureUnits = pass.getNumTextureUnitStates();
                for(ushort t = 0; t < numTextureUnits; ++t)
                {
                    var texUnit = pass.getTextureUnitState(t);
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

                    //gpuParams.Value.setConstant(new UIntPtr(0), new Vector2(realTextureSize.Width, realTextureSize.Height));
                    //gpuParams.Value.setConstant(new UIntPtr(1), 0.0f);
                    //gpuParams.Value.setConstant(new UIntPtr(2), id);
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

        private HashSet<int> activePages = new HashSet<int>();
        private HashSet<int> visibleThisUpdate = new HashSet<int>();
        private List<int> removedPages = new List<int>();
        private List<int> addedPages = new List<int>();

        internal void beginPageUpdate()
        {
            visibleThisUpdate.Clear();
            removedPages.Clear();
            addedPages.Clear();
        }

        internal void processPage(float u, float v, int mip)
        {
            if(mip > highestMip)
            {
                mip = highestMip;
            }
            IntSize2 mipLevelNumPages = numPages / (1 << mip) - 1;
            int xPage = (int)(u * mipLevelNumPages.Width);
            int yPage = (int)(v * mipLevelNumPages.Height);
            int page = yPage * mipLevelNumPages.Width + xPage;
            visibleThisUpdate.Add(page);
            if(!activePages.Contains(page))
            {
                addedPages.Add(page);
            }
        }

        internal void finishPageUpdate()
        {
            foreach(int page in activePages)
            {
                if(!visibleThisUpdate.Contains(page))
                {
                    removedPages.Add(page);
                }
            }
        }

        /// <summary>
        /// Apply page changes to the texture, this writes to the gpu so it must be called from the render thread.
        /// </summary>
        internal unsafe void applyPageChanges()
        {
            if (addedPages.Count > 0 || removedPages.Count > 0)
            {
                using (var fiBitmap = new FreeImageAPI.FreeImageBitmap((int)indirectionTexture.Value.Width, (int)indirectionTexture.Value.Height, FreeImageAPI.PixelFormat.Format32bppArgb))
                {
                    var activeColor = new FreeImageAPI.Color();
                    activeColor.R = 0;
                    activeColor.G = 255;
                    activeColor.B = 0;
                    activeColor.A = 255;

                    var inactiveColor = new FreeImageAPI.Color();
                    activeColor.R = 255;
                    activeColor.G = 0;
                    activeColor.B = 0;
                    activeColor.A = 255;

                    foreach (int page in removedPages)
                    {
                        activePages.Remove(page);
                        Logging.Log.Debug("Removed page {0} for {1}", page, indirectionTexture.Value.Name);
                        fiBitmap.SetPixel((int)(page % indirectionTexture.Value.Width), (int)(page / indirectionTexture.Value.Height), inactiveColor);
                    }
                    foreach (int page in addedPages)
                    {
                        activePages.Add(page);
                        Logging.Log.Debug("Added page {0} for {1}", page, indirectionTexture.Value.Name);
                        fiBitmap.SetPixel((int)(page % indirectionTexture.Value.Width), (int)(page / indirectionTexture.Value.Height), activeColor);
                    }

                    using (var buffer = indirectionTexture.Value.getBuffer())
                    {
                        using (PixelBox pixelBox = new PixelBox(0, 0, fiBitmap.Width, fiBitmap.Height, OgreDrawingUtility.getOgreFormat(fiBitmap.PixelFormat), fiBitmap.GetScanlinePointer(0).ToPointer()))
                        {
                            buffer.Value.blitFromMemory(pixelBox, 0, 0, (int)indirectionTexture.Value.Width, (int)indirectionTexture.Value.Height);
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
    }
}
