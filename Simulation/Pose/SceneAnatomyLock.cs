using BEPUikPlugin;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class SceneAnatomyLock : BehaviorInterface, SceneAnatomyControl
    {
        [Editable]
        private String targetSimObjectName = "this";

        [DoNotSave]
        [DoNotCopy]
        List<BEPUikLimit> ikLimits = new List<BEPUikLimit>();

        [DoNotSave]
        [DoNotCopy]
        private Vector3 ownerOffset;

        protected override void link()
        {
            base.link();

            SimObject targetSimObject = Owner.getOtherSimObject(targetSimObjectName);
            if (targetSimObject == null)
            {
                blacklist("Could not find target SimObject {0}", targetSimObjectName);
            }

            if(targetSimObject != Owner)
            {
                ownerOffset = Quaternion.quatRotate(Owner.Rotation.inverse(), targetSimObject.Translation - Owner.Translation);
            }

            foreach (var element in targetSimObject.getElementIter())
            {
                BEPUikLimit limit = element as BEPUikLimit;
                if (limit != null)
                {
                    ikLimits.Add(limit);
                }
            }

            SceneAnatomyControlManager.addControl(this);
        }

        protected override void destroy()
        {
            SceneAnatomyControlManager.removeControl(this);
            base.destroy();
        }

        public bool Active
        {
            get
            {
                if(ikLimits.Count > 0)
                {
                    return ikLimits[0].Locked;
                }
                return false;
            }
            set
            {
                foreach(var limit in ikLimits)
                {
                    limit.Locked = value;
                }
            }
        }

        public Vector3 WorldPosition
        {
            get
            {
                return Owner.Translation + Quaternion.quatRotate(Owner.Rotation, ownerOffset);
            }
        }

        public SceneAnatomyControlType Type
        {
            get
            {
                return SceneAnatomyControlType.Lock;
            }
        }
    }
}
