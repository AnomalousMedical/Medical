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
using System.Diagnostics;

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

        [Editable]
        private Vector3 normalDiscOffset = Vector3.UnitY * -0.302f;

        [Editable]
        private Vector3 discOffset = Vector3.UnitY * -0.302f;

        [Editable]
        private Vector3 normalRDAOffset = Vector3.UnitY * -0.151f;

        [Editable]
        private Vector3 rdaOffset = Vector3.UnitY * -0.151f;

        [Editable]
        private Vector3 horizontalOffset = Vector3.Zero;

        [Editable]
        private Vector3 clockFaceOffset = Vector3.Zero;

        [Editable]
        private Vector3 normalClockFaceOffset = Vector3.Zero;

        //The location that the disc starts to move with the condyle.
        [Editable]
        private float discPopLocation = 0.5132f;

        //The offset from the discPopLocation where the condyle starts to go under the disc.
        [Editable]
        private float discBackOffset = 0.14f;

        [Editable]
        private float lateralPoleRotation = 0.0f;

        [Editable]
        private bool displaceLateralPole = false;

        [Editable]
        private float lateralPolePopDistance = 0.2f;

        [Editable]
        private float lateralPoleDiscBack = 0.03f;

        [Editable]
        private bool locked = false;

        [Editable]
        String controlPointObject;

        [Editable]
        String controlPointBehavior;

        [Editable]
        String sceneNodeName = "Node";

        [Editable]
        String entityName = "Entity";

        [Editable]
        String fossaObject;

        [Editable]
        String fossaName;

        [Editable]
        DiscLockedPoleControl medialPole = new DiscLockedPoleControl();

        [Editable]
        DiscLockedPoleControl lateralPole = new DiscLockedPoleControl();

        [Editable]
        DiscLockedPoleControl ventralPole = new DiscLockedPoleControl();

        [Editable]
        DiscPosteriorPoleControl posteriorPole = new DiscPosteriorPoleControl();

        [Editable]
        DiscTopSurface topSurface = new DiscTopSurface();

        [DoNotCopy]
        [DoNotSave]
        Vector3 endpointOffset = Vector3.Zero;

        [DoNotCopy]
        [DoNotSave]
        Fossa fossa;

        [DoNotCopy]
        [DoNotSave]
        private ControlPointBehavior controlPoint;

        [DoNotCopy]
        [DoNotSave]
        private PoseManipulator discRotation;

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

            SceneNodeElement node = Owner.getElement(sceneNodeName) as SceneNodeElement;
            if (node != null)
            {
                Entity entity = node.getNodeObject(entityName) as Entity;
                if (entity != null)
                {
                    if (entity.hasSkeleton())
                    {
                        SkeletonInstance skeleton = entity.getSkeleton();
                        medialPole.findBone(skeleton);
                        lateralPole.findBone(skeleton);
                        ventralPole.findBone(skeleton);
                        posteriorPole.initialize(skeleton, Owner, controlPoint, this);
                        topSurface.initialize(skeleton, fossa, Owner);
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
            Vector3 endpointBoneWorld = controlPoint.MandibleBonePosition + controlPoint.MandibleTranslation;
            endpointOffset = this.Owner.Translation - endpointBoneWorld;
            medialPole.initialize(controlPoint, Owner);
            lateralPole.initialize(controlPoint, Owner);
            ventralPole.initialize(controlPoint, Owner);

            discRotation = Owner.getElement("DiscRotator") as PoseManipulator;
            if (discRotation != null)
            {
                discRotation.Position = lateralPoleRotation;
            }
        }

        protected override void destroy()
        {
            DiscController.removeDisc(this);
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            //pole updates
            medialPole.update();
            lateralPole.update();
            ventralPole.update();

            float location = controlPoint.CurrentLocation;

            //Calculate the lateral pole displacement.
            if (displaceLateralPole)
            {
                float popOffset = 0.0f;
                if (locked)
                {
                    popOffset = discPopLocation - NormalPopLocation;
                }
                else if (location < discPopLocation)
                {
                    popOffset = discPopLocation - location;
                }
                if (popOffset < 0.0f)
                {
                    popOffset = 0.0f;
                }
                float rotatePercentage = popOffset / lateralPolePopDistance;
                if (rotatePercentage > 1.0f)
                {
                    rotatePercentage = 1.0f;
                }
                LateralPoleRotation = rotatePercentage;
            }
            //Move the disc with the mandible as it is under the pop location.
            if (controlPoint.CurrentLocation >= discPopLocation && !locked)
            {
                Vector3 translation = Quaternion.quatRotate(controlPoint.MandibleRotation, controlPoint.MandibleBonePosition + endpointOffset) + controlPoint.MandibleTranslation;
                updateTranslation(ref translation);
            }
            //The disc is displaced from the top of the mandible.
            else
            {
                if (displaceLateralPole)
                {
                    if (controlPoint.CurrentLocation >= discPopLocation && locked)
                    {
                        location = controlPoint.CurrentLocation;
                    }
                    else
                    {
                        location = discPopLocation;
                    }
                }
                else
                {
                    if (controlPoint.CurrentLocation >= discPopLocation - discBackOffset && locked)
                    {
                        location = controlPoint.CurrentLocation + discBackOffset;
                    }
                    else
                    {
                        location = discPopLocation;
                    }
                }
                Vector3 translation = Quaternion.quatRotate(controlPoint.MandibleRotation, controlPoint.MandibleBonePosition + endpointOffset) + controlPoint.MandibleTranslation;
                updateTranslation(ref translation);

                posteriorPole.update(location);
            }

            topSurface.update(location);

#if DEBUG_KEYS
            processDebug(eventManager);
#endif
        }

        private Vector3 getOffset(float location)
        {
            if (locked)
            {
                return rdaOffset + horizontalOffset;
            }
            else if(displaceLateralPole)
            {
                if (location < discPopLocation - lateralPoleDiscBack)
                {
                    return rdaOffset + horizontalOffset;
                }
                else
                {
                    return discOffset + horizontalOffset; 
                }
            }
            else
            {
                if (location < discPopLocation - discBackOffset)
                {
                    return rdaOffset + clockFaceOffset + horizontalOffset;
                }
                else if (location < discPopLocation)
                {
                    return discOffset + clockFaceOffset + horizontalOffset;
                }
                else
                {
                    return discOffset + horizontalOffset;
                }
            }
        }

        public Vector3 getPosition(float location)
        {
            return fossa.getPosition(location) + getOffset(location);
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

        public float NormalPopLocation
        {
            get
            {
                return posteriorPole.OneOClockPosition;
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

        public Vector3 ClockFaceOffset
        {
            get
            {
                return clockFaceOffset;
            }
            set
            {
                clockFaceOffset = value;
                if (controlPoint != null)
                {
                    controlPoint.positionModified();
                }
            }
        }

        public Vector3 NormalClockFaceOffset
        {
            get
            {
                return normalClockFaceOffset;
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

        public bool DisplaceLateralPole
        {
            get
            {
                return displaceLateralPole;
            }
            set
            {
                displaceLateralPole = value;
                if (!value)
                {
                    LateralPoleRotation = 0.0f;
                }
            }
        }

        internal float DiscBackOffset
        {
            get
            {
                return discBackOffset;
            }
        }

        [DoNotCopy]
        private float LateralPoleRotation
        {
            get
            {
                return lateralPoleRotation;
            }
            set
            {
                Debug.Assert(lateralPoleRotation >= 0.0f && lateralPoleRotation <= 1.0f, "Lateral pole rotation must be between 0 and 1.");
                lateralPoleRotation = value;
                if (discRotation != null)
                {
                    discRotation.Position = lateralPoleRotation;
                }
            }
        }

#if DEBUG_KEYS
        private void processDebug(EventManager eventManager)
        {
            if (eventManager[DiscEvents.PrintBoneLocations].FirstFrameDown)
            {
                Log.Default.debug("\nCP position -- {0} {1}", Owner.Name, controlPoint.Owner.Translation);
                Log.Default.debug("\nBone position -- {0}", Owner.Name);
                SceneNodeElement node = Owner.getElement(sceneNodeName) as SceneNodeElement;
                if (node != null)
                {
                    Entity entity = node.getNodeObject(entityName) as Entity;
                    if (entity != null)
                    {
                        if (entity.hasSkeleton())
                        {
                            SkeletonInstance skeleton = entity.getSkeleton();
                            for (ushort i = 0; i < skeleton.getNumBones(); ++i)
                            {
                                Bone bone = skeleton.getBone(i);
                                Vector3 loc = Quaternion.quatRotate(Owner.Rotation, bone.getDerivedPosition()) + Owner.Translation;
                                Vector3 rot = bone.getOrientation().getEuler() * 57.2957795f;
                                Log.Default.debug("Bone \"{0}\"{1},{2},{3},{4},{5},{6}", bone.getName(), loc.x, loc.y, loc.z, rot.x, rot.y, rot.z);
                                //Log.Default.debug("Bone \"{0}\"{1},{2},{3},{4},{5},{6}", bone.getName(), loc.x, -loc.z, loc.y, rot.x * -1.0f, rot.y, rot.z * -1.0f);
                            }
                        }
                    }
                }
                Log.Default.debug("End Bone position -- {0}\n", Owner.Name);
            }
        }
#endif
    }
}
