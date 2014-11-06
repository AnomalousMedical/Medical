using BEPUikPlugin;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Pose
{
    public interface PoseableIdentifier
    {
        bool checkCollision(Ray3 ray, ref float distanceOnRay);

        PoseHandler PoseHandler { get; }
    }
}
