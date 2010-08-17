using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    class FixedMedicalUpdate : UpdateListener
    {
        private MedicalController controller;

        public FixedMedicalUpdate(MedicalController controller)
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
            controller._sendFixedUpdate(clock);
        }
    }
}
