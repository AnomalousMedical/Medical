using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;
using Engine;
using Logging;
using OgrePlugin;

namespace Medical
{
    public class ViewportBackground
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

        public void createBackground(OgreSceneManager sceneManager)
        {
            this.sceneManager = sceneManager.SceneManager;
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
                sceneManager = null;
            }
        }

        public void setVisible(bool visible)
        {
            background.setVisible(visible);
        }

        public void updatePosition(Vector3 cameraPos, Vector3 cameraDirection, Quaternion rotation)
        {
            backgroundNode.setPosition(cameraPos + cameraDirection * distance);
            backgroundNode.setOrientation(rotation);
        }

        public ViewportBackground clone(String newName)
        {
            return new ViewportBackground(newName, materialName, distance, halfWidth * 2.0f, halfHeight * 2.0f, uvX, uvY);
        }

        public void preRenderCallback(DrawingWindow window, bool callingCameraRender)
        {
            setVisible(callingCameraRender);
            if (callingCameraRender)
            {
                updatePosition(window.Translation, window.Direction, window.Orientation);
            }
        }
    }
}
