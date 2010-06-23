﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.ObjectManagement;
using Engine.Renderer;
using Logging;

namespace Medical.Controller
{
    class SceneViewWindow : ScreenLayoutContainer, IDisposable
    {
        private SceneView sceneView;
        private CameraMover cameraMover;
        private UpdateTimer mainTimer;
        private RendererWindow window;
        private String name;

        public SceneViewWindow(UpdateTimer mainTimer, CameraMover cameraMover, String name)
        {
            this.cameraMover = cameraMover;
            this.name = name;
            this.mainTimer = mainTimer;
            mainTimer.addFixedUpdateListener(cameraMover);
        }

        public void Dispose()
        {
            mainTimer.removeFixedUpdateListener(cameraMover);
        }

        public void createSceneView(RendererWindow window, SimScene scene)
        {
            Log.Info("Creating SceneView for {0}.", name);
            this.window = window;
            SimSubScene defaultScene = scene.getDefaultSubScene();

            sceneView = window.createSceneView(defaultScene, name, cameraMover.Translation, cameraMover.LookAt);
            sceneView.BackgroundColor = Engine.Color.Black;
            sceneView.addLight();
            sceneView.setNearClipDistance(1.0f);
            sceneView.setFarClipDistance(1000.0f);
            //camera.setRenderingMode(renderingMode);
            cameraMover.setCamera(sceneView);
            //CameraResolver.addMotionValidator(this);
            //camera.showSceneStats(true);
            //basicGUI.ScreenLayout.Root.Center = new SceneViewLayoutItem(sceneView);
            //OgreCameraControl ogreCamera = ((OgreCameraControl)camera);
            //ogreCamera.PreFindVisibleObjects += camera_PreFindVisibleObjects;
            //if (CameraCreated != null)
            //{
            //    CameraCreated.Invoke(this);
            //}
        }

        public void destroySceneView()
        {
            if (sceneView != null)
            {
                Log.Info("Destroying SceneView for {0}.", name);
                cameraMover.setCamera(null);
                window.destroySceneView(sceneView);
                sceneView = null;
            }
        }

        public override void setAlpha(float alpha)
        {

        }

        public override void layout()
        {
            Size totalSize = TopmostWorkingSize;
            if (totalSize.Width == 0.0f)
            {
                totalSize.Width = 1.0f;
            }
            if (totalSize.Height == 0.0f)
            {
                totalSize.Height = 1.0f;
            }
            if (sceneView != null)
            {
                sceneView.setDimensions(Location.x / totalSize.Width, Location.y / totalSize.Height, WorkingSize.Width / totalSize.Width, WorkingSize.Height / totalSize.Height);
            }
        }

        public override Size DesiredSize
        {
            get
            {
                if (sceneView != null)
                {
                    return new Size(sceneView.RenderWidth, sceneView.RenderHeight);
                }
                return new Size();
            }
        }

        
    }
}
