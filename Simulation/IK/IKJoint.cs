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

        //Newer Wild Magic method
        bool updateLocalR(int i)
        {
            return false;
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
    }
}
