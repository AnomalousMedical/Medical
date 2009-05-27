using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using OgreWrapper;
using Engine.ObjectManagement;
using OgrePlugin;

namespace Medical
{
    class BoneScalar : Interface
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
        Vector3 startScale = Vector3.ScaleIdentity;

        [Editable]
        Vector3 endScale = Vector3.Zero;

        [Editable]
        float currentPosition = 0.0f;

        SkeletonInstance skeleton;
        Bone bone;

        protected override void constructed()
        {
            SimObject targetObject = SimObject.getOtherSimObject(targetSimObject);
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

        public void setPosition(float position)
        {
            if (bone != null)
            {
                currentPosition = position;
                bone.setScale(startScale.lerp(ref endScale, ref position));
                bone.needUpdate(true);
            }
        }
    }
}
