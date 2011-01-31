using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;

namespace Medical
{
    class MultiFingerScrollGesture : Gesture
    {
        public delegate void ScrollDelegate(float deltaX, float deltaY);
        public event ScrollDelegate Scroll;
        private int fingerCount;

        public MultiFingerScrollGesture(int fingerCount)
        {
            this.fingerCount = fingerCount;
        }

        public bool processFingers(List<Finger> fingers)
        {
            bool didGesture = false;

            if (fingers.Count == fingerCount)
            {
                Vector2 primaryFingerVec = new Vector2(fingers[0].DeltaX, fingers[0].DeltaY);
                float primaryFingerLen = primaryFingerVec.length();
                Vector2 longestLengthVec = primaryFingerVec;
                float longestLength = primaryFingerLen;
                if (primaryFingerLen > 0)
                {
                    bool allVectorsSameDirection = true;
                    for (int i = 1; i < fingerCount && allVectorsSameDirection; ++i)
                    {
                        Vector2 testFingerVec = new Vector2(fingers[i].DeltaX, fingers[i].DeltaY);
                        float testFingerLen = testFingerVec.length();
                        if (testFingerLen > 0)
                        {
                            float cosTheta = primaryFingerVec.dot(ref testFingerVec) / (primaryFingerLen * testFingerLen);
                            if (cosTheta > 0.5f)
                            {
                                if(testFingerLen > longestLength)
                                {
                                    longestLengthVec = testFingerVec;
                                    longestLength = testFingerLen;
                                }
                            }
                            else
                            {
                                allVectorsSameDirection = false;
                            }
                        }
                    }
                    if (allVectorsSameDirection && Scroll != null)
                    {
                        Scroll.Invoke(longestLengthVec.x, longestLengthVec.y);
                    }
                }
            }

            return didGesture;
        }

        public void additionalProcessing(Clock clock)
        {

        }
    }
}
