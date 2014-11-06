using BulletPlugin;
using Engine;
using Engine.Attributes;
using Engine.Platform;
using Medical.Pose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// This is an FK Root that will schedule its update to run before the scene's bullet
    /// physics scene updates.
    /// </summary>
    public class FKRootBehavior : Behavior, FKRoot
    {
        private const float FullBlend = 0.99999f;

        [DoNotCopy]
        [DoNotSave]
        private Action updateAction;

        [DoNotCopy]
        [DoNotSave]
        private List<FKElement> children = new List<FKElement>();

        /// <summary>
        /// This will fire when a chain state is applied to this element.
        /// </summary>
        [DoNotCopy]
        [DoNotSave]
        public event FKElementUpdatedDelegate ChainStateApplied;

        protected override void link()
        {
            base.link();

            PoseableObjectsManager.addFkChainRoot(this);
        }

        protected override void destroy()
        {
            base.destroy();
            PoseableObjectsManager.removeFkChainRoot(this);
        }

        public void addChild(FKElement child)
        {
            children.Add(child);
        }

        public void removeChild(FKElement child)
        {
            children.Remove(child);
        }

        public Vector3 Translation
        {
            get
            {
                return Owner.Translation;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return Owner.Rotation;
            }
        }

        public String RootName
        {
            get
            {
                return Owner.Name;
            }
        }

        public IEnumerable<FKElement> Children
        {
            get
            {
                return children;
            }
        }

        public void addToChainState(FKChainState chain)
        {
            chain.setLinkState(Owner.Name, Owner.Translation, Owner.Rotation);

            foreach(var child in children)
            {
                child.addToChainState(chain);
            }
        }

        public void applyChainState(FKChainState chain)
        {
            updateAction = () =>
            {
                FKLinkState linkState = chain[Owner.Name];

                Vector3 trans = linkState.LocalTranslation;
                Quaternion rot = linkState.LocalRotation;

                this.updatePosition(ref trans, ref rot);

                if(ChainStateApplied != null)
                {
                    ChainStateApplied.Invoke(this, chain);
                }

                foreach (var child in children)
                {
                    child.applyChainState(chain);
                }
            };
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            if(updateAction != null)
            {
                updateAction.Invoke();
                updateAction = null;
            }
        }
    }
}
