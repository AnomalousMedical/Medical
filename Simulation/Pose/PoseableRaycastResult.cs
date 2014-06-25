using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class PoseableRaycastResult
    {
        public PoseableRaycastResult(PoseableIdentifier poseableIdentifier, float distance)
        {
            this.PoseableIdentifier = poseableIdentifier;
            this.Distance = distance;
        }

        public PoseableIdentifier PoseableIdentifier { get; private set; }

        public float Distance { get; private set; }
    }
}
