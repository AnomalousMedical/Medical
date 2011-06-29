﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;
using Engine;

namespace Medical
{
    class PoseableFinger
    {
        public PoseableFinger(Skeleton skeleton, String metacarpalName, String proximalName, String middleName, String distalName)
        {
            Metacarpal = new PoseableFingerSection(skeleton, metacarpalName);
            ProximalPhalanges = new PoseableFingerSection(skeleton, proximalName);
            MiddlePhalanges = new PoseableFingerSection(skeleton, middleName);
            DistalPhalanges = new PoseableFingerSection(skeleton, distalName);
        }

        public PoseableFingerSection Metacarpal { get; private set; }

        public PoseableFingerSection ProximalPhalanges { get; private set; }

        public PoseableFingerSection MiddlePhalanges { get; private set; }

        public PoseableFingerSection DistalPhalanges { get; private set; }
    }
}
