using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;

namespace Medical
{
    public class MultiFingerScrollGesture : Gesture
    {
        public delegate void ScrollDelegate(float deltaX, float deltaY);
        public event ScrollDelegate Scroll;
        private int fingerCount;
        private bool didGesture;
        private Vector2 momentum = new Vector2();
        private Vector2 momentumDirection = new Vector2();
        private Vector2 deceleration = new Vector2();
        private float decelerationTime;
        private float minimumMomentum;

        public MultiFingerScrollGesture(int fingerCount, float decelerationTime, float minimumMomentum)
        {
            this.fingerCount = fingerCount;
            this.decelerationTime = decelerationTime;
            this.minimumMomentum = minimumMomentum;
        }

        public bool processFingers(List<Finger> fingers)
        {
            if (fingers.Count == fingerCount)
            {
                Vector2 primaryFingerVec = new Vector2(fingers[0].NrmlDeltaX, fingers[0].NrmlDeltaY);
                float primaryFingerLen = primaryFingerVec.length();
                Vector2 longestLengthVec = primaryFingerVec;
                float longestLength = primaryFingerLen;
                if (primaryFingerLen > 0)
                {
                    bool allVectorsSameDirection = true;
                    for (int i = 1; i < fingerCount && allVectorsSameDirection; ++i)
                    {
                        Vector2 testFingerVec = new Vector2(fingers[i].NrmlDeltaX, fingers[i].NrmlDeltaY);
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
                        momentum = longestLengthVec;
                        momentumDirection = new Vector2(1.0f, 1.0f);
                        if (momentum.x < 0.0f)
                        {
                            momentum.x = -momentum.x;
                            momentumDirection.x = -1.0f;
                        }
                        if (momentum.y < 0.0f)
                        {
                            momentum.y = -momentum.y;
                            momentumDirection.y = -1.0f;
                        }
                        if (momentum.x < minimumMomentum)
                        {
                            momentum.x = 0.0f;
                        }
                        if (momentum.y < minimumMomentum)
                        {
                            momentum.y = 0.0f;
                        }
                        deceleration = momentum / decelerationTime;
                    }
                }
            }

            return didGesture;
        }

        public void additionalProcessing(Clock clock)
        {
            if (!didGesture)
            {
                if (momentum.length2() != 0.0f)
                {
                    momentum -= deceleration * clock.DeltaSeconds;
                    if (momentum.x < 0.0f)
                    {
                        momentum.x = 0.0f;
                    }
                    if (momentum.y <= 0.0f)
                    {
                        momentum.y = 0.0f;
                    }
                    if(Scroll != null)
                    {
                        Vector2 finalMomentum = momentum * momentumDirection;
                        Scroll.Invoke(finalMomentum.x, finalMomentum.y);
                    }
                }
            }

            didGesture = false;
        }
    }
}
