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

        private static readonly Matrix3x3 NoParentMatrix = new Matrix3x3(1, 1, 1, 
                                                                         0, 0, 0, 
                                                                         0, 0, 0);
        [DoNotCopy]
        [DoNotSave]
        bool[] allowTranslation = new bool[]{ false, false, false };

        [DoNotCopy]
        [DoNotSave]
        bool[] allowRotation = new bool[] { true, true, true };

        [Editable]
        private String parentSimObjectName;

        private Vector3 localTranslation;
        private Quaternion localRotation;
        private Vector3 worldTranslation;
        private Quaternion worldRotation;

        protected override void link()
        {
            base.link();
            //if (parentSimObjectName != null)
            //{
            //    Parent = Owner.getOtherSimObject(parentSimObjectName);
            //    if (Parent != null) //Initial setup
            //    {
            //        Quaternion inverseParentRot = Parent.Rotation.inverse();
            //        localTranslation = Owner.Translation - Parent.Translation;
            //        localTranslation = Quaternion.quatRotate(inverseParentRot, localTranslation);
            //        localRotation = Owner.Rotation * inverseParentRot;
            //    }
            //    else
            //    {
            //        localTranslation = Vector3.Zero;
            //        localRotation = Quaternion.Identity;
            //    }
            //}
        }

        internal void computeLocalChainOffsets(IKJoint parent)
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

        internal void updateWorldTransforms(IKJoint parent)
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
    }
}
