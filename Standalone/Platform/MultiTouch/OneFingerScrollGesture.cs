using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class OneFingerScrollGesture : Gesture
    {
        public delegate void ScrollDelegate(float deltaX, float deltaY);

        public event ScrollDelegate Scroll;

        public bool processFingers(List<Finger> fingers)
        {
            bool didGesture = false;

            if (fingers.Count == 1)
            {
                if (Scroll != null)
                {
                    didGesture = true;
                    Scroll.Invoke(fingers[0].DeltaX, fingers[0].DeltaY);
                }
            }

            return didGesture;
        }
    }
}
