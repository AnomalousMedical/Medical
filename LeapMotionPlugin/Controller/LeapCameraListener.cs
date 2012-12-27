using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Leap;

namespace LeapMotionPlugin
{
    class LeapCameraListener : Listener
    {
        SceneViewController sceneViewController;
        private Vector lastFingerPos = new Vector(0, 0, 0);
        private bool firstHandFrame = true;

        public LeapCameraListener(SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
        }

        public override void OnFrame(Controller controller)
        {
            // Get the most recent frame and report some basic information
            Frame frame = controller.Frame();
            HandList hands = frame.Hands;
            int numHands = hands.Count;

            if (numHands >= 1)
            {
                // Get the first hand
                Hand hand = hands[0];

                // Check if the hand has any fingers
                FingerList fingers = hand.Fingers;
                int numFingers = fingers.Count;
                if (numFingers > 0)
                {
                    switch (numFingers)
                    {
                        case 1:
                            rotate(fingers);
                            break;
                        case 2:
                            pan(fingers);
                            break;
                    }
                }
            }
            else
            {
                firstHandFrame = true;
            }
        }

        private void rotate(FingerList fingers)
        {
            float sensitivity = 0.1f;
            Finger finger = fingers[0];
            SceneViewWindow window = sceneViewController.ActiveWindow;
            if (window != null)
            {
                Vector currentFingerPos = finger.TipPosition;
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

        private void pan(FingerList fingers)
        {
            float sensitivity = 0.1f;
            Finger finger = fingers[0];
            SceneViewWindow window = sceneViewController.ActiveWindow;
            if (window != null)
            {
                Vector currentFingerPos = finger.TipPosition;
                if (!firstHandFrame)
                {
                    float xDelta = (float)(currentFingerPos.x - lastFingerPos.x) * sensitivity;
                    float yDelta = (float)(lastFingerPos.y - currentFingerPos.y) * sensitivity;
                    ThreadManager.invoke(new Action(() =>
                    {
                        window.pan(xDelta, yDelta);
                    }));
                }
                lastFingerPos = currentFingerPos;
                firstHandFrame = false;
            }
        }
    }
}
