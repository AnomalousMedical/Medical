using Engine;
using Engine.Attributes;
using Engine.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Animation.Proxy
{
    class ProxyRoot : Behavior, ProxyChainSegment
    {
        [DoNotCopy]
        [DoNotSave]
        private ProxyChainSegment childSegment;

        [DoNotCopy]
        [DoNotSave]
        private bool moved = false;

        public override void update(Clock clock, EventManager eventManager)
        {
            //try not to do this every frame
            updatePosition();
        }

        public void setChildSegment(ProxyChainSegment segment)
        {
            childSegment = segment;
        }

        public void updatePosition()
        {
            if (moved && childSegment != null)
            {
                childSegment.updatePosition();
                moved = false;
            }
        }

        internal void alertChainMoved()
        {
            moved = true;
        }
    }
}
