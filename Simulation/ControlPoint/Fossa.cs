using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.Editing;
using Engine.Attributes;
using Engine.ObjectManagement;
using OgreWrapper;
using OgrePlugin;
using Logging;

namespace Medical
{
#if DEBUG_KEYS
    public class Fossa : Behavior
    {
        enum FossaEvents
        {
            PrintBoneLocations,
        }

        static Fossa()
        {
            MessageEvent printDebugInfo = new MessageEvent(FossaEvents.PrintBoneLocations);
            printDebugInfo.addButton(KeyboardButtonCode.KC_K);
            DefaultEvents.registerDefaultEvent(printDebugInfo);
        }
#else
    public class Fossa : Interface
    {
#endif

        [Editable]
        private String interactiveCurveName;

        [Editable]
        private String skullName;

        [Editable]
        private String fossaBoneBaseName;

        [Editable]
        private String skullNodeName;

        [Editable]
        private String skullEntityName;

        [Editable]
        private String eminanceName;

        [Editable]
        private String eminanceNodeName;

        [Editable]
        private String eminanceEntityName;

        [Editable]
        private int eminanceStart = 12;

        [DoNotSave]
        [DoNotCopy]
        private List<Vector3> basePoints = new List<Vector3>();
        [DoNotSave]
        [DoNotCopy]
        private InteractiveCurve translation;
        [DoNotSave]
        [DoNotCopy]
        private List<Vector3> eminanceDistort = new List<Vector3>();
        
        private float eminanceDistortion;
        private SimObject skullSimObject;
        [DoNotSave]
        [DoNotCopy]
        private List<Bone> skullBones = new List<Bone>();

        [DoNotSave]
        [DoNotCopy]
        private SimObject eminanceSimObject;

        [DoNotSave]
        [DoNotCopy]
        private List<Bone> eminanceBones = new List<Bone>();

        [Editable]
        String controlPointObject;

        [Editable]
        String controlPointBehavior;

        private ControlPointBehavior controlPoint;

        protected override void constructed()
        {
            FossaController.add(this);
            translation = Owner.getElement(interactiveCurveName) as InteractiveCurve;
            if (translation == null)
            {
                blacklist("Could not find an interactive curve named {0} in SimObject {1}.", interactiveCurveName, Owner.Name);
            }
            else
            {
                eminanceSimObject = Owner.getOtherSimObject(eminanceName);
                if (eminanceSimObject != null)
                {
                    SceneNodeElement eminanceSceneNode = eminanceSimObject.getElement(eminanceNodeName) as SceneNodeElement;
                    if (eminanceSceneNode != null)
                    {
                        Entity eminanceEntity = eminanceSceneNode.getNodeObject(eminanceEntityName) as Entity;
                        if (eminanceEntity != null)
                        {
                            if (eminanceEntity.hasSkeleton())
                            {
                                SkeletonInstance skeleton = eminanceEntity.getSkeleton();
                                for (int i = 1; skeleton.hasBone(fossaBoneBaseName + i); ++i)
                                {
                                    Bone bone = skeleton.getBone(fossaBoneBaseName + i);
                                    eminanceBones.Add(bone);
                                    bone.setManuallyControlled(true);
                                }
                            }
                        }
                        else
                        {
                            blacklist("Cannot find the eminance entity {0}", eminanceEntityName);
                        }
                    }
                    else
                    {
                        blacklist("Cannot find the eminance scene node {0}", eminanceNodeName);
                    }
                }

                skullSimObject = Owner.getOtherSimObject(skullName);
                if (skullSimObject != null)
                {
                    SceneNodeElement skullNode = skullSimObject.getElement(skullNodeName) as SceneNodeElement;
                    if (skullNode != null)
                    {
                        Entity skullEntity = skullNode.getNodeObject(skullEntityName) as Entity;
                        if (skullEntity != null)
                        {
                            if(skullEntity.hasSkeleton())
                            {
                                SkeletonInstance skeleton = skullEntity.getSkeleton();
                                Vector3 eminanceTrans = Vector3.Zero;
                                for (int i = 1; skeleton.hasBone(fossaBoneBaseName + i); ++i)
                                {
                                    Bone bone = skeleton.getBone(fossaBoneBaseName + i);
                                    skullBones.Add(bone);
                                    bone.setManuallyControlled(true);
                                    Vector3 trans = bone.getDerivedPosition() + skullSimObject.Translation - this.Owner.Translation;
                                    trans.x = 0;
                                    basePoints.Add(trans);
                                    if (i < eminanceStart)
                                    {
                                        eminanceDistort.Add(trans);
                                        eminanceTrans = trans;
                                    }
                                    else
                                    {
                                        trans.y = eminanceTrans.y;
                                        eminanceDistort.Add(trans);
                                    }
                                }
                            }
                        }
                        else
                        {
                            blacklist("Could not find skull entity {0}.", skullEntityName);
                        }
                    }
                    else
                    {
                        blacklist("Could not find skull node {0}.", skullNodeName);
                    }
                }
                else
                {
                    blacklist("Could not find a skull SimObject named {0}.", skullName);
                }
                foreach (Vector3 point in basePoints)
                {
                    translation.addControlPoint(point);
                }

                //Find the control point
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
            }
        }

