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
using BulletPlugin;

namespace Medical
{
    public class ControlPointBehavior : Behavior
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

        [Editable]
        String discObject;

        [Editable]
        String discName;

        [Editable]
        //The position along the curve where the condyle starts
        float neutralLocation;

        [Editable]
        float location;

        SimObject boneObject;
        Bone bone;
        Generic6DofConstraintElement joint;
        Vector3 lastPosition = Vector3.Zero;

        Disc disc;

        bool translate = false;
        float targetLocation = 0.0f;
        float moveSpeed = 0.0f;

        public ControlPointBehavior()
        {
            location = neutralLocation;
        }

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
            SimObject discSimObject = Owner.getOtherSimObject(discObject);
            if (discSimObject != null)
            {
                disc = discSimObject.getElement(discName) as Disc;
                if (disc == null)
                {
                    blacklist("Could not find Disc {0} in SimObject {1}.", discName, discObject);
                }
            }
            else
            {
                blacklist("Could not find Disc SimObject {0}.", discObject);
            }
            joint = Owner.getElement(jointName) as Generic6DofConstraintElement;
            if (joint == null)
            {
                blacklist("Could not find joint {0}.", jointName);
            }

            ControlPointController.addControlPoint(this);
        }

        protected override void destroy()
        {
            ControlPointController.removeControlPoint(this);
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            Vector3 bonePos = bone.getDerivedPosition();
            if (bonePos != lastPosition)
            {
                Vector3 position = Quaternion.quatRotate(boneObject.Rotation, bonePos) + boneObject.Translation;
                joint.setFrameOffsetB(bone.getDerivedPosition());
                lastPosition = bonePos;
            }
            if (translate)
            {
                float newLocation = location;
                if (location < targetLocation)
                {
                    newLocation += moveSpeed * (float)clock.Seconds;
                    if (location > targetLocation)
                    {
                        location = targetLocation;
                        translate = false;
                    }
                }
                else
                {
                    newLocation -= moveSpeed * (float)clock.Seconds;
                    if (location <= targetLocation)
                    {
                        location = targetLocation;
                        translate = false;
                    }
                }
                setLocation(newLocation);
            }
        }

        public void moveToLocation(float location, float speed)
        {
            targetLocation = location;
            moveSpeed = speed;
            translate = true;
        }

        public void stopMovement()
        {
            translate = false;
        }

        public void setLocation(float location)
        {
            this.location = location;
            positionModified();
        }

        public void positionModified()
        {
            Vector3 newLocation = disc.getPosition(location);
            this.updateTranslation(ref newLocation);
        }

        public float getNeutralLocation()
        {
            return neutralLocation;
        }

        public Vector3 MandibleBonePosition
        {
            get
            {
                return bone.getDerivedPosition();
            }
        }

        public Quaternion MandibleBoneRotation
        {
            get
            {
                return bone.getDerivedOrientation() * boneObject.Rotation;
            }
        }

        public Quaternion MandibleRotation
        {
            get
            {
                return boneObject.Rotation;
            }
        }

        public Vector3 MandibleTranslation
        {
            get
            {
                return boneObject.Translation;
            }
        }

        public float CurrentLocation
        {
            get
            {
                return location;
            }
        }
    }
}
