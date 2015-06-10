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
        private IntSize2 indirectionTextureSize;
        private TexturePtr indirectionTexture;
        private VirtualTextureManager virtualTextureManager;
        private IntSize2 numPages;

        public IndirectionTexture(IntSize2 realTextureSize, int textelsPerPage, VirtualTextureManager virtualTextureManager)
        {
            this.virtualTextureManager = virtualTextureManager;
            this.realTextureSize = realTextureSize;
            this.indirectionTextureSize = realTextureSize / textelsPerPage;
            numPages = realTextureSize / textelsPerPage;
            indirectionTexture = TextureManager.getInstance().createManual("IndirectionTexture" + id, VirtualTextureManager.ResourceGroup, TextureType.TEX_TYPE_2D, (uint)indirectionTextureSize.Width, (uint)indirectionTextureSize.Height, 1, 0, PixelFormat.PF_A8R8G8B8, TextureUsage.TU_DYNAMIC_WRITE_ONLY, null, false, 0);
        }

        public void Dispose()
        {
            indirectionTexture.Dispose();
        }

        public void reconfigureTechnique(Technique technique)
        {
            int numPasses = technique.getNumPasses();
            for(ushort i = 0; i < numPasses; ++i)
            {
                var pass = technique.getPass(i);
                ushort numTextureUnits = pass.getNumTextureUnitStates();
                for(ushort t = 0; t < numTextureUnits; ++t)
                {
                    var texUnit = pass.getTextureUnitState(t);
                    texUnit.TextureName = virtualTextureManager.getPhysicalTexture(texUnit.Name).TextureName;
                }
                pass.createTextureUnitState(indirectionTexture.Value.getName()); //Add indirection texture
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

        internal void processPage(float u, float v, int mip)
        {
            IntSize2 pageMultipler = numPages;
            if(mip != 0)
            {
                pageMultipler /= (2 * mip);
            }
            int xPage = (int)(u * pageMultipler.Width);
            int yPage = (int)(u * pageMultipler.Height);
            int page = yPage * numPages.Width + xPage;
            if(!activePages.Contains(page))
            {
                activePages.Add(page);
                Console.WriteLine("Setup page {0} for {1}", page, indirectionTexture.Value.Name);
            }
        }
    }
}
