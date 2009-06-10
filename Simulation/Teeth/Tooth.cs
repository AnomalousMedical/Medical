using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;

namespace Medical
{
    public abstract class Tooth : Behavior
    {
        private bool adapting = false;
        private int adaptFrameCount = 0;

        protected override void constructed()
        {
            TeethController.addTooth(Owner.Name, this);
        }

        protected override void destroy()
        {
            TeethController.removeTooth(Owner.Name);
        }

        public override void update(Clock clock, EventManager eventManager)
        {

        }
    }
}
