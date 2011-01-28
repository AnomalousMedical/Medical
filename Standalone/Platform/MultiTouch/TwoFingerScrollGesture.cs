using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    class TwoFingerScrollGesture : Gesture
    {
        public delegate void ScrollDelegate(float deltaX, float deltaY);

        public event ScrollDelegate Scroll;

        public bool processFingers(List<Finger> fingers)
        {
            bool didGesture = false;

            if (fingers.Count == 2)
            {
                Vector2 finger1Vec = new Vector2(fingers[0].DeltaX, fingers[0].DeltaY);
                Vector2 finger2Vec = new Vector2(fingers[1].DeltaX, fingers[1].DeltaY);
                float finger1Len = finger1Vec.length2();
                float finger2Len = finger2Vec.length2();
                if (finger1Len > 0 && finger2Len > 0)
                {
                    float cosTheta = finger1Vec.dot(ref finger2Vec) / (finger1Vec.length() * finger2Vec.length());
                    if (cosTheta > 0.5f && Scroll != null)
                    {
                        didGesture = true;
                        if (finger1Len > finger2Len)
                        {
                            Scroll.Invoke(finger1Vec.x, finger1Vec.y);
                        }
                        else
                        {
                            Scroll.Invoke(finger2Vec.x, finger2Vec.y);
                        }
                    }
                }
            }

            return didGesture;
        }
    }
}
