using Engine;
using Engine.ObjectManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class FollowerMovableObject : MovableObject
    {
        private SimObjectFollowerWithRotation follower;

        public FollowerMovableObject(SimObjectFollowerWithRotation follower)
        {
            this.follower = follower;
        }

        public Vector3 ToolTranslation
        {
            get
            {
                return follower.Owner.Translation;
            }
        }

        public void move(Vector3 offset)
        {
            follower.move(offset);
        }

        public Engine.Quaternion ToolRotation
        {
            get
            {
                return follower.Owner.Rotation;
            }
        }

        public void rotate(ref Quaternion newRot)
        {
            follower.rotate(newRot);
        }

        public bool ShowTools { get; set; }

        public void alertToolHighlightStatus(bool highlighted)
        {
            
        }
    }
}
