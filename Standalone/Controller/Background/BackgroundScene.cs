using Engine;
using MyGUIPlugin;
using OgrePlugin;
using OgreWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class BackgroundScene
    {
        private SceneNode backgroundNode;
        private ManualObject background;
        private SceneNode parentNode;
        private SceneManager sceneManager;

        private String name;
        private String materialName;
        private float halfWidth;
        private float halfHeight;
        private float uvX;
        private float uvY;
        private float distance;
        private bool visible = false;

        public BackgroundScene(String name, String materialName, float distance, float width, float height, float uvX, float uvY)
        {
            this.name = name;
            this.materialName = materialName;
            this.distance = distance;
            this.halfWidth = width / 2.0f;
            this.halfHeight = height / 2.0f;
            this.uvX = uvX;
            this.uvY = uvY;

            createBackground();
        }

        public void Dispose()
        {
            destroyBackground();
        }

        private void createBackground()
        {
            sceneManager = Root.getSingleton().createSceneManager(SceneType.ST_GENERIC, name + "BackgroundScene");

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

            Vector3 backgroundPosition = new Vector3(0, 0, -distance);
            backgroundNode.setPosition(backgroundPosition);
        }

        private void destroyBackground()
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

        public SceneManager SceneManager
        {
            get
            {
                return sceneManager;
            }
        }

        public Vector3 BackgroundPosition
        {
            get
            {
                if (backgroundNode != null)
                {
                    return backgroundNode.getPosition();
                }
                return Vector3.Zero;
            }
        }
    }
}
