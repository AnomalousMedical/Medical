using Engine;
using Engine.Threads;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class VirtualTextureManager : IDisposable
    {
        enum Phase
        {
            RenderFeedback = 0,
            CopyFeedbackToStaging = 1,
            CopyStagingToMemory = 2,
            AnalyzeFeedback = 3,
            Max = 4
        }

        public const String ResourceGroup = "VirtualTextureGroup";

        FeedbackBuffer feedbackBuffer;
        int frameCount = 0;
        int updateBufferFrame = 2;
        Phase phase = Phase.RenderFeedback;
        int texelsPerPage = 128;
        int padding;
        IntSize2 physicalTextureSize = new IntSize2(4096, 4096);

        Dictionary<String, PhysicalTexture> physicalTextures = new Dictionary<string, PhysicalTexture>();
        Dictionary<String, IndirectionTexture> indirectionTextures = new Dictionary<string, IndirectionTexture>();
        Dictionary<int, IndirectionTexture> indirectionTexturesById = new Dictionary<int, IndirectionTexture>();
        TextureLoader textureLoader;

        public VirtualTextureManager()
        {
            if (!OgreResourceGroupManager.getInstance().resourceGroupExists(VirtualTextureManager.ResourceGroup))
            {
                OgreResourceGroupManager.getInstance().createResourceGroup(VirtualTextureManager.ResourceGroup);
            }

            switch (OgreInterface.Instance.SelectedTextureFormat)
            {
                case CompressedTextureSupport.DXT:
                    padding = 4;
                    break;
                default:
                    padding = 1;
                    break;
            }

            feedbackBuffer = new FeedbackBuffer(this, 0);
            textureLoader = new TextureLoader(this, new IntSize2(4096, 4096), texelsPerPage, padding);
        }

        public void Dispose()
        {
            foreach (var physicalTexture in physicalTextures.Values)
            {
                physicalTexture.Dispose();
            }

            foreach (var indirectionTexture in indirectionTextures.Values)
            {
                indirectionTexture.Dispose();
            }

            //Feedback buffer cameras are intended to be destroyed by the classes that create them, this does not provide automatic cleanup
        }

        public PhysicalTexture createPhysicalTexture(String name)
        {
            PhysicalTexture pt = new PhysicalTexture(name, physicalTextureSize, this, texelsPerPage);
            physicalTextures.Add(name, pt);
            return pt;
        }

        public void destroyFeedbackBufferCamera()
        {
            feedbackBuffer.destroyCamera();
        }

        public void createFeedbackBufferCamera(Camera camera, IntSize2 size)
        {
            feedbackBuffer.createCamera(camera, size);
        }

        public void update()
        {
            if (frameCount == 0)
            {
                switch (phase)
                {
                    case Phase.RenderFeedback:
                        feedbackBuffer.update();
                        break;
                    case Phase.CopyFeedbackToStaging:
                        feedbackBuffer.blitToStaging();
                        break;
                    case Phase.CopyStagingToMemory:
                        feedbackBuffer.blitStagingToMemory();
                        break;
                    case Phase.AnalyzeFeedback:
                        textureLoader.beginPageUpdate();
                        feedbackBuffer.analyzeBuffer();

                        PerformanceMonitor.start("Finish Page Update");
                        foreach (var indirectionTex in indirectionTextures.Values)
                        {
                            indirectionTex.finishPageUpdate();
                        }
                        PerformanceMonitor.stop("Finish Page Update");

                        PerformanceMonitor.start("Update Texture Loader");
                        textureLoader.updatePagesFromRequests();
                        PerformanceMonitor.stop("Update Texture Loader");

                        PerformanceMonitor.start("Upload Indirection Texture Update");
                        foreach (var indirectionTex in indirectionTextures.Values) //This could be improved with a queue of just updated textures
                        {
                            indirectionTex.uploadPageChanges();
                        }
                        PerformanceMonitor.stop("Upload Indirection Texture Update");
                        break;
                }

                phase = (Phase)(((int)phase + 1) % (int)Phase.Max);
            }
            frameCount = (frameCount + 1) % updateBufferFrame;
        }

        public void processMaterialAdded(String materialSetKey, Technique mainTechnique, Technique feedbackTechnique)
        {
            //This funciton will not be needed in the future
            IndirectionTexture indirectionTex;
            if (!indirectionTextures.TryGetValue(materialSetKey, out indirectionTex))
            {
                IntSize2 textureSize = new IntSize2();
                if (getTextureSize(mainTechnique, ref textureSize) && textureSize.Width > texelsPerPage && textureSize.Height > texelsPerPage)
                {
                    indirectionTex = new IndirectionTexture(materialSetKey, textureSize, texelsPerPage, this); //This is a terrible way to get size since ogre must load the resource first, trying to avoid that
                    indirectionTextures.Add(materialSetKey, indirectionTex);
                    indirectionTexturesById.Add(indirectionTex.Id, indirectionTex);
                }
                else
                {
                    Logging.Log.Debug("Could not create a feedback texture for material {0}", materialSetKey);
                    return;
                }
            }
            indirectionTex.reconfigureTechnique(mainTechnique, feedbackTechnique);
        }

        /// <summary>
        /// Create or retrieve an indirection texture, will return true if the texture was just created. Useful
        /// for filling out other info on the texture if needed.
        /// </summary>
        /// <param name="indirectionKey">The key to use to search for this texture.</param>
        /// <param name="textureSize">The size of the virtual texture this indirection texture needs to remap.</param>
        /// <param name="indirectionTex">An out variable for the results.</param>
        /// <returns>True if the texture was just created, false if not.</returns>
        public bool createOrRetrieveIndirectionTexture(String indirectionKey, IntSize2 textureSize, out IndirectionTexture indirectionTex)
        {
            if (!indirectionTextures.TryGetValue(indirectionKey, out indirectionTex))
            {
                indirectionTex = new IndirectionTexture(indirectionKey, textureSize, texelsPerPage, this);
                indirectionTextures.Add(indirectionKey, indirectionTex);
                indirectionTexturesById.Add(indirectionTex.Id, indirectionTex);
                return true;
            }
            return false;
        }

        public bool getIndirectionTexture(String indirectionKey, out IndirectionTexture indirectionTex)
        {
            return indirectionTextures.TryGetValue(indirectionKey, out indirectionTex);
        }

        public void setupVirtualTextureFragmentParams(GpuProgramParametersSharedPtr gpuParams)
        {
            if (gpuParams.Value.hasNamedConstant("physicalSizeRecip"))
            {
                gpuParams.Value.setNamedConstant("physicalSizeRecip", PhysicalSizeRecrip);
                gpuParams.Value.setNamedConstant("pageSizeLog2", new Vector2(TexelsPerPageLog2, TexelsPerPageLog2));
                gpuParams.Value.setNamedConstant("pagePaddingScale", TextureLoader.PagePaddingScale);
                gpuParams.Value.setNamedConstant("pagePaddingOffset", TextureLoader.PagePaddingOffset);
            }
            else
            {
                Logging.Log.Error("physicalSizeRecip varaible missing");
            }
        }

        public bool getTextureSize(Technique technique, ref IntSize2 size)
        {
            int numPasses = technique.getNumPasses();
            if (numPasses > 0)
            {
                var pass = technique.getPass(0);
                ushort numTextureUnits = pass.getNumTextureUnitStates();
                if (numTextureUnits > 0)
                {
                    var texUnit = pass.getTextureUnitState(0);
                    using (var texture = TextureManager.getInstance().getByName(texUnit.TextureName, technique.getResourceGroup()))
                    {
                        if (texture.Value != null)
                        {
                            size.Width = (int)texture.Value.Width;
                            size.Height = (int)texture.Value.Height;
                            return true;
                        }
                        else
                        {
                            Logging.Log.Debug("Texture {0} not found for material {1}", texUnit.TextureName, technique.getParent().Name);
                        }
                    }
                }
                else
                {
                    Logging.Log.Debug("No texture units in material {0}", technique.getParent().Name);
                }
            }
            else
            {
                Logging.Log.Debug("No passes in material {0}", technique.getParent().Name);
            }
            return false;
        }

        public void processMaterialRemoved(Object materialSetKey)
        {
            //Need to do something here
        }

        internal PhysicalTexture getPhysicalTexture(string name)
        {
            return physicalTextures[name];
        }

        internal bool getIndirectionTexture(int id, out IndirectionTexture tex)
        {
            return indirectionTexturesById.TryGetValue(id, out tex);
        }

        public IEnumerable<string> TextureNames
        {
            get
            {
                foreach (var item in physicalTextures.Values)
                {
                    yield return item.TextureName;
                }
                if (feedbackBuffer.TextureName != null)
                {
                    yield return feedbackBuffer.TextureName;
                }
                foreach (var item in indirectionTextures.Values)
                {
                    yield return item.TextureName;
                }
            }
        }

        internal IEnumerable<IndirectionTexture> IndirectionTextures
        {
            get
            {
                return indirectionTextures.Values;
            }
        }

        internal TextureLoader TextureLoader
        {
            get
            {
                return textureLoader;
            }
        }

        public int TexelsPerPage
        {
            get
            {
                return texelsPerPage;
            }
        }

        public int TexelsPerPageLog2
        {
            get
            {
                //return (int)Math.Log(texelsPerPage, 2.0); //Should be this
                //hlsl needs between 0 and 1
                return (int)(Math.Log(texelsPerPage, 2.0) / 6.0f);
            }
        }

        internal Vector2 PhysicalSizeRecrip
        {
            get
            {
                float textelRatio = texelsPerPage + padding * 2;
                return new Vector2(1.0f / (physicalTextureSize.Width / textelRatio), 1.0f / (physicalTextureSize.Height / textelRatio));
            }
        }
    }
}
