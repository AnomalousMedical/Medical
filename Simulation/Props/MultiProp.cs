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
        private Dictionary<String, MultiPropSection> sections = new Dictionary<String, MultiPropSection>();

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

            addSection(new MultiPropSection("Woot1", "Box016.mesh", "Box016", new Vector3(-1, 0, 0), Quaternion.Identity, Vector3.ScaleIdentity, this));
            addSection(new MultiPropSection("Woot2", "Box016.mesh", "Box016", new Vector3(1, 0, 0), Quaternion.Identity, Vector3.ScaleIdentity, this));
            addSection(new MultiPropSection("Woot3", "PerfTooth01.mesh", "Tooth1collision", new Vector3(0, 0, 1), Quaternion.Identity, Vector3.ScaleIdentity, this));
        }

        protected override void willDestroy()
        {
            foreach(var section in sections.Values)
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

        private float currentScale = 0.0f;


        public override void update(Clock clock, EventManager eventManager)
        {
            currentScale += 0.1f * clock.DeltaSeconds;
            currentScale %= 1.0f;

            var section = sections["Woot3"];
            section.Scale = new Vector3(1, currentScale, 1);
            section.updatePosition(this);
            updateRigidBody();
        }

        private void updateRigidBody()
        {
            rigidBody.recomputeMassProps();
            rigidBody.forceActivationState(ActivationState.ActiveTag);
        }

        private void addSection(MultiPropSection section)
        {
            sections.Add(section.Name, section);
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
