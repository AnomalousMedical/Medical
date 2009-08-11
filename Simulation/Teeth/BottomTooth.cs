using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine;
using Logging;

namespace Medical
{
    class BottomTooth : Tooth
    {
        protected override void looseChanged(bool loose)
        {
            
        }

        protected override void offsetChanged(Vector3 offset)
        {
            offset.y *= -1.0f;
            joint.setFrameOffsetA(startingLocation + offset);
            //joint.setFrameOffsetA(startingLocation + Quaternion.quatRotate(joint.getFrameOffsetBasisA(), offset));
        }
    }
}
