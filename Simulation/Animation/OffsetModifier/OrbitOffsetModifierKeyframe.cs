using Engine;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class OrbitOffsetModifierKeyframe : OffsetModifierKeyframe
    {
        private Vector3 orbitTranslation;
        private Quaternion orbitRotation;
        private float orbitRadius;
        private Quaternion orbiterAdditionalRotation;
        private EditInterface editInterface;
        private Vector3 orbitStartDirection;

        internal OrbitOffsetModifierKeyframe()
        {
            orbitStartDirection = Vector3.Forward;
            orbitRotation = Quaternion.Identity;
            orbiterAdditionalRotation = Quaternion.Identity;
        }

        [Editable]
        public Vector3 OrbitTranslation
        {
            get
            {
                return orbitTranslation;
            }
            set
            {
                orbitTranslation = value;
                fireDataNeedsRefresh();
            }
        }

        [Editable]
        public Quaternion OrbitRotation
        {
            get
            {
                return orbitRotation;
            }
            set
            {
                orbitRotation = value;
                fireDataNeedsRefresh();
            }
        }

        [Editable]
        public float OrbitRadius
        {
            get
            {
                return orbitRadius;
            }
            set
            {
                orbitRadius = value;
                fireDataNeedsRefresh();
            }
        }

        [Editable]
        public Quaternion OrbiterAdditionalRotation
        {
            get
            {
                return orbiterAdditionalRotation;
            }
            set
            {
                orbiterAdditionalRotation = value;
                fireDataNeedsRefresh();
            }
        }

        [Editable]
        public Vector3 OrbitStartDirection
        {
            get
            {
                return orbitStartDirection;
            }
            set
            {
                orbitStartDirection = value;
                fireDataNeedsRefresh();
            }
        }

        public float BlendAmount { get; set; }

        public void blendFrom(OffsetModifierKeyframe previousFrame, float percentage, SimObjectFollowerWithRotation follower)
        {
            var orbitPreviousFrame = previousFrame as OrbitOffsetModifierKeyframe;
            if (orbitPreviousFrame != null)
            {
                Vector3 orbitTranslationOffset = orbitPreviousFrame.orbitTranslation.lerp(ref orbitTranslation, ref percentage);
                Quaternion orbitRotationOffset = orbitPreviousFrame.orbitRotation.nlerp(ref orbitRotation, ref percentage);
                float radiusOffset = orbitPreviousFrame.orbitRadius.interpolate(orbitRadius, percentage);
                Quaternion orbiterAdditionalRotationOffset = orbitPreviousFrame.orbiterAdditionalRotation.nlerp(ref orbiterAdditionalRotation, ref percentage);

                follower.TranslationOffset = orbitTranslationOffset + Quaternion.quatRotate(orbitRotationOffset, orbitStartDirection * radiusOffset);
                follower.RotationOffset = orbitRotationOffset * orbiterAdditionalRotationOffset;

                follower.computePosition();
            }
        }

        /// <summary>
        /// Apply the current translation and rotation offset to the given keyframe.
        /// </summary>
        /// <param name="keyframe"></param>
        public void deriveOffsetFromFollower(SimObjectFollowerWithRotation follower)
        {
            //SimObject child = follower.Owner;
            //SimObject parent = follower.TargetSimObject;

            //Quaternion inverseParentRot = parent.Rotation.inverse();

            //this.TranslationOffset = Quaternion.quatRotate(inverseParentRot, child.Translation - parent.Translation);
            //this.RotationOffset = inverseParentRot * child.Rotation;
        }

        public void preview(SimObjectFollowerWithRotation follower)
        {
            blendFrom(this, 0.0f, follower);
        }

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, "Properties");
                }
                return editInterface;
            }
        }

        public IEnumerable<OffsetModifierMovableSection> MovableSections
        {
            get
            {
                yield return new SocketMovableSection(this);
                yield return new SocketSecondaryMovableSection(this);
            }
        }

        private void fireDataNeedsRefresh()
        {
            if (editInterface != null)
            {
                editInterface.fireDataNeedsRefresh();
            }
        }

        protected OrbitOffsetModifierKeyframe(LoadInfo info)
        {
            orbitTranslation = info.GetVector3("orbitTranslation");
            orbitRotation = info.GetQuaternion("orbitRotation");
            orbitRadius = info.GetFloat("orbitRadius");
            orbiterAdditionalRotation = info.GetQuaternion("orbiterAdditionalRotation");
            orbitStartDirection = info.GetVector3("orbitStartDirection");
            BlendAmount = info.GetFloat("BlendAmount");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("orbitTranslation", orbitTranslation);
            info.AddValue("orbitRotation", orbitRotation);
            info.AddValue("orbitRadius", orbitRadius);
            info.AddValue("orbiterAdditionalRotation", orbiterAdditionalRotation);
            info.AddValue("orbitStartDirection", orbitStartDirection);
            info.AddValue("BlendAmount", BlendAmount);
        }

        class SocketMovableSection : OffsetModifierMovableSection
        {
            private OrbitOffsetModifierKeyframe keyframe;

            public SocketMovableSection(OrbitOffsetModifierKeyframe keyframe)
            {
                this.keyframe = keyframe;
            }

            public Vector3 getTranslation(SimObjectFollowerWithRotation follower)
            {
                return follower.TargetSimObject.Translation + Quaternion.quatRotate(follower.TargetSimObject.Rotation, keyframe.OrbitTranslation);
            }

            public void move(Vector3 offset, SimObjectFollowerWithRotation follower)
            {
                keyframe.OrbitTranslation += Quaternion.quatRotate(follower.TargetSimObject.Rotation.inverse(), offset);
            }

            public Quaternion getRotation(SimObjectFollowerWithRotation follower)
            {
                return follower.TargetSimObject.Rotation * keyframe.OrbitRotation;
            }

            public void setRotation(Quaternion rotation, SimObjectFollowerWithRotation follower)
            {
                keyframe.OrbitRotation = follower.TargetSimObject.Rotation.inverse() * rotation;
            }
        }

        class SocketSecondaryMovableSection : OffsetModifierMovableSection
        {
            private OrbitOffsetModifierKeyframe keyframe;

            public SocketSecondaryMovableSection(OrbitOffsetModifierKeyframe keyframe)
            {
                this.keyframe = keyframe;
            }

            public Vector3 getTranslation(SimObjectFollowerWithRotation follower)
            {
                return follower.Owner.Translation;
            }

            public void move(Vector3 offset, SimObjectFollowerWithRotation follower)
            {
                keyframe.OrbitRadius += offset.z;
            }

            public Quaternion getRotation(SimObjectFollowerWithRotation follower)
            {
                return follower.Owner.Rotation;
            }

            public void setRotation(Quaternion rotation, SimObjectFollowerWithRotation follower)
            {
                keyframe.OrbiterAdditionalRotation = follower.TargetSimObject.Rotation.inverse() * keyframe.orbitRotation.inverse() * rotation;
            }
        }
    }
}
