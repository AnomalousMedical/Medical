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
        private BackgroundScene backgroundScene;

        public ViewportBackground(String name, int zOrder, BackgroundScene backgroundScene, RenderTarget renderTarget)
        {
            this.name = name;
            this.renderTarget = renderTarget;
            this.backgroundScene = backgroundScene;

            //Create camera and viewport
            camera = backgroundScene.SceneManager.createCamera(name + "BackgroundCamera");
            camera.setNearClipDistance(1.0f);
            camera.setAutoAspectRatio(true);
            camera.setFOVy(new Degree(10.0f));
            vp = renderTarget.addViewport(camera, zOrder, 0.0f, 0.0f, 1.0f, 1.0f);
            vp.setBackgroundColor(new Color(0.149f, 0.149f, 0.149f));
            vp.setOverlaysEnabled(false);
            vp.setClearEveryFrame(false);
            vp.clear();

            camera.lookAt(backgroundScene.BackgroundPosition);
        }

        public void Dispose()
        {
            if (vp != null)
            {
                renderTarget.destroyViewport(vp);
            }
            if (camera != null)
            {
                backgroundScene.SceneManager.destroyCamera(camera);
            }
        }

        public void setDimensions(float left, float top, float width, float height)
        {
            vp.setDimensions(left, top, width, height);
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
