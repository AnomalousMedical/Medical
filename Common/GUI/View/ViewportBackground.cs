using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;
using Engine;
using Logging;

namespace Medical
{
    class ViewportBackground
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
        private Camera camera;

        public ViewportBackground(String name, String materialName, float width, float height, float uvX, float uvY)
        {
            this.name = name;
            this.materialName = materialName;
            this.halfWidth = width / 2.0f;
            this.halfHeight = height / 2.0f;
            this.uvX = uvX;
            this.uvY = uvY;
        }

        public void createBackground(SceneNode parentNode, SceneManager sceneManager, Camera camera)
        {
            parentNode = sceneManager.getRootSceneNode();

            this.parentNode = parentNode;
            this.sceneManager = sceneManager;
            this.camera = camera;

            background = sceneManager.createManualObject(name);
            background.begin(materialName, OperationType.OT_TRIANGLE_LIST);

            //bottom left
            background.position(-halfWidth, -halfHeight, 0);
            //background.normal(ref Vector3.Backward);
            background.textureCoord(0, uvY);

            //top left
            background.position(-halfWidth, halfHeight, 0);
            //background.normal(ref Vector3.Backward);
            background.textureCoord(0, 0);

            //bottom right
            background.position(halfWidth, -halfHeight, 0);
           // background.normal(ref Vector3.Backward);
            background.textureCoord(uvX, uvY);

            //top left
            background.position(-halfWidth, halfHeight, 0);
            //background.normal(ref Vector3.Backward);
            background.textureCoord(0, 0);

            //top right
            background.position(halfWidth, halfHeight, 0);
            //background.normal(ref Vector3.Backward);
            background.textureCoord(uvX, 0);

            //bottom right
            background.position(halfWidth, -halfHeight, 0);
            //background.normal(ref Vector3.Backward);
            background.textureCoord(uvX, uvY);
            background.setRenderQueueGroup(0);

            background.end();

            backgroundNode = sceneManager.createSceneNode(name + "Node");
            backgroundNode.attachObject(background);
            backgroundNode.setPosition(new Vector3(0, 0, -997));
            backgroundNode.setInheritOrientation(false);
            //backgroundNode.setOrientation(new Quaternion(1.5707f, 0, 0));

            parentNode.addChild(backgroundNode);

            Log.Debug("BG Pos {0} {1}.", backgroundNode.getDerivedPosition(), backgroundNode.getDerivedOrientation().getEuler());
            Log.Debug("Parent pos {0} {1}.", parentNode.getDerivedPosition(), parentNode.getDerivedOrientation().getEuler());
        }

        public void destroyBackground()
        {
            if (backgroundNode != null)
            {
                Log.Debug("BG Pos {0} {1}.", backgroundNode.getDerivedPosition(), backgroundNode.getDerivedOrientation().getEuler());
                Log.Debug("Parent pos {0} {1}.", parentNode.getDerivedPosition(), parentNode.getDerivedOrientation().getEuler());

                parentNode.removeChild(backgroundNode);
                sceneManager.destroyManualObject(background);
                sceneManager.destroySceneNode(backgroundNode);
                background = null;
                backgroundNode = null;
                parentNode = null;
                sceneManager = null;
                camera = null;
            }
        }

        public void setVisible(bool visible)
        {
            background.setVisible(visible);
            if (visible)
            {
                //backgroundNode.lookAt(parentNode.getDerivedPosition(), Node.TransformSpace.TS_WORLD);
                //Quaternion rot = backgroundNode.getOrientation();
                //Vector3 euler = rot.getEuler();
                //euler.y = 0;
                //rot.setEuler(euler.x, euler.y, euler.z);
                //backgroundNode.setOrientation(rot);
                backgroundNode.setPosition(camera.getRealPosition() + camera.getRealDirection() * 900);
                backgroundNode.setOrientation(camera.getRealOrientation());
            }
        }
    }
}
