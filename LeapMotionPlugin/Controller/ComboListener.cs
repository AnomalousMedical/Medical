using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Leap;

namespace LeapMotionPlugin
{
    class ComboListener : Listener
    {
        SceneViewController sceneViewController;
        private Vector lastFingerPos = new Vector(0, 0, 0);
        private bool firstHandFrame = true;

        public ComboListener(SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
        }

        public override void OnFrame(Controller controller)
        {
            // Get the most recent frame and report some basic information
            Frame frame = controller.Frame();
            HandList hands = frame.Hands;
            int numHands = hands.Count;
            FingerList fingers;

            if (numHands == 1)
            {
                // Get the first hand
                Hand hand = hands[0];

                // Check if the hand has any fingers
                fingers = hand.Fingers;
                int numFingers = fingers.Count;
                if (numFingers > 0)
                {
                    //Find the most forward pointing finger.
                    Finger finger = fingers[0];
                    for (int i = 1; i < numFingers; ++i)
                    {
                        if (finger.Direction.z < fingers[i].Direction.z)
                        {
                            finger = fingers[i];
                        }
                    }
                    //Ray ray = finger.tip();
                    //if (ray != null)
                    {
                        if (finger.Direction.z >= 0.9)
                        {
                            rotate(finger.TipPosition);
                        }
                    }
                }
                else
                {
                    firstHandFrame = true;
                }
            }
            else if (numHands == 2)
            {
                // Get the first hand
                Hand hand = hands[0];
                Hand hand1 = hands[1];
                // Check if the hand has any fingers
                int numFingers = hand.Fingers.Count + hand1.Fingers.Count;
                if (numFingers <= 1)
                {
                        Vector averagePos = new Vector(0, 0, 0);

                        averagePos.x += hand.PalmPosition.x;
                        averagePos.y += hand.PalmPosition.y;
                        averagePos.z += hand.PalmPosition.z;

                        averagePos.x += hand1.PalmPosition.x;
                        averagePos.y += hand1.PalmPosition.y;
                        averagePos.z += hand1.PalmPosition.z;

                        averagePos.x /= 2;
                        averagePos.y /= 2;
                        averagePos.z /= 2;

                        pan(averagePos);
                }
                else
                {
                    firstHandFrame = true;
                }
            }
            else
            {
                firstHandFrame = true;
            }
        }

        private void rotate(Vector currentFingerPos)
        {
            float sensitivity = 0.1f;
            SceneViewWindow window = sceneViewController.ActiveWindow;
            if (window != null)
            {
                if (!firstHandFrame)
                {
                    float yawDelta = (float)(lastFingerPos.x - currentFingerPos.x) * sensitivity;
                    float pitchDelta = (float)(lastFingerPos.y - currentFingerPos.y) * sensitivity;
                    ThreadManager.invoke(new Action(() =>
                    {
                        window.rotate(yawDelta, pitchDelta);
                        window.stopMaintainingIncludePoint();
                    }));
                }
                lastFingerPos = currentFingerPos;
                firstHandFrame = false;
            }
        }

        private void pan(Vector currentFingerPos)
        {
            float sensitivity = 0.8f;
            float zoomSensitivity = 1.0f;
            SceneViewWindow window = sceneViewController.ActiveWindow;
            if (window != null)
            {
                if (!firstHandFrame)
                {
                    float xDelta = (float)(currentFingerPos.x - lastFingerPos.x) * sensitivity;
                    float yDelta = (float)(lastFingerPos.y - currentFingerPos.y) * sensitivity;
                    float zDelta = (float)(lastFingerPos.z - currentFingerPos.z) * zoomSensitivity;
                    ThreadManager.invoke(new Action(() =>
                    {
                        window.pan(xDelta, yDelta);
                        window.zoom(zDelta);
                        window.stopMaintainingIncludePoint();
                    }));
                }
                lastFingerPos = currentFingerPos;
                firstHandFrame = false;
            }
        }
    }
}
