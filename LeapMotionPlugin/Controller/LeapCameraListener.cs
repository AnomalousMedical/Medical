using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;

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

        public override void onFrame(Controller controller)
        {
            // Get the most recent frame and report some basic information
            Frame frame = controller.frame();
            HandArray hands = frame.hands();
            int numHands = hands.Count;

            if (numHands >= 1)
            {
                // Get the first hand
                Hand hand = hands[0];

                // Check if the hand has any fingers
                FingerArray fingers = hand.fingers();
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

        private void rotate(FingerArray fingers)
        {
            float sensitivity = 0.1f;
            Finger finger = fingers[0];
            SceneViewWindow window = sceneViewController.ActiveWindow;
            if (window != null)
            {
                Ray tip = finger.tip();
                Vector currentFingerPos = tip.position;
                if (!firstHandFrame)
                {
                    float yawDelta = (float)(lastFingerPos.x - tip.position.x) * sensitivity;
                    float pitchDelta = (float)(lastFingerPos.y - tip.position.y) * sensitivity;
                    ThreadManager.invoke(new Action(() =>
                    {
                        window.rotate(yawDelta, pitchDelta);
                    }));
                }
                lastFingerPos = currentFingerPos;
                firstHandFrame = false;
            }
        }

        private void pan(FingerArray fingers)
        {
            float sensitivity = 0.1f;
            Finger finger = fingers[0];
            SceneViewWindow window = sceneViewController.ActiveWindow;
            if (window != null)
            {
                Ray tip = finger.tip();
                Vector currentFingerPos = tip.position;
                if (!firstHandFrame)
                {
                    float xDelta = (float)(tip.position.x - lastFingerPos.x) * sensitivity;
                    float yDelta = (float)(lastFingerPos.y - tip.position.y) * sensitivity;
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
