using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    class MedicalUpdate : UpdateListener
    {
        private MedicalController controller;

        public MedicalUpdate(MedicalController controller)
        {
            this.controller = controller;
        }

        public void exceededMaxDelta()
        {
            
        }

        public void loopStarting()
        {
            
        }

        public void sendUpdate(Clock clock)
        {
            controller._sendUpdate(clock);
        }
    }
}
