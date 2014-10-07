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
        private OffsetModifierMovableSection movementSection;

        public KeyframeMovableObject(OffsetModifierMovableSection movementSection)
        {
            this.movementSection = movementSection;
        }

        public Vector3 ToolTranslation
        {
            get
            {
                if(Follower != null)
                {
                    return movementSection.getTranslation(Follower);
                }
                return Vector3.Zero;
            }
        }

        public void move(Vector3 offset)
        {
            movementSection.move(offset, Follower);
        }

        public Quaternion ToolRotation
        {
            get
            {
                if (Follower != null)
                {
                    return movementSection.getRotation(Follower);
                }
                return Quaternion.Identity;
            }
        }

        public void rotate(Quaternion newRot)
        {
            movementSection.setRotation(newRot, Follower);
        }

        public bool ShowTools
        {
            get
            {
                return Follower != null;
            }
        }

        public void alertToolHighlightStatus(bool highlighted)
        {
            
        }

        public SimObjectFollowerWithRotation Follower { get; set; }
    }
}
