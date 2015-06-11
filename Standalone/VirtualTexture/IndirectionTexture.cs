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
        private IntSize2 indirectionTextureSize;
        private TexturePtr indirectionTexture;
        private VirtualTextureManager virtualTextureManager;
        private IntSize2 numPages;
        private int highestMip = 0; //The highest mip level that does not fall below one page in size

        public IndirectionTexture(String materialSetKey, IntSize2 realTextureSize, int textelsPerPage, VirtualTextureManager virtualTextureManager)
        {
            this.virtualTextureManager = virtualTextureManager;
            this.realTextureSize = realTextureSize;
            this.indirectionTextureSize = realTextureSize / textelsPerPage;
            numPages = realTextureSize / textelsPerPage;
            for (highestMip = 0; realTextureSize.Width >> highestMip > textelsPerPage && realTextureSize.Height >> highestMip > textelsPerPage; ++highestMip) { }
            indirectionTexture = TextureManager.getInstance().createManual(String.Format("{0}_IndirectionTexture_{1}", materialSetKey, id), VirtualTextureManager.ResourceGroup, TextureType.TEX_TYPE_2D, (uint)indirectionTextureSize.Width, (uint)indirectionTextureSize.Height, 1, 0, PixelFormat.PF_A8R8G8B8, TextureUsage.TU_DYNAMIC_WRITE_ONLY, null, false, 0);
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

        internal void beginPageUpdate()
        {
            visibleThisUpdate.Clear();
        }

        internal void processPage(float u, float v, int mip)
        {
            if(mip > highestMip)
            {
                mip = highestMip;
            }
            IntSize2 mipLevelNumPages = numPages;
            mipLevelNumPages /= 1 << mip;
            int xPage = (int)(u * mipLevelNumPages.Width);
            int yPage = (int)(v * mipLevelNumPages.Height);
            int page = yPage * numPages.Width + xPage;
            visibleThisUpdate.Add(page);
            if(!activePages.Contains(page))
            {
                activePages.Add(page);
                Logging.Log.Debug("Added page {0} (mip {1}) for {2} pages for mip level {3} total {4}", page, mip, indirectionTexture.Value.Name, mipLevelNumPages.Width, numPages.Width);
            }
        }

        internal void finishPageUpdate()
        {
            List<int> removedPages = new List<int>();
            foreach(int page in activePages)
            {
                if(!visibleThisUpdate.Contains(page))
                {
                    removedPages.Add(page);
                }
            }
            foreach(int page in removedPages)
            {
                activePages.Remove(page);
                Logging.Log.Debug("Removed page {0} for {1}", page, indirectionTexture.Value.Name);
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
