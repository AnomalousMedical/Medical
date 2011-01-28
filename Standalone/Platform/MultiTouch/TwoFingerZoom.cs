using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Logging;

namespace Medical
{
    class TwoFingerZoom : Gesture
    {
        public delegate void ZoomDelegate(float zoomDelta);

        public event ZoomDelegate Zoom;
        public float lastPinchDistance = 0;

        public bool processFingers(List<Finger> fingers)
        {
            bool didGesture = false;

            if (fingers.Count == 2)
            {
                Finger finger1 = fingers[0];
                Finger finger2 = fingers[1];
                Vector2 finger1Vec = new Vector2(finger1.DeltaX, finger1.DeltaY);
                Vector2 finger2Vec = new Vector2(finger2.DeltaX, finger2.DeltaY);
                float finger1Len = finger1Vec.length2();
                float finger2Len = finger2Vec.length2();
                if (finger1Len > 0 && finger2Len > 0)
                {
                    float cosTheta = finger1Vec.dot(ref finger2Vec) / (finger1Vec.length() * finger2Vec.length());
                    if (cosTheta < -0.5f && Zoom != null)
                    {
                        computeZoom(ref didGesture, finger1, finger2, ref finger1Vec, ref finger2Vec);
                    }
                }
                else if (finger1Len == 0 && finger2Len > 0)
                {
                    computeZoom(ref didGesture, finger1, finger2, ref finger1Vec, ref finger2Vec);
                }
                else if (finger2Len == 0 && finger1Len > 0)
                {
                    computeZoom(ref didGesture, finger1, finger2, ref finger1Vec, ref finger2Vec);
                }
            }

            return didGesture;
        }

        private void computeZoom(ref bool didGesture, Finger finger1, Finger finger2, ref Vector2 finger1Vec, ref Vector2 finger2Vec)
        {
            Vector2 finger1Pos = new Vector2(finger1.X, finger1.Y);
            Vector2 finger2Pos = new Vector2(finger2.X, finger2.Y);
            float currentPinchDistance = (finger1Pos - finger2Pos).length2();

            Vector2 vectorSum = finger1Vec - finger2Vec;
            float sumLength = vectorSum.length();
            if (currentPinchDistance > lastPinchDistance)
            {
                sumLength = -sumLength;
            }
            didGesture = true;
            lastPinchDistance = currentPinchDistance;
            if (Zoom != null)
            {
                Zoom.Invoke(sumLength);
            }
        }
    }
}
