using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Leap;

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

        public override void OnInit(Controller controller)
        {
            SafeWriteLine("Initialized");
        }

        public override void OnConnect(Controller controller)
        {
            SafeWriteLine("Connected");
        }

        public override void OnDisconnect(Controller controller)
        {
            SafeWriteLine("Disconnected");
        }

        public override void OnFrame(Controller controller)
        {
            // Get the most recent frame and report some basic information
            Frame frame = controller.Frame();
            HandList hands = frame.Hands;
            int numHands = hands.Count;
            SafeWriteLine("Frame id: " + frame.Id
                        + ", timestamp: " + frame.Timestamp
                        + ", hands: " + numHands);

            if (numHands >= 1)
            {
                // Get the first hand
                Hand hand = hands[0];

                // Check if the hand has any fingers
                FingerList fingers = hand.Fingers;
                int numFingers = fingers.Count;
                if (numFingers >= 1)
                {
                    // Calculate the hand's average finger tip position
                    Vector pos = new Vector(0, 0, 0);
                    foreach (Finger finger in fingers)
                    {
                        pos.x += finger.TipPosition.x;
                        pos.y += finger.TipPosition.y;
                        pos.z += finger.TipPosition.z;
                    }
                    pos = new Vector(pos.x / numFingers, pos.y / numFingers, pos.z / numFingers);
                    SafeWriteLine("Hand has " + numFingers + " fingers with average tip position"
                                + " (" + pos.x + ", " + pos.y + ", " + pos.z + ")");
                }

                // Check if the hand has a palm
                    // Get the palm position and wrist direction
                    Vector palm = hand.PalmPosition;
                    Vector wrist = hand.PalmNormal;
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
