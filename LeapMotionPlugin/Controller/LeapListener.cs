using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeapMotionPlugin
{
    class LeapListener : Listener
    {
        public override void onInit(Controller controller)
        {
            Logging.Log.Debug("onInit" + controller);
        }

        public override void onConnect(Controller controller)
        {
            Logging.Log.Debug("onConnect" + controller);
        }

        public override void onDisconnect(Controller controller)
        {
            Logging.Log.Debug("onDisconnect" + controller);
        }

        public override void onFrame(Controller controller)
        {
            Logging.Log.Debug("onFrame" + controller);
        }
    }
}
