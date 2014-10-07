using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public interface OffsetModifierMovableSection
    {
        Vector3 getTranslation(SimObjectFollowerWithRotation follower);

        void move(Vector3 offset, SimObjectFollowerWithRotation follower);

        Quaternion getRotation(SimObjectFollowerWithRotation follower);

        void setRotation(Quaternion rotation, SimObjectFollowerWithRotation follower);
    }
}
