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
using Engine.Platform;

namespace Medical
{
    class MultiProp : Behavior
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
        private List<MultiPropSection> sections = new List<MultiPropSection>();

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

            sections.Add(new MultiPropSection("Woot1", "Box016.mesh", "Box016", new Vector3(-1, 0, 0), Quaternion.Identity, this));
            sections.Add(new MultiPropSection("Woot2", "Box016.mesh", "Box016", new Vector3(1, 0, 0), Quaternion.Identity, this));
            sections.Add(new MultiPropSection("Woot3", "PerfTooth01.mesh", "Tooth1collision", new Vector3(0, 0, 1), Quaternion.Identity, this));

            //setupShape("Woot1", "Box016.mesh", "Box016", new Vector3(-1, 0, 0), Quaternion.Identity);
            //setupShape("Woot2", "Box016.mesh", "Box016", new Vector3(1, 0, 0), Quaternion.Identity);
            //setupShape("Woot3", "PerfTooth01.mesh", "Tooth1collision", new Vector3(0, 0, 1), Quaternion.Identity);
        }

        //private void setupShape(String name, String mesh, String collision, Vector3 translation, Quaternion rotation)
        //{
        //    var node = ogreSceneManager.SceneManager.createSceneNode(String.Format("{0}_MultiPropNode_{1}", Owner.Name, name));
        //    node.setPosition(translation);
        //    node.setOrientation(rotation);
        //    mainNode.addChild(node);
        //    nodes.Add(node);

        //    var entity = ogreSceneManager.SceneManager.createEntity(String.Format("{0}_MultiPropEntity_{1}", Owner.Name, name), mesh);
        //    node.attachObject(entity);

        //    if(!rigidBody.addNamedShape(name, collision, translation, rotation))
        //    {
        //        Logging.Log.Error("Cannot find collision shape '{0}'", collision);
        //    }
        //}

        private float currentScale = 0.0f;
            

        public override void update(Clock clock, EventManager eventManager)
        {
            //rigidBody.moveOrigin("Woot3", new Vector3(0, 2, 0), Quaternion.Identity);
            currentScale += 0.1f * clock.DeltaSeconds;
            currentScale %= 1.0f;
            rigidBody.setLocalScaling("Woot3", new Vector3(1, currentScale, 1));
            rigidBody.forceActivationState(ActivationState.ActiveTag);

            rigidBody.recomputeMassProps();
        }

        protected override void willDestroy()
        {
            foreach(var section in sections)
            {
                section.destroy(this);
            }
            sections.Clear();

            base.willDestroy();
        }

        protected override void destroy()
        {
            base.destroy();
        }

        internal OgreSceneManager OgreSceneManager
        {
            get
            {
                return ogreSceneManager;
            }
        }

        internal SceneNodeElement MainNode
        {
            get
            {
                return mainNode;
            }
        }

        internal ReshapeableRigidBody RigidBody
        {
            get
            {
                return rigidBody;
            }
        }
    }
}
