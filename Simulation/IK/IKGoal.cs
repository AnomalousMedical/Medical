using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.IK
{
    class IKGoal
    {
        [DoNotCopy]
        [DoNotSave]
        private SimObject target;

        [DoNotCopy]
        [DoNotSave]
        private IKJoint effector;

        public IKGoal(SimObject target, IKJoint effector)
        {
            Weight = 1.0f;
            this.target = target;
            this.effector = effector;
        }

        public Vector3 Target
        {
            get
            {
                return target.Translation;
            }
        }

        public Vector3 Effector
        {
            get
            {
                return effector.WorldTranslation;
            }
        }

        [Editable]
        public float Weight { get; set; }
    }
}
