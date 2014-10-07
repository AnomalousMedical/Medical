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
    public class OffsetModifierKeyframe : Saveable
    {
        private Vector3 translationOffset;
        private Quaternion rotationOffset;
        private EditInterface editInterface;

        public OffsetModifierKeyframe()
        {

        }

        [Editable]
        public Vector3 TranslationOffset
        {
            get
            {
                return translationOffset;
            }
            set
            {
                translationOffset = value;
                fireDataNeedsRefresh();
            }
        }

        [Editable]
        public Quaternion RotationOffset
        {
            get
            {
                return rotationOffset;
            }
            set
            {
                rotationOffset = value;
                fireDataNeedsRefresh();
            }
        }

        public float BlendAmount { get; set; }

        public void blendFrom(OffsetModifierKeyframe previousFrame, float percentage, SimObjectFollowerWithRotation follower)
        {
            follower.TranslationOffset = previousFrame.translationOffset.lerp(ref translationOffset, ref percentage);
            follower.RotationOffset = previousFrame.rotationOffset.nlerp(ref rotationOffset, ref percentage);
            follower.computePosition();
        }

        /// <summary>
        /// Apply the current translation and rotation offset to the given keyframe.
        /// </summary>
        /// <param name="keyframe"></param>
        public void deriveOffsetFromFollower(SimObjectFollowerWithRotation follower)
        {
            SimObject child = follower.Owner;
            SimObject parent = follower.TargetSimObject;

            Quaternion inverseParentRot = parent.Rotation.inverse();

            this.TranslationOffset = Quaternion.quatRotate(inverseParentRot, child.Translation - parent.Translation);
            this.RotationOffset = inverseParentRot * child.Rotation;
        }

        protected OffsetModifierKeyframe(LoadInfo info)
        {
            TranslationOffset = info.GetVector3("TranslationOffset");
            RotationOffset = info.GetQuaternion("RotationOffset");
            BlendAmount = info.GetFloat("BlendAmount");
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

        private void fireDataNeedsRefresh()
        {
            if (editInterface != null)
            {
                editInterface.fireDataNeedsRefresh();
            }
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("TranslationOffset", TranslationOffset);
            info.AddValue("RotationOffset", RotationOffset);
            info.AddValue("BlendAmount", BlendAmount);
        }

        public void preview(SimObjectFollowerWithRotation follower)
        {
            blendFrom(this, 0.0f, follower);
        }
    }
}
