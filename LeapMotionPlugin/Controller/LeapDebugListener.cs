using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;

namespace LeapMotionPlugin
{
    class LeapDebugListener : Listener
    {
        private Object thisLock = new Object();

        private void SafeWriteLine(String line)
        {
            lock (thisLock)
            {
                ThreadManager.invoke(new Action(() =>
                {
                    Logging.Log.Debug(line);
                }));
            }
        }

        public override void onInit(Controller controller)
        {
            SafeWriteLine("Initialized");
        }

        public override void onConnect(Controller controller)
        {
            SafeWriteLine("Connected");
        }

        public override void onDisconnect(Controller controller)
        {
            SafeWriteLine("Disconnected");
        }

        public override void onFrame(Controller controller)
        {
            // Get the most recent frame and report some basic information
            Frame frame = controller.frame();
            HandArray hands = frame.hands();
            int numHands = hands.Count;
            SafeWriteLine("Frame id: " + frame.id()
                        + ", timestamp: " + frame.timestamp()
                        + ", hands: " + numHands);

            if (numHands >= 1)
            {
                // Get the first hand
                Hand hand = hands[0];

                // Check if the hand has any fingers
                FingerArray fingers = hand.fingers();
                int numFingers = fingers.Count;
                if (numFingers >= 1)
                {
                    // Calculate the hand's average finger tip position
                    Vector pos = new Vector(0, 0, 0);
                    foreach (Finger finger in fingers)
                    {
                        Ray tip = finger.tip();
                        pos.x += tip.position.x;
                        pos.y += tip.position.y;
                        pos.z += tip.position.z;
                    }
                    pos = new Vector(pos.x / numFingers, pos.y / numFingers, pos.z / numFingers);
                    SafeWriteLine("Hand has " + numFingers + " fingers with average tip position"
                                + " (" + pos.x + ", " + pos.y + ", " + pos.z + ")");
                }

                // Check if the hand has a palm
                Ray palmRay = hand.palm();
                if (palmRay != null)
                {
                    // Get the palm position and wrist direction
                    Vector palm = palmRay.position;
                    Vector wrist = palmRay.direction;
                    string direction = "";
                    if (wrist.x > 0)
                        direction = "left";
                    else
                        direction = "right";
                    SafeWriteLine("Hand is pointing to the " + direction + " with palm position"
                                + " (" + palm.x + ", " + palm.y + ", " + palm.z + ")");
                }
            }
        }
    }
}
