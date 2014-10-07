using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class KeyframeMovableObject : MovableObject
    {
        public Vector3 ToolTranslation
        {
            get
            {
                if(Follower != null)
                {
                    return Follower.Owner.Translation;
                }
                return Vector3.Zero;
            }
        }

        public void move(Vector3 offset)
        {
            Keyframe.TranslationOffset += Quaternion.quatRotate(Follower.TargetSimObject.Rotation.inverse(), offset);
        }

        public Quaternion ToolRotation
        {
            get
            {
                if (Follower != null)
                {
                    return Follower.Owner.Rotation;
                }
                return Quaternion.Identity;
            }
        }

        public bool ShowTools
        {
            get
            {
                return Follower != null && Keyframe != null;
            }
        }

        public void rotate(ref Quaternion newRot)
        {
            Keyframe.RotationOffset = Follower.TargetSimObject.Rotation.inverse() * newRot;
        }

        public void alertToolHighlightStatus(bool highlighted)
        {
            
        }

        public SimObjectFollowerWithRotation Follower { get; set; }

        public OffsetModifierKeyframe Keyframe { get; set; }
    }
}
