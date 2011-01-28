using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;
using Engine.Platform;
using Engine;

namespace Medical
{
    class GestureEngine : UpdateListener
    {
        private List<Gesture> gestures = new List<Gesture>();
        GenericObjectPool<Finger> fingerPool = new GenericObjectPool<Finger>();
        private Dictionary<int, Finger> fingers = new Dictionary<int, Finger>();//Values coming in for ids are not always 0 based so better to use a dictionary
        private List<Finger> fingerList = new List<Finger>();

        public GestureEngine(MultiTouch multiTouch)
        {
            multiTouch.TouchStarted += new TouchEvent(multiTouch_TouchStarted);
            multiTouch.TouchEnded += new TouchEvent(multiTouch_TouchEnded);
            multiTouch.TouchMoved += new TouchEvent(multiTouch_TouchMoved);
        }

        public void addGesture(Gesture gesture)
        {
            gestures.Add(gesture);
        }

        public void removeGesture(Gesture gesture)
        {
            gestures.Remove(gesture);
        }

        public void processGestures()
        {
            if (fingers.Count > 0)
            {
                foreach (Gesture gesture in gestures)
                {
                    if (gesture.processFingers(fingerList))
                    {
                        break;
                    }
                }
                foreach (Finger finger in fingerList)
                {
                    finger.captureCurrentPositionAsLast();
                }
            }
        }

        void multiTouch_TouchMoved(TouchInfo info)
        {
            Finger finger = fingers[info.id];
            finger.setCurrentPosition(info.normalizedX, info.normalizedY);
            //Log.Debug("GestureEngine Touch moved {0} {1} {2}", info.id, info.normalizedX, info.normalizedY);
        }

        void multiTouch_TouchEnded(TouchInfo info)
        {
            Finger finger = fingers[info.id];
            fingers.Remove(info.id);
            fingerList.Remove(finger);
            finger.finished();
            //Log.Debug("GestureEngine Touch ended {0} {1} {2}", info.id, info.normalizedX, info.normalizedY);
        }

        void multiTouch_TouchStarted(TouchInfo info)
        {
            Finger finger = fingerPool.getPooledObject();
            finger.setInfoOutOfPool(info.id, info.normalizedX, info.normalizedY);
            fingers.Add(info.id, finger);
            fingerList.Add(finger);
            //Log.Debug("GestureEngine Touch started {0} {1} {2}", info.id, info.normalizedX, info.normalizedY);
        }

        #region UpdateListener Members

        public void exceededMaxDelta()
        {
            
        }

        public void loopStarting()
        {
            
        }

        public void sendUpdate(Clock clock)
        {
            processGestures();
        }

        #endregion
    }
}
