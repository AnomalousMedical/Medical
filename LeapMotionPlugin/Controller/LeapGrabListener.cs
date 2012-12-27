using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Leap;

namespace LeapMotionPlugin
{
    class LeapGrabListener : Listener
    {
        SceneViewController sceneViewController;
        private Vector lastFingerPos = new Vector(0, 0, 0);
        private bool firstHandFrame = true;

        public LeapGrabListener(SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
        }

        public override void OnFrame(Controller controller)
        {
            // Get the most recent frame and report some basic information
            Frame frame = controller.Frame();
            HandList hands = frame.Hands;
            int numHands = hands.Count;

            if (numHands == 1)
            {
                // Get the first hand
                Hand hand = hands[0];

                // Check if the hand has any fingers
                FingerList fingers = hand.Fingers;
                int numFingers = fingers.Count;
                if (numFingers == 0)
                {
                    //grabbing
                    //Ray palm = hand.PalmPosition();
                    //if (palm != null)
                    {
                        rotate(hand.PalmPosition);
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
                    //Ray palm = hand.palm();
                    //Ray palm1 = hand1.palm();
                    //if (palm != null && palm1 != null)
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
                    }));
                }
                lastFingerPos = currentFingerPos;
                firstHandFrame = false;
            }
        }
    }
}
