using Engine;
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
        public IKGoal()
        {
            Weight = 1.0f;
        }

        public Vector3 Target { get; set; }

        public Vector3 Effector { get; set; }

        [Editable]
        public float Weight { get; set; }
    }
}
