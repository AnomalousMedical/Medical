using BEPUikPlugin;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using Medical.Pose.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Pose
{
    class IKPoseHandler : BehaviorInterface, PoseHandler
    {
        [Editable]
        private String boneSimObjectName = "this";

        [Editable]
        private String boneName = "IKBone";

        [DoNotCopy]
        [DoNotSave]
        private PoseCommand poseCommand = new PoseCommand();

        [DoNotCopy]
        [DoNotSave]
        private BEPUikBone bone;

        protected override void link()
        {
            base.link();

            SimObject boneSimObject = Owner.getOtherSimObject(boneSimObjectName);
            if (boneSimObject == null)
            {
                blacklist("Cannot find Bone SimObject named '{0}'", boneSimObjectName);
            }

            bone = boneSimObject.getElement(boneName) as BEPUikBone;
            if (bone == null)
            {
                blacklist("Cannot find BEPUikBone '{0}' in Bone SimObject '{1}'", boneName, boneSimObjectName);
            }
        }

        public void addPoseCommandAction(PoseCommandAction action)
        {
            poseCommand.addAction(action);
        }

        public void removePoseCommandAction(PoseCommandAction action)
        {
            poseCommand.removeAction(action);
        }

        public void posingStarted()
        {
            poseCommand.posingStarted();
        }

        public void posingEnded()
        {
            poseCommand.posingEnded();
        }

        public BEPUikBone Bone
        {
            get
            {
                return bone;
            }
        }
    }
}
