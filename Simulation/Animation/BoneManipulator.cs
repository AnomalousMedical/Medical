using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using OgreWrapper;
using Engine.ObjectManagement;
using OgrePlugin;
using Logging;
using Engine.Attributes;

namespace Medical
{
    abstract class BoneManipulator : Interface, AnimationManipulator
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
        float currentPosition = 0.0f;

        //The tag on the UI control for this manipulator. Must be unique.
        [Editable]
        String uiName;

        SkeletonInstance skeleton;
        protected Bone bone;

        protected override void constructed()
        {
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
                                AnimationManipulatorController.addAnimationManipulator(this);
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
                AnimationManipulatorController.removeAnimationManipulator(this);
            }
        }

        protected abstract void positionUpdated(float position);

        public abstract AnimationManipulatorStateEntry createStateEntry();

        public float Position
        {
            get
            {
                return currentPosition;
            }
            set
            {
                currentPosition = value;
                positionUpdated(currentPosition);
            }
        }

        public abstract float DefaultPosition
        {
            get;
        }

        public String UIName
        {
            get
            {
                return uiName;
            }
        }
    }
}