        private void updateBones()
        {
            if (basePoints.Count != 0)
            {
                float boneOffsetDelta = 1.0f / basePoints.Count;
                float currentOffset = 0.0f;
                for (int i = 0; i < skullBones.Count; ++i)
                {
                    skullBones[i].setPosition(translation.interpolate(currentOffset) + this.Owner.Translation - skullSimObject.Translation);
                    skullBones[i].needUpdate(true);
                    eminanceBones[i].setPosition(translation.interpolate(currentOffset) + this.Owner.Translation - eminanceSimObject.Translation);
                    eminanceBones[i].needUpdate(true);
                    currentOffset += boneOffsetDelta;
                }
            }
        }

        protected override void destroy()
        {
            FossaController.remove(this);
        }

        public void setEminanceDistortion(float percent)
        {
            eminanceDistortion = percent;
            for (int i = 0; i < basePoints.Count; ++i)
            {
                Vector3 distort = eminanceDistort[i];
                translation.updateControlPoint(i, basePoints[i].lerp(ref distort, ref percent));
            }
            translation.recomputeCurve();
            updateBones();
            controlPoint.positionModified();
        }

        public float getEminanceDistortion()
        {
            return eminanceDistortion;
        }

        protected override void link()
        {
            if (translation != null)
            {
                translation.recomputeCurve();
                updateBones();
            }
        }

        public Vector3 getPosition(float position)
        {
            return Owner.Translation + translation.interpolate(position);
        }

#if DEBUG_KEYS
        public override void update(Clock clock, EventManager eventManager)
        {
            if (eventManager[FossaEvents.PrintBoneLocations].FirstFrameDown)
            {
                Log.Default.debug("\nFossa position -- {0} {1}", Owner.Name, controlPoint.Owner.Translation);
                Log.Default.debug("\nBone position -- {0}", Owner.Name);
                SceneNodeElement node = eminanceSimObject.getElement(eminanceNodeName) as SceneNodeElement;
                if (node != null)
                {
                    Entity entity = node.getNodeObject(eminanceEntityName) as Entity;
                    if (entity != null)
                    {
                        if (entity.hasSkeleton())
                        {
                            SkeletonInstance skeleton = entity.getSkeleton();
                            for (ushort i = 0; i < skeleton.getNumBones(); ++i)
                            {
                                Bone bone = skeleton.getBone(i);
                                Vector3 loc = Quaternion.quatRotate(Owner.Rotation, bone.getDerivedPosition()) + eminanceSimObject.Translation;
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
/**
if (basePoints.Count == 0)
                {
                    basePoints.Add(new Vector3(Owner.Translation.x, -5.227f, 0.839f) - Owner.Translation);
                    basePoints.Add(new Vector3(Owner.Translation.x, -4.887f, 0.983f) - Owner.Translation);
                    basePoints.Add(new Vector3(Owner.Translation.x, -4.755f, 1.087f) - Owner.Translation);
                    basePoints.Add(new Vector3(Owner.Translation.x, -4.635f, 1.239f) - Owner.Translation);
                    basePoints.Add(new Vector3(Owner.Translation.x, -4.523f, 1.478f) - Owner.Translation);
                    basePoints.Add(new Vector3(Owner.Translation.x, -4.517f, 1.595f) - Owner.Translation);
                    basePoints.Add(new Vector3(Owner.Translation.x, -4.61f, 1.764f) - Owner.Translation);
                    basePoints.Add(new Vector3(Owner.Translation.x, -4.765f, 1.938f) - Owner.Translation);
                    basePoints.Add(new Vector3(Owner.Translation.x, -4.947f, 2.125f) - Owner.Translation);
                    basePoints.Add(new Vector3(Owner.Translation.x, -5.085f, 2.381f) - Owner.Translation);
                    basePoints.Add(new Vector3(Owner.Translation.x, -5.127f, 2.542f) - Owner.Translation);
                    basePoints.Add(new Vector3(Owner.Translation.x, -5.028f, 2.989f) - Owner.Translation);
                    basePoints.Add(new Vector3(Owner.Translation.x, -4.817f, 3.341f) - Owner.Translation);
                }
                if (eminanceDistort.Count == 0)
                {
                    eminanceDistort.Add(new Vector3(Owner.Translation.x, -5.227f, 0.839f) - Owner.Translation);
                    eminanceDistort.Add(new Vector3(Owner.Translation.x, -4.887f, 0.983f) - Owner.Translation);
                    eminanceDistort.Add(new Vector3(Owner.Translation.x, -4.755f, 1.087f) - Owner.Translation);
                    eminanceDistort.Add(new Vector3(Owner.Translation.x, -4.635f, 1.239f) - Owner.Translation);
                    eminanceDistort.Add(new Vector3(Owner.Translation.x, -4.523f, 1.478f) - Owner.Translation);
                    eminanceDistort.Add(new Vector3(Owner.Translation.x, -4.517f, 1.595f) - Owner.Translation);
                    eminanceDistort.Add(new Vector3(Owner.Translation.x, -4.61f, 1.764f) - Owner.Translation);
                    eminanceDistort.Add(new Vector3(Owner.Translation.x, -4.765f, 1.938f) - Owner.Translation);
                    eminanceDistort.Add(new Vector3(Owner.Translation.x, -4.765f, 2.125f) - Owner.Translation);
                    eminanceDistort.Add(new Vector3(Owner.Translation.x, -4.765f, 2.381f) - Owner.Translation);
                    eminanceDistort.Add(new Vector3(Owner.Translation.x, -4.765f, 2.542f) - Owner.Translation);
                    eminanceDistort.Add(new Vector3(Owner.Translation.x, -4.765f, 2.989f) - Owner.Translation);
                    eminanceDistort.Add(new Vector3(Owner.Translation.x, -4.765f, 3.341f) - Owner.Translation);
                }
*/