using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;
using Engine;

namespace Medical
{
    public class PoseableFinger
    {
        public PoseableFinger(Skeleton skeleton, String metacarpalName, String proximalName, String intermediateName, String distalName)
        {
            Metacarpal = new PoseableFingerSection(skeleton, metacarpalName);
            ProximalPhalanges = new PoseableFingerSection(skeleton, proximalName);
            IntermediatePhalanges = new PoseableFingerSection(skeleton, intermediateName);
            DistalPhalanges = new PoseableFingerSection(skeleton, distalName);
        }

        public PoseableFingerSection Metacarpal { get; private set; }

        public PoseableFingerSection ProximalPhalanges { get; private set; }

        public PoseableFingerSection IntermediatePhalanges { get; private set; }

        public PoseableFingerSection DistalPhalanges { get; private set; }
    }
}
