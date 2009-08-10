using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical
{
    class TopTooth : Tooth
    {
        protected override void looseChanged(bool loose)
        {
            //if (loose)
            //{
            //    actorElement.Actor.clearBodyFlag(PhysXWrapper.BodyFlag.NX_BF_KINEMATIC);
            //}
            //else
            //{
            //    actorElement.Actor.raiseBodyFlag(PhysXWrapper.BodyFlag.NX_BF_KINEMATIC);
            //}
        }
    }
}
