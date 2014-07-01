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

        public FKLinkState(Vector3 localTranslation, Quaternion localRotation)
        {
            this.LocalTranslation = localTranslation;
            this.LocalRotation = localRotation;
        }

        public Vector3 LocalTranslation;

        public Quaternion LocalRotation;

        protected FKLinkState(LoadInfo info)
        {
            LocalTranslation = info.GetVector3("LocalTranslation");
            LocalRotation = info.GetQuaternion("LocalRotation");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("LocalTranslation", LocalTranslation);
            info.AddValue("LocalRotation", LocalRotation);
        }
    }
}
