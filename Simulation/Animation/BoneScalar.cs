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
            }
        }
    }
}
