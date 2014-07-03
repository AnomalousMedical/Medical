using BulletPlugin;
using Engine;
using Engine.Attributes;
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
    public class FKBulletRoot : Interface, FKRoot
    {
        [DoNotCopy]
        [DoNotSave]
        private BulletScene bulletScene;

        [DoNotCopy]
        [DoNotSave]
        private List<FKElement> children = new List<FKElement>();

        protected override void link()
        {
            base.link();
            bulletScene = Owner.SubScene.getSimElementManager<BulletScene>();
            if(bulletScene == null)
            {
                blacklist("Cannot find bullet scene.");
            }

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
            bulletScene.addPreSynchronizeTask(() =>
            {
                FKLinkState linkState = chain[Owner.Name];

                this.updatePosition(ref linkState.LocalTranslation, ref linkState.LocalRotation);

                foreach (var child in children)
                {
                    child.applyChainState(chain);
                }
            });
        }
    }
}
