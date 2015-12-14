using BulletPlugin;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class MultiProp : BehaviorInterface
    {
        [Editable]
        private String mainNodeName = "Node";

        [Editable]
        private String reshapeableRigidBodyName = "RigidBody";

        [DoNotCopy]
        [DoNotSave]
        private SceneNodeElement mainNode;

        [DoNotCopy]
        [DoNotSave]
        private ReshapeableRigidBody rigidBody;

        [DoNotCopy]
        [DoNotSave]
        private List<SceneNode> nodes = new List<SceneNode>();

        [DoNotCopy]
        [DoNotSave]
        private List<Entity> entities = new List<Entity>();

        [DoNotCopy]
        [DoNotSave]
        private OgreSceneManager ogreSceneManager;

        protected override void constructed()
        {
            base.constructed();

            mainNode = Owner.getElement(mainNodeName) as SceneNodeElement;
            if(mainNode == null)
            {
                blacklist("Cannot find main scene node {0}", mainNodeName);
            }

            rigidBody = Owner.getElement(reshapeableRigidBodyName) as ReshapeableRigidBody;
            if(rigidBody == null)
            {
                blacklist("Cannot find reshapeable rigid body {0}", reshapeableRigidBodyName);
            }

            ogreSceneManager = Owner.SubScene.getSimElementManager<OgreSceneManager>();

            setupShape("Woot1", "Box016.mesh", "Box016", new Vector3(-1, 0, 0), Quaternion.Identity);
            setupShape("Woot2", "Box016.mesh", "Box016", new Vector3(1, 0, 0), Quaternion.Identity);
        }

        private void setupShape(String name, String mesh, String collision, Vector3 translation, Quaternion rotation)
        {
            var node = ogreSceneManager.SceneManager.createSceneNode(String.Format("{0}_MultiPropNode_{1}", Owner.Name, name));
            node.setPosition(translation);
            node.setOrientation(rotation);
            mainNode.addChild(node);
            nodes.Add(node);

            var entity = ogreSceneManager.SceneManager.createEntity(String.Format("{0}_MultiPropEntity_{1}", Owner.Name, name), mesh);
            node.attachObject(entity);

            if(!rigidBody.addNamedShape(name, collision, translation, rotation))
            {
                
            }
        }

        protected override void willDestroy()
        {
            foreach(var entity in entities)
            {
                entity.detachFromParent();
                ogreSceneManager.SceneManager.destroyEntity(entity);
            }
            entities.Clear();

            foreach(var node in nodes)
            {
                mainNode.removeChild(node);
                ogreSceneManager.SceneManager.destroySceneNode(node);
            }
            nodes.Clear();

            base.willDestroy();
        }

        protected override void destroy()
        {
            base.destroy();
        }
    }
}
