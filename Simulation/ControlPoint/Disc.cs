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
using Logging;

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
        
#if DEBUG_KEYS

    enum DiscEvents
    {
        PrintBoneLocations,
    }

        static Disc()
        {
            MessageEvent printDebugInfo = new MessageEvent(DiscEvents.PrintBoneLocations);
            printDebugInfo.addButton(KeyboardButtonCode.KC_L);
            DefaultEvents.registerDefaultEvent(printDebugInfo);
        }
#endif

        private const float DEG_TO_RAD = 0.0174532925f;

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

        //The location that the disc starts to move with the condyle.
        [Editable]
        private float discPopLocation = 0.0f;

        //The offset from the discPopLocation where the condyle starts to go under the disc.
        [Editable]
        private float discBackOffset = 0.14f;

        [Editable]
        private float popAdditionalOffsetPercent = 0.5f;

        [Editable]
        private bool locked = false;

        [Editable]
        private Vector3 nineOClockRotation = new Vector3(0.0f, 0.0f, 110.0f);

        [Editable]
        private Vector3 nineOClockCounterRotation = new Vector3(0.0f, 0.0f, -50.0f);

        [Editable]
        private float nineOClockPosition = .7368f;

        [Editable]
        private float oneOClockPosition = .5132f;

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

        [Editable]
        String fossaObject;

        [Editable]
        String fossaName;

        Vector3 endpointOffset = Vector3.Zero;

        [DoNotCopy]
        [DoNotSave]
        List<DiscBonePair> bones = new List<DiscBonePair>();

        [DoNotCopy]
        [DoNotSave]
        Bone medialPoleBone;

        [DoNotCopy]
        [DoNotSave]
        Bone lateralPoleBone;

        [DoNotCopy]
        [DoNotSave]
        Bone ventralPoleBone;

        [DoNotCopy]
        [DoNotSave]
        Bone posteriorPoleBone;

        [DoNotCopy]
        [DoNotSave]
        Bone posteriorPoleRotator;

        [DoNotCopy]
        [DoNotSave]
        Vector3 medialPoleBoneOffset;

        [DoNotCopy]
        [DoNotSave]
        Vector3 lateralPoleBoneOffset;

        [DoNotCopy]
        [DoNotSave]
        Vector3 ventralPoleBoneOffset;

        [DoNotCopy]
        [DoNotSave]
        Fossa fossa;

        [DoNotCopy]
        [DoNotSave]
        Quaternion nineOClockRotationQuat;

        [DoNotCopy]
        [DoNotSave]
        Quaternion nineOClockCounterRotationQuat;

        [DoNotCopy]
        [DoNotSave]
        Quaternion startingRot;

        [DoNotCopy]
        [DoNotSave]
        float rotationRange;

        private ControlPointBehavior controlPoint;

        protected override void constructed()
        {
            Vector3 rotRad = nineOClockRotation * DEG_TO_RAD;
            nineOClockRotationQuat.setEuler(rotRad.x, rotRad.y, rotRad.z);
            rotRad = nineOClockCounterRotation * DEG_TO_RAD;
            nineOClockCounterRotationQuat.setEuler(rotRad.x, rotRad.y, rotRad.z);
            startingRot = this.Owner.Rotation;
            rotationRange = nineOClockPosition - oneOClockPosition;

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
                        float delta = 0.013f;
                        for (int i = 1; skeleton.hasBone(boneBaseName + i); ++i)
                        {
                            Bone bone = skeleton.getBone(boneBaseName + i);
                            bone.setManuallyControlled(true);
                            bones.Add(new DiscBonePair(bone, current));
                            current += delta;
                        }
                        //TEMP TEMP TEMP TEMP TEMP REMOVE EMENENCE REPLACE HACK HACK
                        medialPoleBone = skeleton.getBone(boneBaseName.Replace("Emenence", "") + "MedialPoleControl2");
                        medialPoleBone.setManuallyControlled(true);
                        lateralPoleBone = skeleton.getBone(boneBaseName.Replace("Emenence", "") + "LateralPoleControl2");
                        lateralPoleBone.setManuallyControlled(true);
                        ventralPoleBone = skeleton.getBone(boneBaseName.Replace("Emenence", "") + "VentralPoleControl");
                        ventralPoleBone.setManuallyControlled(true);
                        posteriorPoleBone = skeleton.getBone(boneBaseName.Replace("Emenence", "") + "PosteriorPole");
                        posteriorPoleBone.setManuallyControlled(true);
                        posteriorPoleRotator = skeleton.getBone(boneBaseName.Replace("Emenence", "") + "PosteriorPoleRotator");
                        posteriorPoleRotator.setManuallyControlled(true);
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

            SimObject fossaSimObject = Owner.getOtherSimObject(fossaObject);
            if (fossaSimObject != null)
            {
                fossa = fossaSimObject.getElement(fossaName) as Fossa;
                if (fossa == null)
                {
                    blacklist("Could not find Fossa {0} in SimObject {1}.", fossaName, fossaObject);
                }
            }
            else
            {
                blacklist("Could not find Fossa SimObject {0}.", fossaObject);
            }
        }

        protected override void link()
        {
            Vector3 endpointBoneWorld = controlPoint.MandibleBonePosition + controlPoint.MandibleTranslation;
            endpointOffset = this.Owner.Translation - endpointBoneWorld;
            medialPoleBoneOffset = medialPoleBone.getDerivedPosition() + Owner.Translation - endpointBoneWorld;
            lateralPoleBoneOffset = lateralPoleBone.getDerivedPosition() + Owner.Translation - endpointBoneWorld;
            ventralPoleBoneOffset = ventralPoleBone.getDerivedPosition() + Owner.Translation - endpointBoneWorld;
        }

        protected override void destroy()
        {
            DiscController.removeDisc(this);
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            //pole updates
            Vector3 medialTranslation = Quaternion.quatRotate(controlPoint.MandibleRotation, controlPoint.MandibleBonePosition + medialPoleBoneOffset) + controlPoint.MandibleTranslation - Owner.Translation;
            medialPoleBone.setPosition(Quaternion.quatRotate(Owner.Rotation.inverse(), medialTranslation));
            medialPoleBone.needUpdate(true);
            Vector3 lateralTranslation = Quaternion.quatRotate(controlPoint.MandibleRotation, controlPoint.MandibleBonePosition + lateralPoleBoneOffset) + controlPoint.MandibleTranslation - Owner.Translation;
            lateralPoleBone.setPosition(Quaternion.quatRotate(Owner.Rotation.inverse(), lateralTranslation));
            lateralPoleBone.needUpdate(true);
            Vector3 ventralTranslation = Quaternion.quatRotate(controlPoint.MandibleRotation, controlPoint.MandibleBonePosition + ventralPoleBoneOffset) + controlPoint.MandibleTranslation - Owner.Translation;
            ventralPoleBone.setPosition(Quaternion.quatRotate(Owner.Rotation.inverse(), ventralTranslation));
            ventralPoleBone.needUpdate(true);

            float location = controlPoint.CurrentLocation;
            if (controlPoint.CurrentLocation >= discPopLocation && !locked)
            {
                Vector3 translation = Quaternion.quatRotate(controlPoint.MandibleRotation, controlPoint.MandibleBonePosition + endpointOffset) + controlPoint.MandibleTranslation;
                updateTranslation(ref translation);
            }
            else
            {
                location = discPopLocation;
                Vector3 translation = Quaternion.quatRotate(controlPoint.MandibleRotation, controlPoint.MandibleBonePosition + endpointOffset) + controlPoint.MandibleTranslation;
                updateTranslation(ref translation);

                Quaternion posteriorPopRotation = Quaternion.Identity;
                Quaternion counterPopRotation = Quaternion.Identity;
                if (location < nineOClockPosition && location > oneOClockPosition)
                {
                    float rotBlend = (location - oneOClockPosition) / rotationRange;
                    posteriorPopRotation = startingRot.slerp(ref nineOClockRotationQuat, rotBlend);
                    counterPopRotation = startingRot.slerp(ref nineOClockCounterRotationQuat, rotBlend);
                }
                else
                {
                    if (location >= nineOClockPosition)
                    {
                        posteriorPopRotation = nineOClockRotationQuat;
                        counterPopRotation = nineOClockCounterRotationQuat;
                    }
                    if (location <= oneOClockPosition)
                    {
                        posteriorPopRotation = startingRot;
                        counterPopRotation = startingRot;
                    }
                }
                if (controlPoint.CurrentLocation < discPopLocation - discBackOffset)
                {
                    posteriorPoleRotator.setOrientation(posteriorPopRotation);
                    posteriorPoleRotator.needUpdate(true);
                    posteriorPoleBone.setOrientation(counterPopRotation);
                    posteriorPoleBone.needUpdate(true);
                }
                else
                {
                    float rotBlend = (controlPoint.CurrentLocation - discPopLocation + discBackOffset) / discBackOffset;
                    Quaternion posteriorSlipRotation = posteriorPopRotation.slerp(ref startingRot, rotBlend);
                    posteriorPoleRotator.setOrientation(posteriorSlipRotation);
                    posteriorPoleRotator.needUpdate(true);
                    Quaternion counterSlipRotation = counterPopRotation.slerp(ref startingRot, rotBlend);
                    posteriorPoleBone.setOrientation(counterSlipRotation);
                    posteriorPoleBone.needUpdate(true);
                }
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
                bone.bone.setPosition(Quaternion.quatRotate(Owner.Rotation.inverse(), fossa.getPosition(loc) - Owner.Translation));
                bone.bone.needUpdate(true);
            }

            
            posteriorPoleBone.needUpdate(true);

#if DEBUG_KEYS
            if (eventManager[DiscEvents.PrintBoneLocations].FirstFrameDown)
            {
                Log.Default.debug("\nBone position -- {0}", boneBaseName);
                SceneNodeElement node = Owner.getElement(sceneNodeName) as SceneNodeElement;
                if (node != null)
                {
                    Entity entity = node.getNodeObject(entityName) as Entity;
                    if (entity != null)
                    {
                        if (entity.hasSkeleton())
                        {
                            SkeletonInstance skeleton = entity.getSkeleton();
                            for(ushort i = 0; i < skeleton.getNumBones(); ++i)
                            {
                                Bone bone = skeleton.getBone(i);
                                Vector3 loc = Quaternion.quatRotate(Owner.Rotation, bone.getDerivedPosition()) + Owner.Translation;
                                Log.Default.debug("Bone {0} - {1}, {2}, {3}", bone.getName(), loc.x, -loc.z, loc.y);
                            }    
                        }
                    }
                }
                Log.Default.debug("End Bone position -- {0}\n", boneBaseName);
            }
#endif
        }

        private Vector3 getOffset(float location)
        {
            if (location < discPopLocation - discBackOffset)
            {
                return rdaOffset + horizontalOffset;
            }
            else if (location < discPopLocation)
            {
                return discOffset + discOffset * popAdditionalOffsetPercent + horizontalOffset;
            }
            else
            {
                return discOffset + horizontalOffset;
            }
        }

        public Vector3 getPosition(float location)
        {
            return fossa.getPosition(location) + getOffset(location);
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

        public bool Locked
        {
            get
            {
                return locked;
            }
            set
            {
                locked = value;
            }
        }
    }
}
