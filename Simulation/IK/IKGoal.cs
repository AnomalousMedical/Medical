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

        public Vector3 TargetPosition
        {
            get
            {
                return target.Translation;
            }
        }

        public Vector3 EffectorPosition
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
