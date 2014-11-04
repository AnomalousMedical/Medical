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

namespace Medical.Pose.Commands
{
    class EnableAndSynchronizePositionAction : PoseCommandActionBase
    {
        [Editable]
        private String controlSimObjectName = "this";

        [Editable]
        private String dragControlName = "DragControl";

        [DoNotCopy]
        [DoNotSave]
        private BEPUikDragControl control;

        [DoNotCopy]
        [DoNotSave]
        private Vector3 controlBoneOffset;

        protected override void link()
        {
            SimObject controlSimObject = Owner.getOtherSimObject(controlSimObjectName);
            if (controlSimObject == null)
            {
                blacklist("Cannot find Control SimObject named '{0}'", controlSimObjectName);
            }

            control = controlSimObject.getElement(dragControlName) as BEPUikDragControl;
            if (control == null)
            {
                blacklist("Cannot find BEPUikDragControl '{0}' in Control SimObject '{1}'", dragControlName, controlSimObjectName);
            }

            var boneOwner = control.Bone.Owner;

            controlBoneOffset = Quaternion.quatRotate(boneOwner.Rotation.inverse(), control.Owner.Translation - boneOwner.Translation);

            base.link();
        }

        public override void posingEnded()
        {
            Owner.Enabled = false;
        }

        public override void posingStarted()
        {
            Owner.Enabled = true;

            var boneOwner = control.Bone.Owner;
            control.TargetPosition = boneOwner.Translation + Quaternion.quatRotate(boneOwner.Rotation, controlBoneOffset);
        }
    }
}
