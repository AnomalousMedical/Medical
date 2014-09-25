using Engine;
using Engine.Attributes;
using Engine.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Spine
{
    class SpineRoot : Behavior, SpineSegment
    {
        [DoNotCopy]
        [DoNotSave]
        private SpineSegment childSegment;

        public override void update(Clock clock, EventManager eventManager)
        {
            //try not to do this every frame
            updatePosition();
        }

        public void setChildSegment(SpineSegment segment)
        {
            childSegment = segment;
        }

        public void updatePosition()
        {
            if (childSegment != null)
            {
                childSegment.updatePosition();
            }
        }
    }
}
