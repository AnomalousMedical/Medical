using BulletPlugin;
using Engine;
using Engine.Attributes;
using Engine.Platform;
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

                foreach (var child in children)
                {
                    child.applyChainState(chain);
                }
            };
        }

        public void blendChainStates(FKChainState start, FKChainState end, float blend)
        {
            //Since blending has to go through this root make sure we aren't just trying to get to the end.
            if (blend > FullBlend)
            {
                blend = 1.0f;
            }

            updateAction = () =>
            {
                FKLinkState startState = start[Owner.Name];
                FKLinkState endState = end[Owner.Name];

                Vector3 trans = startState.getBlendedLocalTranslation(endState, blend);
                Quaternion rot = startState.getBlendedLocalRotation(endState, blend);

                this.updatePosition(ref trans, ref rot);

                foreach (var child in children)
                {
                    child.blendChainStates(start, end, blend);
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
