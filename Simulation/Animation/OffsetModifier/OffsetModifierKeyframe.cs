using Engine;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public interface OffsetModifierKeyframe : Saveable
    {
        void blendFrom(OffsetModifierKeyframe previousFrame, float percentage, SimObjectFollowerWithRotation follower);

        void deriveOffsetFromFollower(SimObjectFollowerWithRotation follower);

        float BlendAmount { get; set; }

        EditInterface EditInterface { get; }

        IEnumerable<OffsetModifierMovableSection> MovableSections { get; }
    }
}
