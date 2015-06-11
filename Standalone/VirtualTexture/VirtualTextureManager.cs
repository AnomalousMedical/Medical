using Anomalous.GuiFramework.Cameras;
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
        public const String ResourceGroup = "VirtualTextureGroup";

        FeedbackBuffer[] feedbackBuffers = new FeedbackBuffer[2];
        int frameCount = 0;
        int updateBufferFrame = 5;
        bool readbackThisFrame = false;
        int currentRenderTexture = 0;
        int currentReadbackTexture = 0;
        bool allowImageRender = true;
        int texelsPerPage = 64;

        Dictionary<String, PhysicalTexture> physicalTextures = new Dictionary<string, PhysicalTexture>();
        Dictionary<String, IndirectionTexture> indirectionTextures = new Dictionary<string, IndirectionTexture>();
        Dictionary<int, IndirectionTexture> indirectionTexturesById = new Dictionary<int, IndirectionTexture>();

        public VirtualTextureManager(SceneViewWindow window)
        {
            window.CameraCreated += window_CameraCreated;
            window.CameraDestroyed += window_CameraDestroyed;
            if (!OgreResourceGroupManager.getInstance().resourceGroupExists(VirtualTextureManager.ResourceGroup))
            {
                OgreResourceGroupManager.getInstance().createResourceGroup(VirtualTextureManager.ResourceGroup);
            }

            for (int i = 0; i < feedbackBuffers.Length; ++i)
            {
                feedbackBuffers[i] = new FeedbackBuffer(window, this, i);
            }

            currentRenderTexture = feedbackBuffers.Length - 1; //Separate rendering from readback

            //Create physical textures
            physicalTextures.Add("NormalMap", new PhysicalTexture("NormalMap", new IntSize2(4096, 4096), this, texelsPerPage));
            physicalTextures.Add("Diffuse", new PhysicalTexture("Diffuse", new IntSize2(4096, 4096), this, texelsPerPage));
            physicalTextures.Add("Specular", new PhysicalTexture("Specular", new IntSize2(4096, 4096), this, texelsPerPage));
            physicalTextures.Add("Opacity", new PhysicalTexture("Opacity", new IntSize2(4096, 4096), this, texelsPerPage));

            physicalTextures["NormalMap"].color(Color.Blue);
            physicalTextures["Diffuse"].color(Color.Red);
            physicalTextures["Specular"].color(Color.Green);
            physicalTextures["Opacity"].color(Color.HotPink);
        }

        public void Dispose()
        {
            foreach(var physicalTexture in physicalTextures.Values)
            {
                physicalTexture.Dispose();
            }
            //This do not need to be disposed for now since the window cleans them up, but we want to remove that code if possible.
            //foreach (var item in feedbackBuffers)
            //{
            //    item.Dispose();
            //}
            foreach (var indirectionTexture in indirectionTextures.Values)
            {
                indirectionTexture.Dispose();
            }
        }

        void window_CameraDestroyed(SceneViewWindow window)
        {
            for (int i = 0; i < feedbackBuffers.Length; ++i)
            {
                feedbackBuffers[i].cameraDestroyed();
            }
        }

        void window_CameraCreated(SceneViewWindow window)
        {
            for (int i = 0; i < feedbackBuffers.Length; ++i)
            {
                feedbackBuffers[i].cameraCreated();
            }
        }

        public void update(SceneViewWindow window, bool currentCameraRender)
        {
            if (allowImageRender && frameCount == 0)
            {
                ThreadManager.invoke(() =>
                {
                    allowImageRender = false;

                    if (readbackThisFrame)
                    {
                        feedbackBuffers[currentReadbackTexture].copyFromGpu();
                        currentReadbackTexture = (currentReadbackTexture + 1) % feedbackBuffers.Length;
                        readbackThisFrame = false;
                    }
                    else
                    {
                        feedbackBuffers[currentRenderTexture].update();
                        currentRenderTexture = (currentRenderTexture + 1) % feedbackBuffers.Length;
                        readbackThisFrame = true;
                    }

                    allowImageRender = true;

                });
            }
            frameCount = (frameCount + 1) % updateBufferFrame;
        }

        public void processMaterialAdded(String materialSetKey, Technique mainTechnique, Technique feedbackTechnique)
        {
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

        public bool getTextureSize(Technique technique, ref IntSize2 size)
        {
            int numPasses = technique.getNumPasses();
            if(numPasses > 0)
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
                    }
                }
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

        internal void beginPageUpdate()
        {
            foreach(var indirectionTex in indirectionTextures.Values)
            {
                indirectionTex.beginPageUpdate();
            }
        }

        internal void finishPageUpdate()
        {
            PerformanceMonitor.start("Finish Page Update");
            foreach (var indirectionTex in indirectionTextures.Values)
            {
                indirectionTex.finishPageUpdate();
            }
            PerformanceMonitor.stop("Finish Page Update");

            PerformanceMonitor.start("Apply Page Update");
            foreach (var indirectionTex in indirectionTextures.Values)
            {
                indirectionTex.applyPageChanges();
            }
            PerformanceMonitor.stop("Apply Page Update");

            PerformanceMonitor.start("Update Physical Texture");
            foreach(var physTex in physicalTextures.Values)
            {
                physTex.loadPages();
            }
            PerformanceMonitor.stop("Update Physical Texture");
        }

        public IEnumerable<string> TextureNames
        {
            get
            {
                foreach (var item in physicalTextures.Values)
                {
                    yield return item.TextureName;
                }
                foreach(var item in feedbackBuffers)
                {
                    if(item.TextureName != null)
                    {
                        yield return item.TextureName;
                    }
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
    }
}
