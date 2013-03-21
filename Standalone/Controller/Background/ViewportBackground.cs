using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;
using Engine;
using Logging;
using OgrePlugin;
using MyGUIPlugin;

namespace Medical.Controller
{
    public class ViewportBackground : IDisposable
    {
        private String name;
        private RenderTarget renderTarget;
        private Camera camera;
        private Viewport vp;
        private OgreRenderManager ogreRenderManager;
        private BackgroundScene backgroundScene;

        public ViewportBackground(String name, BackgroundScene backgroundScene, RenderTarget renderTarget, OgreRenderManager ogreRenderManager = null)
        {
            this.name = name;
            this.ogreRenderManager = ogreRenderManager;
            this.renderTarget = renderTarget;
            this.backgroundScene = backgroundScene;

            //Create camera and viewport
            camera = backgroundScene.SceneManager.createCamera(name + "BackgroundCamera");
            camera.setNearClipDistance(1.0f);
            camera.setAutoAspectRatio(true);
            camera.setFOVy(new Degree(10.0f));
            vp = renderTarget.addViewport(camera, 0, 0.0f, 0.0f, 1.0f, 1.0f);
            vp.setBackgroundColor(new Color(0.149f, 0.149f, 0.149f));
            vp.setOverlaysEnabled(false);
            vp.setClearEveryFrame(true);
            vp.clear();

            camera.lookAt(backgroundScene.BackgroundPosition);

            if (ogreRenderManager != null)
            {
                ogreRenderManager.setActiveViewport(ogreRenderManager.getActiveViewport() + 1);
            }
        }

        public void Dispose()
        {
            if (vp != null)
            {
                renderTarget.destroyViewport(vp);
                if (ogreRenderManager != null)
                {
                    ogreRenderManager.setActiveViewport(ogreRenderManager.getActiveViewport() - 1);
                }
            }
            if (camera != null)
            {
                backgroundScene.SceneManager.destroyCamera(camera);
            }
        }

        public Camera Camera
        {
            get
            {
                return camera;
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return vp.getBackgroundColor();
            }
            set
            {
                vp.setBackgroundColor(value);
            }
        }
    }
}
