using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using OgreWrapper;
using Engine;
using OgrePlugin;
using Engine.ObjectManagement;
using Engine.Attributes;
using Logging;

namespace Medical
{
    class BoneRotator : Interface, BoneManipulator
    {
        const float DEG_TO_RAD = 0.0174532925f;

        [Editable]
        String targetSimObject;

        [Editable]
        String targetNode;

        [Editable]
        String targetEntity;

        [Editable]
        String targetBone;

        [Editable]
        Vector3 startRotation = Vector3.Zero;

        [Editable]
        Vector3 endRotation = Vector3.Zero;

        [Editable]
        float currentPosition = 0.0f;

        [Editable]
        String niceName;

        SkeletonInstance skeleton;
        Bone bone;

        [DoNotCopy]
        [DoNotSave]
        Vector3 startRotationRad;

        [DoNotCopy]
        [DoNotSave]
        Vector3 endRotationRad;

        [DoNotCopy]
        [DoNotSave]
        Quaternion newRotQuat = Quaternion.Identity;

        protected override void constructed()
        {
            startRotationRad = startRotation * DEG_TO_RAD;
            endRotationRad = endRotation * DEG_TO_RAD;
            SimObject targetObject = Owner.getOtherSimObject(targetSimObject);
            if (targetObject != null)
            {
                SceneNodeElement node = targetObject.getElement(targetNode) as SceneNodeElement;
                if (node != null)
                {
                    Entity entity = node.getNodeObject(targetEntity) as Entity;
                    if (entity != null)
                    {
                        if (entity.hasSkeleton())
                        {
                            skeleton = entity.getSkeleton();
                            if (skeleton.hasBone(targetBone))
                            {
                                bone = skeleton.getBone(targetBone);
                                bone.setManuallyControlled(true);
                                BoneManipulatorController.addBoneManipulator(this.Name, this);
                                Log.Default.debug("Bone {0} rotation is {1}.", bone.getName(), bone.getOrientation().getEuler() * 57.2957795f);
                            }
                            else
                            {
                                blacklist("Entity {0} does not have a bone named {1}.", targetEntity, targetBone);
                            }
                        }
                        else
                        {
                            blacklist("Entity {0} does not have a skeleton.", targetEntity);
                        }
                    }
                    else
                    {
                        blacklist("Could not find Entity {0}.", targetEntity);
                    }
                }
                else
                {
                    blacklist("Could not find target SceneNodeElement {0}.", targetNode);
                }
            }
            else
            {
                blacklist("Could not find Target SimObject {0}.", targetSimObject);
            }
        }

        protected override void destroy()
        {
            if (bone != null)
            {
                BoneManipulatorController.removeBoneManipulator(this.Name);
            }
        }

        public void setPosition(float position)
        {
            if (bone != null)
            {
                currentPosition = position;
                Vector3 newRot = startRotationRad.lerp(ref endRotationRad, ref position);
                newRotQuat.setEuler(newRot.x, newRot.y, newRot.z);
                bone.setOrientation(newRotQuat);
                bone.needUpdate(true);
            }
        }

        public String getUIName()
        {
            return niceName;
        }
    }
}
