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
    class IKJoint : Interface
    {
        const float epsilon = 1e-6f;

        private static readonly Matrix3x3 NoParentMatrix = Quaternion.Identity.toRotationMatrix();
        [DoNotCopy]
        [DoNotSave]
        bool[] allowTranslation = new bool[]{ false, false, false };

        [DoNotCopy]
        [DoNotSave]
        bool[] allowRotation = new bool[] { true, true, true };

        [DoNotCopy]
        [DoNotSave]
        private IKJoint parent;

        [Editable]
        private String parentSimObjectName;

        private Vector3 localTranslation;
        private Quaternion localRotation;
        private Vector3 worldTranslation;
        private Quaternion worldRotation;

        protected override void link()
        {
            base.link();
        }

        internal void computeLocalChainOffsets()
        {
            worldTranslation = Owner.Translation;
            worldRotation = Owner.Rotation;

            if (parent != null)
            {
                Quaternion inverseParentRot = parent.worldRotation.inverse();
                localTranslation = worldTranslation - parent.worldTranslation;
                localTranslation = Quaternion.quatRotate(inverseParentRot, localTranslation);
                localRotation = worldRotation * inverseParentRot;
            }
            else
            {
                localTranslation = Owner.Translation;
                localRotation = Owner.Rotation;
            }
        }

        internal bool UpdateLocalR(int i, List<IKGoal> goals)
        {
            Vector3 U = GetAxis(i);
            float numer = 0.0f;
            float denom = 0.0f;

            float oldNorm = 0.0f;
            IKGoal goal;
            int g;
            for (g = 0; g < goals.Count; ++g)
            {
                goal = goals[g];
                Vector3 EmP = goal.EffectorPosition - worldTranslation;
                Vector3 GmP = goal.TargetPosition - worldTranslation;
                Vector3 GmE = goal.TargetPosition - goal.EffectorPosition;

                oldNorm = GmE.length2();
                Vector3 UxEmP = U.cross(ref EmP);
                Vector3 UxUxEmP = U.cross(ref UxEmP);
                numer += goal.Weight * GmP.dot(ref UxEmP);
                denom -= goal.Weight * GmP.dot(ref UxUxEmP);
            }

            if (numer * numer + denom * denom < epsilon)
            {
                // Undefined atan2, no rotation.
                return false;
            }

            // Desired angle to rotate about axis(i).
            float theta = (float)Math.Atan2(numer, denom);

            //Skip euler clamping

            // Test whether step should be taken.
            float newNorm = 0.0f;
            Quaternion rotate = new Quaternion(U, theta);
            for (g = 0; g < goals.Count; ++g)
            {
                goal = goals[g];
                Vector3 EmP = goal.EffectorPosition - worldTranslation;
                Vector3 newE = worldTranslation + Quaternion.quatRotate(rotate, EmP);
                Vector3 GmE = goal.TargetPosition - newE;
                newNorm += GmE.length2();
            }

            if (newNorm >= oldNorm)
            {
                // Rotation does not get effector closer to goal.
                return false;
            }

            // Update the local rotation.
            localRotation = rotate;

            return true;
        }

        private Vector3 GetAxis(int i)
        {
            //Slow, fix later
            if (parent != null)
            {
                return parent.WorldRotation.toRotationMatrix().getColumn(i);
            }
            else
            {
                return NoParentMatrix.getColumn(i);
            }
        }

        internal void updateWorldTransforms()
        {
            //Apply trans and rot
            if (parent == null)
            {
                worldTranslation = localTranslation;
                worldRotation = localRotation;
            }
            else
            {
                worldTranslation = parent.worldTranslation + Quaternion.quatRotate(ref parent.worldRotation, ref localTranslation);
                worldRotation = parent.worldRotation * localRotation;
            }
        }

        internal void updateSimObjectPosition()
        {
            updatePosition(ref worldTranslation, ref worldRotation);
        }

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

        public Vector3 LocalTranslation
        {
            get
            {
                return localTranslation;
            }
            set
            {
                localTranslation = value;
            }
        }

        public Quaternion LocalRotation
        {
            get
            {
                return localRotation;
            }
            set
            {
                localRotation = value;
            }
        }

        public IKJoint Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        internal bool AllowRotation(int i)
        {
            return allowRotation[i];
        }
    }
}
