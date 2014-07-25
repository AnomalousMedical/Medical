using Engine;
using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// The state of a link in an FK chain, this class is only to be used by FKChainState and violates
    /// encapsulation a bit for performance to be able to pass by value for the math classes.
    /// </summary>
    class FKLinkState : Saveable
    {
        /// <summary>
        /// This state will not modify the position of objects it is applied to.
        /// </summary>
        public static readonly FKLinkState IdentityState = new FKLinkState(Vector3.Zero, Quaternion.Identity);

        private Vector3 localTranslation;
        private Quaternion localRotation;

        public FKLinkState(Vector3 localTranslation, Quaternion localRotation)
        {
            this.localTranslation = localTranslation;
            this.localRotation = localRotation;
        }

        public Vector3 LocalTranslation
        {
            get
            {
                return localTranslation;
            }
            set
            {
                localTranslation = value;
            }
        }

        public Quaternion LocalRotation
        {
            get
            {
                return localRotation;
            }
            set
            {
                localRotation = value;
            }
        }

        public Vector3 getBlendedLocalTranslation(FKLinkState endState, float blendFactor)
        {
            return localTranslation.lerp(ref endState.localTranslation, ref blendFactor);
        }

        public Quaternion getBlendedLocalRotation(FKLinkState endState, float blendFactor)
        {
            return localRotation.nlerp(ref endState.localRotation, ref blendFactor);
        }

        protected FKLinkState(LoadInfo info)
        {
            localTranslation = info.GetVector3("LocalTranslation");
            localRotation = info.GetQuaternion("LocalRotation");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("LocalTranslation", localTranslation);
            info.AddValue("LocalRotation", localRotation);
        }
    }
}
