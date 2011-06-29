using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;

namespace Medical
{
    class PoseableThumb
    {
        public PoseableThumb(Skeleton skeleton, String metacarpalName, String proximalName, String distalName)
        {
            Metacarpal = new PoseableFingerSection(skeleton, metacarpalName);
            ProximalPhalanges = new PoseableFingerSection(skeleton, proximalName);
            DistalPhalanges = new PoseableFingerSection(skeleton, distalName);
        }

        public PoseableFingerSection Metacarpal { get; private set; }

        public PoseableFingerSection ProximalPhalanges { get; private set; }

        public PoseableFingerSection DistalPhalanges { get; private set; }
    }
}
