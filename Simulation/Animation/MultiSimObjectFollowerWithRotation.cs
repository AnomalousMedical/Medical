using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Platform;
using Engine.Attributes;
using Engine.Behaviors.Animation;
using Engine.Saving;

namespace Medical
{
    /// <summary>
    /// This class uses lerp and slerp to blend between multiple sim object world positions to move an individual object.
    /// </summary>
    public class MultiSimObjectFollowerWithRotation : BehaviorScheduledUpdate
    {
        [DoNotSave]
        private List<Entry> weightedEntries = new List<Entry>();

        protected override void link()
        {
            if(weightedEntries.Count == 0)
            {
                blacklist("No weighted entries");
            }

            float totalWeight = 0.0f;
            foreach(var entry in weightedEntries)
            {
                entry.link(this);
                totalWeight += entry.Weight;
                entry.PreviousTotalWeights = totalWeight;
            }
        }

        protected override void destroy()
        {
            foreach (var entry in weightedEntries)
            {
                entry.destroy();
            }
            base.destroy();
        }

        private void broadcasterMoved()
        {
            schedule();
        }

        protected override void scheduledUpdate(Clock clock, EventManager eventManager)
        {
            Vector3 trans = weightedEntries[0].WorldTranslation;
            Quaternion rotation = weightedEntries[0].WorldRotation;
            var previousEntry = weightedEntries[0];
            float slerpAmount = 0;

            for(int i = 1; i < weightedEntries.Count; ++i)
            {
                var entry = weightedEntries[i];

                if(previousEntry.PreviousTotalWeights > entry.Weight)
                {
                    slerpAmount = 1 - previousEntry.Weight / entry.PreviousTotalWeights;
                }
                else
                {
                    slerpAmount = entry.Weight / entry.PreviousTotalWeights;
                }
                trans = trans.lerp(entry.WorldTranslation, slerpAmount);
                rotation = rotation.slerp(entry.WorldRotation, slerpAmount);

                previousEntry = entry;
            }

            updatePosition(ref trans, ref rotation);
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            base.customizeEditInterface(editInterface);
            editInterface.addSubInterface(new ReflectedListLikeEditInterface<Entry>(weightedEntries, "Weighted Broadcasters", () => new Entry()).EditInterface);
        }

        protected override void customLoad(LoadInfo info)
        {
            base.customLoad(info);
            info.RebuildList("WeightedEntry", weightedEntries);
        }

        protected override void customSave(SaveInfo info)
        {
            base.customSave(info);
            info.ExtractList("WeightedEntry", weightedEntries);
        }

        class Entry : Saveable
        {
            [Editable]
            String targetSimObjectName;

            [Editable]
            String targetPositionBroadcasterName = "PositionBroadcaster";

            [Editable]
            float weight;

            [DoNotCopy]
            [DoNotSave]
            SimObject targetSimObject;

            [DoNotCopy]
            [DoNotSave]
            Vector3 translationOffset;

            [DoNotCopy]
            [DoNotSave]
            Quaternion rotationOffset;

            [DoNotCopy]
            [DoNotSave]
            PositionBroadcaster broadcaster;

            [DoNotCopy]
            [DoNotSave]
            MultiSimObjectFollowerWithRotation follower;

            [DoNotCopy]
            [DoNotSave]
            Vector3 worldTranslation;

            [DoNotCopy]
            [DoNotSave]
            Quaternion worldRotation;

            public Entry()
            {

            }

            public void link(MultiSimObjectFollowerWithRotation follower)
            {
                this.follower = follower;

                targetSimObject = follower.Owner.getOtherSimObject(targetSimObjectName);
                if (targetSimObject == null)
                {
                    throw new Exception(String.Format("Cannot find target SimObject {0}.", targetSimObjectName));
                }

                broadcaster = targetSimObject.getElement(targetPositionBroadcasterName) as PositionBroadcaster;
                if (broadcaster == null)
                {
                    throw new Exception(String.Format("Cannot find target PositionBroadcaster '{0}' on SimObject '{1}'", targetPositionBroadcasterName, targetSimObjectName));
                }
                broadcaster.PositionChanged += broadcaster_PositionChanged;

                Quaternion inverseTargetRot = targetSimObject.Rotation.inverse();

                translationOffset = follower.Owner.Translation - targetSimObject.Translation;
                translationOffset = Quaternion.quatRotate(inverseTargetRot, translationOffset);

                rotationOffset = inverseTargetRot * follower.Owner.Rotation;
            }

            public void destroy()
            {
                broadcaster.PositionChanged -= broadcaster_PositionChanged;
            }

            void broadcaster_PositionChanged(SimObject obj)
            {
                worldTranslation = targetSimObject.Translation + Quaternion.quatRotate(targetSimObject.Rotation, translationOffset);
                worldRotation = targetSimObject.Rotation * rotationOffset;

                follower.broadcasterMoved();
            }

            public float Weight
            {
                get
                {
                    return weight;
                }
            }

            [DoNotCopy]
            public float PreviousTotalWeights { get; set; }

            public Vector3 WorldTranslation
            {
                get
                {
                    return worldTranslation;
                }
            }

            public Quaternion WorldRotation
            {
                get
                {
                    return worldRotation;
                }
            }

            protected Entry(LoadInfo info)
            {
                targetSimObjectName = info.GetString("targetSimObjectName");
                targetPositionBroadcasterName = info.GetString("targetPositionBroadcasterName");
                weight = info.GetSingle("weight");
            }

            public void getInfo(SaveInfo info)
            {
                info.AddValue("targetSimObjectName", targetSimObjectName);
                info.AddValue("targetPositionBroadcasterName", targetPositionBroadcasterName);
                info.AddValue("weight", weight);
            }
        }

        //[DoNotCopy]
        //public Vector3 TranslationOffset
        //{
        //    get
        //    {
        //        return translationOffset;
        //    }
        //    set
        //    {
        //        translationOffset = value;
        //    }
        //}

        //[DoNotCopy]
        //public Quaternion RotationOffset
        //{
        //    get
        //    {
        //        return rotationOffset;
        //    }
        //    set
        //    {
        //        rotationOffset = value;
        //    }
        //}

        ///// <summary>
        ///// The SimObject that is being followed.
        ///// </summary>
        //[DoNotCopy]
        //public SimObject TargetSimObject
        //{
        //    get
        //    {
        //        return targetSimObject;
        //    }
        //}
    }
}
