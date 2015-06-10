﻿using Anomalous.GuiFramework.Cameras;
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
    class VirtualTextureManager : IDisposable
    {
        public const String ResourceGroup = "VirtualTextureGroup";

        FeedbackBuffer[] feedbackBuffers = new FeedbackBuffer[2];
        int frameCount = 0;
        int updateBufferFrame = 1;
        bool readbackThisFrame = false;
        int currentRenderTexture = 0;
        int currentReadbackTexture = 0;
        bool allowImageRender = true;

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
                feedbackBuffers[i] = new FeedbackBuffer(window, this);
            }

            currentRenderTexture = feedbackBuffers.Length - 1; //Separate rendering from readback

            //Create physical textures
            physicalTextures.Add("NormalMap", new PhysicalTexture("NormalMap", new IntSize2(4096, 4096)));
            physicalTextures.Add("Diffuse", new PhysicalTexture("Diffuse", new IntSize2(4096, 4096)));
            physicalTextures.Add("Specular", new PhysicalTexture("Specular", new IntSize2(4096, 4096)));
            physicalTextures.Add("Opacity", new PhysicalTexture("Opacity", new IntSize2(4096, 4096)));

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

                    //if (readbackThisFrame)
                    {
                        feedbackBuffers[currentReadbackTexture].copyFromGpu();
                        currentReadbackTexture = (currentReadbackTexture + 1) % feedbackBuffers.Length;
                        readbackThisFrame = false;
                    }
                    //else
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
                indirectionTex = new IndirectionTexture(new IntSize2(2048, 2048), 64, this); //Don't hardcode size
                indirectionTextures.Add(materialSetKey, indirectionTex);
                indirectionTexturesById.Add(indirectionTex.Id, indirectionTex);
            }
            indirectionTex.reconfigureTechnique(mainTechnique, feedbackTechnique);
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
    }
}