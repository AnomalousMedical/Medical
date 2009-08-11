﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using BulletPlugin;
using Engine;

namespace Medical
{
    class TopTooth : Tooth
    {
        protected override void looseChanged(bool loose)
        {
            //if (loose)
            //{
            //    actorElement.clearCollisionFlag(CollisionFlags.KinematicObject);
            //}
            //else
            //{
            //    actorElement.raiseCollisionFlag(CollisionFlags.KinematicObject);
            //}
        }

        protected override void offsetChanged(Vector3 offset)
        {
            joint.setFrameOffsetA(startingLocation + offset);
            //joint.setFrameOffsetA(startingLocation + Quaternion.quatRotate(joint.getFrameOffsetBasisA(), offset));
        }
    }
}
