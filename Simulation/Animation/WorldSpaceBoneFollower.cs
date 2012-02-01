using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Attributes;
using OgreWrapper;
using Engine.ObjectManagement;
using Engine;
using Engine.Platform;
using OgrePlugin;

namespace Medical
{
    /// <summary>
    /// Follows a bone, but translates the bone position to world space.
    /// </summary>
    class WorldSpaceBoneFollower : Behavior
    {
        [Editable]
        String targetSimObject;

        [Editable]
        String targetNode;

        [Editable]
        String targetEntity;

        [Editable]
        String targetBone;

        SkeletonInstance skeleton;
        SimObject targetObject;
        Bone bone;

        [DoNotSave]
        [DoNotCopy]
        Vector3 offset;

        [DoNotSave]
        [DoNotCopy]
        Vector3 lastPosition;

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
                                //Calculate the offset from this object to the bone
                                offset = Owner.Translation - (targetObject.Translation + bone.getDerivedPosition());
                                //calculate the scale at 100%
                                Vector3 adjust = (Vector3.ScaleIdentity - Owner.Scale) * offset;
                                offset += adjust;
                                //Rotate the offset into the space for the object
                                offset = Quaternion.quatRotate(targetObject.Rotation.inverse(), offset);
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

        public override void update(Clock clock, EventManager eventManager)
        {
            Vector3 bonePos = targetObject.Translation + Quaternion.quatRotate(targetObject.Rotation, bone.getDerivedPosition() + offset * Owner.Scale);
            if (bonePos != lastPosition)
            {
                this.updateScale(bone.getDerivedScale());
                this.updateTranslation(bonePos);
                Quaternion rotation = targetObject.Rotation;
                this.updatePosition(ref bonePos, ref rotation);
                lastPosition = bonePos;
            }
        }
    }
}
