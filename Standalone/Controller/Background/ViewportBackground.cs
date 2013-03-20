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
        private SceneNode backgroundNode;
        private ManualObject background;
        private SceneNode parentNode;
        private SceneManager sceneManager;
        private OgreWindow ogreWindow;
        private Camera camera;
        private Viewport vp;
        private OgreRenderManager ogreRenderManager;

        private String name;
        private String materialName;
        private float halfWidth;
        private float halfHeight;
        private float uvX;
        private float uvY;
        private float distance;
        private bool visible = false;

        public ViewportBackground(String name, String materialName, float distance, float width, float height, float uvX, float uvY)
        {
            this.name = name;
            this.materialName = materialName;
            this.distance = distance;
            this.halfWidth = width / 2.0f;
            this.halfHeight = height / 2.0f;
            this.uvX = uvX;
            this.uvY = uvY;
        }

        public void Dispose()
        {
            destroyBackground();
        }

        public void createBackground(OgreRenderManager ogreRenderManager)
        {
            this.ogreRenderManager = ogreRenderManager;

            sceneManager = Root.getSingleton().createSceneManager(SceneType.ST_GENERIC, name + "BackgroundScene");
            ogreWindow = PluginManager.Instance.RendererPlugin.PrimaryWindow as OgreWindow;

            //Create camera and viewport
            camera = sceneManager.createCamera(name + "BackgroundCamera");
            camera.setNearClipDistance(1.0f);
            camera.setAutoAspectRatio(true);
            camera.setFOVy(new Degree(10.0f));
            vp = ogreWindow.OgreRenderWindow.addViewport(camera, 0, 0.0f, 0.0f, 1.0f, 1.0f);
            vp.setBackgroundColor(new Color(0.149f, 0.149f, 0.149f));
            vp.setOverlaysEnabled(false);
            vp.setClearEveryFrame(true);
            vp.clear();

            parentNode = this.sceneManager.getRootSceneNode();

            background = this.sceneManager.createManualObject(name);
            background.begin(materialName, OperationType.OT_TRIANGLE_LIST);

            //bottom left
            background.position(-halfWidth, -halfHeight, 0);
            background.textureCoord(0, uvY);

            //top left
            background.position(-halfWidth, halfHeight, 0);
            background.textureCoord(0, 0);

            //bottom right
            background.position(halfWidth, -halfHeight, 0);
            background.textureCoord(uvX, uvY);

            //top left
            background.position(-halfWidth, halfHeight, 0);
            background.textureCoord(0, 0);

            //top right
            background.position(halfWidth, halfHeight, 0);
            background.textureCoord(uvX, 0);

            //bottom right
            background.position(halfWidth, -halfHeight, 0);
            background.textureCoord(uvX, uvY);
            background.setRenderQueueGroup(0);

            background.end();

            backgroundNode = this.sceneManager.createSceneNode(name + "Node");
            backgroundNode.attachObject(background);

            parentNode.addChild(backgroundNode);
            backgroundNode.setVisible(visible);

            Vector3 backgroundPosition = new Vector3(0, 0, -1000);
            backgroundNode.setPosition(backgroundPosition);
            camera.lookAt(backgroundPosition);

            ogreRenderManager.setActiveViewport(ogreRenderManager.getActiveViewport() + 1);
        }

        public void destroyBackground()
        {
            if (backgroundNode != null)
            {
                parentNode.removeChild(backgroundNode);
                sceneManager.destroyManualObject(background);
                sceneManager.destroySceneNode(backgroundNode);
                background = null;
                backgroundNode = null;
                parentNode = null;
            }
            if (vp != null)
            {
                ogreWindow.OgreRenderWindow.destroyViewport(vp);
                ogreRenderManager.setActiveViewport(ogreRenderManager.getActiveViewport() - 1);
            }
            if (camera != null)
            {
                sceneManager.destroyCamera(camera);
            }
            if (sceneManager != null)
            {
                Root.getSingleton().destroySceneManager(sceneManager);
            }
        }

        public void setVisible(bool visible)
        {
            this.visible = visible;
            if (background != null)
            {
                background.setVisible(visible);
            }
        }

        public void updatePosition(Vector3 cameraPos, Vector3 cameraDirection, Quaternion rotation)
        {
            //backgroundNode.setPosition(cameraPos + cameraDirection * distance);
            //backgroundNode.setOrientation(rotation);
        }
    }
}
