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

namespace Medical
{
    class DiscBonePair
    {
        public Bone bone;
        public float offset;

        public DiscBonePair(Bone bone, float offset)
        {
            this.bone = bone;
            this.offset = offset;
        }
    }

    public class Disc : Behavior
    {
        [Editable]
        private Vector3 normalDiscOffset = Vector3.UnitY * -0.302f;

        [Editable]
        private Vector3 discOffset = Vector3.UnitY * -0.302f;

        [Editable]
        private Vector3 normalRDAOffset = Vector3.UnitY * -0.151f;

        [Editable]
        private Vector3 rdaOffset = Vector3.UnitY * -0.151f;

        [Editable]
        private Vector3 horizontalTickSpacing = Vector3.UnitX * 0.1f;

        [Editable]
        private Vector3 horizontalOffset = Vector3.Zero;

        [Editable]
        private float discPopLocation = 0.0f;

        [Editable]
        String controlPointObject;

        [Editable]
        String controlPointBehavior;

        [Editable]
        String sceneNodeName = "Node";

        [Editable]
        String entityName = "Entity";

        [Editable]
        String boneBaseName;

        Vector3 endpointOffset = Vector3.Zero;

        [DoNotCopy]
        [DoNotSave]
        List<DiscBonePair> bones = new List<DiscBonePair>();

        private ControlPointBehavior controlPoint;

        protected override void constructed()
        {
            DiscController.addDisc(this);
            SimObject controlPointObj = Owner.getOtherSimObject(controlPointObject);
            if (controlPointObj != null)
            {
                controlPoint = controlPointObj.getElement(controlPointBehavior) as ControlPointBehavior;
                if (controlPoint == null)
                {
                    blacklist("Could not find controlPointBehavior {0}.", controlPointBehavior);
                }
            }
            else
            {
                blacklist("Could not find controlPointObject {0}.", controlPointObject);
            }
            SceneNodeElement node = Owner.getElement(sceneNodeName) as SceneNodeElement;
            if (node != null)
            {
                Entity entity = node.getNodeObject(entityName) as Entity;
                if (entity != null)
                {
                    if (entity.hasSkeleton())
                    {
                        SkeletonInstance skeleton = entity.getSkeleton();
                        float current = -0.1f;
                        float delta = 0.025f;
                        for (int i = 1; skeleton.hasBone(boneBaseName + i); ++i)
                        {
                            Bone bone = skeleton.getBone(boneBaseName + i);
                            bone.setManuallyControlled(true);
                            bones.Add(new DiscBonePair(bone, current));
                            current += delta;
                        }
                    }
                }
                else
                {
                    blacklist("Could not find entity {0} in node {1}.", entityName, sceneNodeName);
                }
            }
            else
            {
                blacklist("Could not find SceneNode {0}.", sceneNodeName);
            }
        }

        protected override void link()
        {
            endpointOffset = this.Owner.Translation - (controlPoint.MandibleBonePosition + controlPoint.MandibleTranslation);
        }

        protected override void destroy()
        {
            DiscController.removeDisc(this);
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            float location = controlPoint.CurrentLocation;
            if (controlPoint.CurrentLocation >= discPopLocation)
            {
                Vector3 translation = Quaternion.quatRotate(controlPoint.MandibleRotation, controlPoint.MandibleBonePosition + endpointOffset) + controlPoint.MandibleTranslation;
                Quaternion rotation = controlPoint.MandibleBoneRotation * controlPoint.MandibleRotation;
                updatePosition(ref translation, ref rotation);
            }
            else
            {
                location = discPopLocation;
                Vector3 offset = controlPoint.getPositionAt(discPopLocation) + endpointOffset;
                Quaternion rotation = Quaternion.Identity;
                updatePosition(ref offset, ref rotation);
            }
            foreach (DiscBonePair bone in bones)
            {
                float loc = location + bone.offset;
                if (loc < 0.0f)
                {
                    loc = 0.0f;
                }
                else if (loc > 1.0f)
                {
                    loc = 1.0f;
                }
                bone.bone.setPosition(controlPoint.getPositionAt(loc) - getOffset(loc) - Owner.Translation);
                bone.bone.needUpdate(true);
            }
        }

        public Vector3 getOffset(float location)
        {
            if (location < discPopLocation)
            {
                return rdaOffset + horizontalOffset;
            }
            else
            {
                return discOffset + horizontalOffset;
            }
        }

        public Vector3 HorizontalTickSpacing
        {
            get
            {
                return horizontalTickSpacing;
            }
        }

        public Vector3 DiscOffset
        {
            get
            {
                return discOffset;
            }
            set
            {
                discOffset = value;
                if (controlPoint != null)
                {
                    controlPoint.positionModified();
                }
            }
        }

        public Vector3 NormalDiscOffset
        {
            get
            {
                return normalDiscOffset;
            }
        }

        public Vector3 RDAOffset
        {
            get
            {
                return rdaOffset;
            }
            set
            {
                rdaOffset = value;
                if (controlPoint != null)
                {
                    controlPoint.positionModified();
                }
            }
        }

        public Vector3 NormalRDAOffset
        {
            get
            {
                return normalRDAOffset;
            }
        }

        public float PopLocation
        {
            get
            {
                return discPopLocation;
            }
            set
            {
                discPopLocation = value;
                if (controlPoint != null)
                {
                    controlPoint.positionModified();
                }
            }
        }

        public Vector3 HorizontalOffset
        {
            get
            {
                return horizontalOffset;
            }
            set
            {
                horizontalOffset = value;
                if (controlPoint != null)
                {
                    controlPoint.positionModified();
                }
            }
        }
    }
}
