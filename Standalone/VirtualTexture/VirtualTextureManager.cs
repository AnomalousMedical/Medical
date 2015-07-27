using Engine;
using Engine.ObjectManagement;
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
            RenderFeedback,
            CopyFeedbackToStaging,
            CopyStagingToMemory,
            AnalyzeFeedback,
            Waiting,
            InitialLoad,
            Reset
        }

        public const String ResourceGroup = "VirtualTextureGroup";

        FeedbackBuffer feedbackBuffer;
        int frameCount = 0;
        int updateBufferFrame = 2;
        Phase phase = Phase.InitialLoad; //All indirection textures will be created requesting their lowest mip levels, this will force us to load those as quickly as possible
        int texelsPerPage;
        int padding;
        CompressedTextureSupport textureFormat;
        IntSize2 physicalTextureSize;

        Dictionary<String, PhysicalTexture> physicalTextures = new Dictionary<string, PhysicalTexture>();
        Dictionary<String, IndirectionTexture> indirectionTextures = new Dictionary<string, IndirectionTexture>();
        Dictionary<int, IndirectionTexture> indirectionTexturesById = new Dictionary<int, IndirectionTexture>();
        TextureLoader textureLoader;

        public VirtualTextureManager(int numPhysicalTextures, IntSize2 physicalTextureSize, int texelsPerPage, CompressedTextureSupport textureFormat, int padding, int stagingBufferCount, IntSize2 feedbackBufferSize)
        {
            this.physicalTextureSize = physicalTextureSize;
            this.texelsPerPage = texelsPerPage;
            if (!OgreResourceGroupManager.getInstance().resourceGroupExists(VirtualTextureManager.ResourceGroup))
            {
                OgreResourceGroupManager.getInstance().createResourceGroup(VirtualTextureManager.ResourceGroup);
            }

            this.padding = padding;
            this.textureFormat = textureFormat;

            feedbackBuffer = new FeedbackBuffer(this, feedbackBufferSize, 0);
            textureLoader = new TextureLoader(this, physicalTextureSize, texelsPerPage, padding, stagingBufferCount, numPhysicalTextures, 6, 500 * 1024 * 1024);

            MipSampleBias = -3.0f;
        }

        public void Dispose()
        {
            textureLoader.Dispose();
            feedbackBuffer.Dispose();

            foreach (var physicalTexture in physicalTextures.Values)
            {
                physicalTexture.Dispose();
            }

            foreach (var indirectionTexture in indirectionTextures.Values)
            {
                indirectionTexture.Dispose();
            }
        }

        public PhysicalTexture createPhysicalTexture(String name, PixelFormatUsageHint pixelFormatUsageHint)
        {
            PhysicalTexture pt = new PhysicalTexture(name, physicalTextureSize, this, texelsPerPage, textureFormat, pixelFormatUsageHint);
            physicalTextures.Add(name, pt);
            textureLoader.addedPhysicalTexture(pt);
            return pt;
        }

        public void destroyFeedbackBufferCamera(SimScene scene)
        {
            feedbackBuffer.destroyCamera(scene);
        }

        public void createFeedbackBufferCamera(SimScene scene, FeedbackCameraPositioner cameraPositioner)
        {
            feedbackBuffer.createCamera(scene, cameraPositioner);
        }

        public void update()
        {
            if (frameCount == 0)
            {
                switch (phase)
                {
                    case Phase.RenderFeedback:
                        feedbackBuffer.update();
                        phase = Phase.CopyFeedbackToStaging;
                        break;
                    case Phase.CopyFeedbackToStaging:
                        feedbackBuffer.blitToStaging();
                        phase = Phase.CopyStagingToMemory;
                        break;
                    case Phase.CopyStagingToMemory:
                        feedbackBuffer.blitStagingToMemory();
                        phase = Phase.AnalyzeFeedback;
                        break;
                    case Phase.AnalyzeFeedback:
                        phase = Phase.Waiting;
                        ThreadManager.RunInBackground(() =>
                            {
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

                                phase = Phase.RenderFeedback;
                            });
                        break;
                    case Phase.InitialLoad:
                        phase = Phase.Waiting;
                        ThreadManager.RunInBackground(() =>
                        {
                            textureLoader.beginPageUpdate();

                            foreach (var indirectionTex in indirectionTextures.Values)
                            {
                                indirectionTex.finishPageUpdate();
                            }

                            textureLoader.updatePagesFromRequests();

                            phase = Phase.RenderFeedback;
                        });
                        break;
                    case Phase.Reset:
                        phase = Phase.Waiting;
                        ThreadManager.RunInBackground(() =>
                        {
                            textureLoader.beginPageUpdate();

                            foreach (var indirectionTex in indirectionTextures.Values)
                            {
                                indirectionTex.finishPageUpdate();
                            }
                            textureLoader.updatePagesFromRequests();
                            textureLoader.clearCache();

                            phase = Phase.RenderFeedback;
                        });
                        break;
                }
            }
            frameCount = (frameCount + 1) % updateBufferFrame;
        }

        public void reset()
        {
            phase = Phase.Reset;
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

        public float MipSampleBias { get; set; }

        internal Vector2 PhysicalSizeRecrip
        {
            get
            {
                float textelRatio = texelsPerPage + padding * 2;
                return new Vector2(1.0f / (physicalTextureSize.Width / textelRatio), 1.0f / (physicalTextureSize.Height / textelRatio));
            }
        }

        //New System
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
    }
}
