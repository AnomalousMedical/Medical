using Engine;
using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Spine
{
    class SpineIKSegment : Interface
    {
        [Editable]
        String targetSimObjectName;

        [Editable]
        String targetPositionBroadcasterName = "PositionBroadcaster";
    }
}
