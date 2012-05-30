using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;

namespace Medical.GUI
{
    class RocketRenderSystemListener : RenderSystemListener
    {
        public RocketRenderSystemListener()
        {
            
        }

        public override void eventOccured()
        {
            switch (EventType)
            {
                case KnownRenderSystemEvents.DeviceLost:
                    
                    break;
                case KnownRenderSystemEvents.DeviceRestored:
                    RocketWidgetManager.deviceRestored();
                    break;
            }
        }
    }
}
