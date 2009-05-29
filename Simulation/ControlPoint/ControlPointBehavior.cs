using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.Editing;
using OgreWrapper;
using OgrePlugin;
using Engine.ObjectManagement;
using PhysXWrapper;
using PhysXPlugin;

namespace Medical
{
    class ControlPointBehavior : Behavior
    {
        [Editable]
        String boneSimObject;

        [Editable]
        String boneNode;

        [Editable]
        String boneEntity;

        [Editable]
        String targetBone;

        [Editable]
        String jointName;

        SimObject boneObject;
        Bone bone;
        PhysD6JointElement joint;
        Vector3 lastPosition = Vector3.Zero;
        PhysD6JointDesc jointDesc = new PhysD6JointDesc();

        protected override void constructed()
        {
            boneObject = Owner.getOtherSimObject(boneSimObject);
            if (boneObject != null)
            {
                SceneNodeElement node = boneObject.getElement(boneNode) as SceneNodeElement;
                if (node != null)
                {
                    Entity entity = node.getNodeObject(boneEntity) as Entity;
                    if (entity != null)
                    {
                        if (entity.hasSkeleton())
                        {
                            SkeletonInstance skeleton = entity.getSkeleton();
                            if (skeleton.hasBone(targetBone))
                            {
                                bone = skeleton.getBone(targetBone);
                            }
                            else
                            {
                                blacklist("Entity {0} does not have a bone named {1}.", boneEntity, targetBone);
                            }
                        }
                        else
                        {
                            blacklist("Entity {0} does not have a skeleton.", boneEntity);
                        }
                    }
                    else
                    {
                        blacklist("Could not find Entity {0}.", boneEntity);
                    }
                }
                else
                {
                    blacklist("Could not find target SceneNodeElement {0}.", boneNode);
                }
            }
            else
            {
                blacklist("Could not find Target SimObject {0}.", boneSimObject);
            }
            joint = Owner.getElement(jointName) as PhysD6JointElement;
            if (joint == null)
            {
                blacklist("Could not find joint {0}.", jointName);
            }
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            //Vector3 location = Quaternion.quatRotate(boneObject.Rotation, bone.getDerivedPosition()) + boneObject.Translation;
            Vector3 bonePos = bone.getDerivedPosition();
            if (bonePos != lastPosition)
            {
                joint.RealJoint.saveToDesc(jointDesc);
                jointDesc.set_LocalAnchor(0, bonePos);
                joint.RealJoint.loadFromDesc(jointDesc);

                lastPosition = bonePos;
            }
        }
    }
}
