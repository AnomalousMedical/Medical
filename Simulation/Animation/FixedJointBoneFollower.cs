using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.Editing;
using OgreWrapper;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine.Attributes;
using PhysXWrapper;
using PhysXPlugin;

namespace Medical
{
    class FixedJointBoneFollower : Behavior
    {
        [Editable]
        String targetSimObject;

        [Editable]
        String targetNode;

        [Editable]
        String targetEntity;

        [Editable]
        String targetBone;

        [Editable]
        String jointName;

        SkeletonInstance skeleton;
        SimObject targetObject;
        Bone bone;

        [DoNotSave]
        [DoNotCopy]
        Vector3 offset;
        PhysD6JointDesc jointDesc = new PhysD6JointDesc();
        [DoNotSave]
        [DoNotCopy]
        Vector3 lastPosition;
        PhysD6JointElement joint;

        protected override void constructed()
        {
            targetObject = Owner.getOtherSimObject(targetSimObject);
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
                                offset = Owner.Translation - targetObject.Translation - bone.getDerivedPosition();
                                //calculate the scale at 100%
                                Vector3 adjust = (Vector3.ScaleIdentity - Owner.Scale) * offset;
                                offset += adjust;
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

            joint = Owner.getElement(jointName) as PhysD6JointElement;
            if (joint == null)
            {
                blacklist("Could not find D6 Joint {0} in SimObject {1}.", jointName, Owner.Name);
            }
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            Vector3 bonePos = bone.getDerivedPosition();
            if (bonePos != lastPosition)
            {
                this.updateScale(bone.getDerivedScale());

                joint.RealJoint.saveToDesc(jointDesc);
                jointDesc.set_LocalAnchor(0, bonePos + offset * Owner.Scale);
                joint.RealJoint.loadFromDesc(jointDesc);

                lastPosition = bonePos;
            }
        }
    }
}
