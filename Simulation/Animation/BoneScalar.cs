using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using OgreWrapper;
using Engine.ObjectManagement;
using OgrePlugin;

namespace Medical
{
    class BoneScalar : BoneManipulator
    {
        [Editable]
        Vector3 startScale = Vector3.ScaleIdentity;

        [Editable]
        Vector3 endScale = Vector3.Zero;

        public override void positionUpdated(float position, Bone bone)
        {
            if (bone != null)
            {
                bone.setScale(startScale.lerp(ref endScale, ref position));
                bone.needUpdate(true);
            }
        }
    }
}
