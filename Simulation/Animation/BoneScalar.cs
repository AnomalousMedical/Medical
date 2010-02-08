using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using OgreWrapper;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine.Attributes;
using Logging;

namespace Medical
{
    class BoneScalar : BoneManipulator
    {
        [Editable]
        Vector3 startScale = Vector3.ScaleIdentity;

        [Editable]
        Vector3 endScale = Vector3.Zero;

        [Editable]
        float defaultPosition = 0.0f;

        [DoNotCopy]
        [DoNotSave]
        private Vector3 lengthRange;

        [DoNotCopy]
        [DoNotSave]
        private int nonZeroAxis = 0;

        protected override void constructed()
        {
            base.constructed();
            lengthRange = endScale - startScale;
            if (lengthRange.x != 0.0f)
            {
                nonZeroAxis = 0;
            }
            else if (lengthRange.y != 0.0f)
            {
                nonZeroAxis = 1;
            }
            else if (lengthRange.z != 0.0f)
            {
                nonZeroAxis = 2;
            }
        }

        protected override void positionUpdated(float position)
        {
            if (bone != null)
            {
                bone.setScale(startScale.lerp(ref endScale, ref position));
                bone.needUpdate(true);
            }
        }

        public override AnimationManipulatorStateEntry createStateEntry()
        {
            return new BoneScalarStateEntry(UIName, Scale);
        }

        public override float DefaultPosition
        {
            get 
            {
                return defaultPosition;
            }
        }

        [Editable]
        [DoNotCopy]
        public float DefaultXSetter
        {
            get
            {
                return 0;
            }
            set
            {
                float val = (value - endScale.x) / (startScale.x - endScale.x) - 1;
                defaultPosition = Math.Abs(val);
                Position = defaultPosition;
            }
        }

        [DoNotCopy]
        public Vector3 Scale
        {
            get
            {
                return bone.getScale();
            }
            set
            {
                bone.setScale(value);
                bone.needUpdate(true);
                switch(nonZeroAxis)
                {
                    case 0:
                        currentPosition = (value.x - startScale.x) / (lengthRange.x);
                        break;
                    case 1:
                        currentPosition = (value.y - startScale.y) / (lengthRange.y);
                        break;
                    case 2:
                        currentPosition = (value.z - startScale.z) / (lengthRange.z);
                        break;
                }
            }
        }
    }
}
