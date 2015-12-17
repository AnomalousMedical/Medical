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

            if (sections.Count > 0)
            {
                beginUpdates();

                foreach (var section in sections.Values)
                {
                    section.create(this);
                }

                finishUpdates();
            }
        }

        protected override void willDestroy()
        {
            beginUpdates();

            foreach (var section in sections.Values)
            {
                section.destroy(this);
            }
            sections.Clear();

            finishUpdates();

            base.willDestroy();
        }

        protected override void destroy()
        {
            base.destroy();
        }

        public void beginUpdates()
        {
            rigidBody.beginUpdates();
        }

        public void finishUpdates()
        {
            rigidBody.finishUpdates();
            rigidBody.forceActivationState(ActivationState.ActiveTag);
        }

        public MultiPropSection addSection(MultiPropSection section)
        {
            sections.Add(section.Name, section);
            section.create(this);
            return section;
        }

        public void removeSection(String name)
        {
            MultiPropSection section;
            if(sections.TryGetValue(name, out section))
            {
                removeSection(section);
            }
        }

        public void removeSection(MultiPropSection section)
        {
            if(sections.Remove(section.Name))
            {
                section.destroy(this);
            }
        }

        public bool tryGetSection(String name, out MultiPropSection value)
        {
            return sections.TryGetValue(name, out value);
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
