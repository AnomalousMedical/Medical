using BEPUikPlugin;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Pose.Commands
{
    class PinBoneAction : PoseCommandActionBase
    {
        [Editable]
        private String boneSimObjectName = "this";

        [Editable]
        private String boneName = "IKBone";

        [DoNotCopy]
        [DoNotSave]
        private BEPUikBone bone;

        protected override void link()
        {
            var simObject = Owner.getOtherSimObject(boneSimObjectName);
            if (simObject == null)
            {
                blacklist("Cannot find Bone SimObject named '{0}'", boneSimObjectName);
            }

            bone = simObject.getElement(boneName) as BEPUikBone;
            if (bone == null)
            {
                blacklist("Cannot find BEPUik bone '{0}' on '{1}'", boneName, boneSimObjectName);
            }

            base.link();
        }

        public override void posingStarted()
        {
            bone.Pinned = true;
        }

        public override void posingEnded()
        {
            bone.Pinned = false;
        }
    }
}
