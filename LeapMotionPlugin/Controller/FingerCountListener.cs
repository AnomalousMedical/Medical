using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;

namespace LeapMotionPlugin
{
    class FingerCountListener : Listener
    {
        SceneViewController sceneViewController;
        private Vector lastFingerPos = new Vector(0, 0, 0);
        private bool firstHandFrame = true;

        public FingerCountListener(SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
        }

        public override void onFrame(Controller controller)
        {
            // Get the most recent frame and report some basic information
            Frame frame = controller.frame();
            HandArray hands = frame.hands();
            int numHands = hands.Count;
            FingerArray fingers;

            if (numHands == 2)
            {
                // Get the first hand
                Hand hand = hands[0];

                // Check if the hand has any fingers
                fingers = hand.fingers();
                int numFingers = fingers.Count;
                if (numFingers > 0)
                {
                    //Find the most forward pointing finger.
                    Finger finger = fingers[0];
                    //for (int i = 1; i < numFingers; ++i)
                    //{
                    //    if (finger.tip().direction.z < fingers[i].tip().direction.z)
                    //    {
                    //        finger = fingers[i];
                    //    }
                    //}
                    Ray ray = finger.tip();
                    if (ray != null)
                    {
                        Hand counterHand = hands[1];
                        if (counterHand.fingers().Count == 1)
                        {
                            rotate(ray.position);
                        }
                        else if (counterHand.fingers().Count == 2)
                        {
                            pan(ray.position);
                        }
                        else
                        {
                            firstHandFrame = true;
                        }
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
