using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;

namespace LeapMotionPlugin
{
    class LeapListener : Listener
    {
        public override void onInit(Controller controller)
        {
            ThreadManager.invoke(new Action(() =>
            {
                Logging.Log.Debug("onInit" + controller);
            }));
        }

        public override void onConnect(Controller controller)
        {
            ThreadManager.invoke(new Action(() =>
            {
                Logging.Log.Debug("onConnect" + controller);
            }));
        }

        public override void onDisconnect(Controller controller)
        {
            ThreadManager.invoke(new Action(() =>
            {
                Logging.Log.Debug("onDisconnect" + controller);
            }));
        }

        public override void onFrame(Controller controller)
        {
            ThreadManager.invoke(new Action(() =>
            {
                Logging.Log.Debug("onFrame" + controller);
            }));
        }
    }
}
