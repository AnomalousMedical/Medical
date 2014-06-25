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
        
        [DoNotSave]
        [DoNotCopy]
        private List<Bone> skullBones = new List<Bone>();

        //The fossa stays in the teeth bullet scene space, therefore the eminanceOffset and skullOffset are calculated on construction only.

        [DoNotSave]
        [DoNotCopy]
        private Vector3 eminanceOffset;

        [DoNotSave]
        [DoNotCopy]
        private Vector3 skullOffset;

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
                SimObject eminanceSimObject = Owner.getOtherSimObject(eminanceName);
                if (eminanceSimObject != null)
                {
                    eminanceOffset = eminanceSimObject.Translation;
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

                SimObject skullSimObject = Owner.getOtherSimObject(skullName);
                if (skullSimObject != null)
                {
                    skullOffset = skullSimObject.Translation;
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
                    skullBones[i].setPosition(translation.interpolate(currentOffset) + this.Owner.Translation - skullOffset);
                    skullBones[i].needUpdate(true);
                    eminanceBones[i].setPosition(translation.interpolate(currentOffset) + this.Owner.Translation - eminanceOffset);
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
    }
}