using BEPUikPlugin;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using OgrePlugin;
using OgreWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// This class will search for poseables by raycasting onto an entity.
    /// </summary>
    class PoseableByAnatomyIdentifier : BehaviorInterface, PoseableIdentifier
    {
        [Editable]
        private String anatomySimObjectName = "this";

        [Editable]
        private String anatomyName = "Anatomy";

        [Editable]
        private String boneSimObjectName = "this";

        [Editable]
        private String boneName = "IKBone";

        [Editable]
        private String controlSimObjectName;

        [Editable]
        private String controlName;

        [Editable]
        private String poseCommandName = null;

        [DoNotCopy]
        [DoNotSave]
        private AnatomyIdentifier anatomy;

        [DoNotCopy]
        [DoNotSave]
        private BEPUikBone bone;

        [DoNotCopy]
        [DoNotSave]
        private BEPUikDragControl control;

        [DoNotCopy]
        [DoNotSave]
        private Vector3 controlBoneOffset;

        protected override void link()
        {
            base.link();
            SimObject entitySimObject = Owner.getOtherSimObject(anatomySimObjectName);
            if(entitySimObject == null)
            {
                blacklist("Cannot find Anatomy SimObject named '{0}'", anatomySimObjectName);
            }

            anatomy = entitySimObject.getElement(anatomyName) as AnatomyIdentifier;
            if (anatomy == null)
            {
                blacklist("Cannot find AnatomyIdentifier '{0}' on Anatomy SimObject '{1}'", anatomyName, anatomySimObjectName);
            }

            SimObject boneSimObject = Owner.getOtherSimObject(boneSimObjectName);
            if(boneSimObject == null)
            {
                blacklist("Cannot find Bone SimObject named '{0}'", boneSimObjectName);
            }

            bone = boneSimObject.getElement(boneName) as BEPUikBone;
            if (bone == null)
            {
                blacklist("Cannot find BEPUikBone '{0}' in Bone SimObject '{1}'", boneName, boneSimObjectName);
            }

            //Control is optional, so make sure both names are defined.
            if(!String.IsNullOrEmpty(controlSimObjectName) && !String.IsNullOrEmpty(controlName))
            {
                SimObject controlSimObject = Owner.getOtherSimObject(controlSimObjectName);
                if (controlSimObject == null)
                {
                    blacklist("Cannot find Control SimObject named '{0}'", controlSimObjectName);
                }

                control = controlSimObject.getElement(controlName) as BEPUikDragControl;
                if (control == null)
                {
                    blacklist("Cannot find BEPUikDragControl '{0}' in Control SimObject '{1}'", controlName, controlSimObjectName);
                }

                controlBoneOffset = Quaternion.quatRotate(bone.Owner.Rotation.inverse(), control.Owner.Translation - bone.Owner.Translation);
            }

            PoseableObjectsManager.add(this);
        }

        protected override void destroy()
        {
            PoseableObjectsManager.remove(this);
            base.destroy();
        }

        public bool checkCollision(Ray3 ray, ref float distanceOnRay)
        {
            return anatomy.CurrentAlpha > 0.0f && anatomy.checkCollision(ray, ref distanceOnRay);
        }

        public void syncControlToBone()
        {
            if(control != null)
            {
                var boneOwner = bone.Owner;
                control.TargetPosition = boneOwner.Translation + Quaternion.quatRotate(boneOwner.Rotation, controlBoneOffset);
            }
        }

        public BEPUikBone Bone
        {
            get
            {
                return bone;
            }
        }

        public BEPUikDragControl Control
        {
            get
            {
                return control;
            }
        }

        public String PoseCommandName
        {
            get
            {
                return poseCommandName;
            }
        }
    }
}
