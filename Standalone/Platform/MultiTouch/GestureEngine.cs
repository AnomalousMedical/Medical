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
        private FastIteratorMap<int, Finger> fingers = new FastIteratorMap<int, Finger>();

        public GestureEngine(MultiTouch multiTouch)
        {
            multiTouch.TouchStarted += new TouchEvent(multiTouch_TouchStarted);
            multiTouch.TouchEnded += new TouchEvent(multiTouch_TouchEnded);
            multiTouch.TouchMoved += new TouchEvent(multiTouch_TouchMoved);
            multiTouch.AllTouchesCanceled += new TouchCanceledEvent(multiTouch_AllTouchesCanceled);
        }

        public void addGesture(Gesture gesture)
        {
            gestures.Add(gesture);
        }

        public void removeGesture(Gesture gesture)
        {
            gestures.Remove(gesture);
        }

        public void processGestures(Clock clock)
        {
            bool notProcessed = true;
            foreach (Gesture gesture in gestures)
            {
                if (notProcessed && gesture.processFingers(fingers.List))
                {
                    notProcessed = false;
                }
                gesture.additionalProcessing(clock);
            }
            foreach (Finger finger in fingers)
            {
                finger.captureCurrentPositionAsLast();
            }
        }

        void multiTouch_TouchMoved(TouchInfo info)
        {
            Finger finger;
            if (!fingers.TryGetValue(info.id, out finger))
            {
                multiTouch_TouchStarted(info);
                finger = fingers[info.id];
            }
            finger.setCurrentPosition(info.normalizedX, info.normalizedY);
            //Log.Debug("GestureEngine Touch moved {0} {1} {2}", info.id, info.normalizedX, info.normalizedY);
        }

        void multiTouch_TouchEnded(TouchInfo info)
        {
            Finger finger;
            if (fingers.TryGetValue(info.id, out finger))
            {
                fingers.Remove(info.id);
                finger.finished();
            }
            //Log.Debug("GestureEngine Touch ended {0} {1} {2}", info.id, info.normalizedX, info.normalizedY);
        }

        void multiTouch_TouchStarted(TouchInfo info)
        {
            Finger finger;
            if (!fingers.TryGetValue(info.id, out finger))
            {
                finger = fingerPool.getPooledObject();
                fingers.Add(info.id, finger);
            }
            finger.setInfoOutOfPool(info.id, info.normalizedX, info.normalizedY);
            //Log.Debug("GestureEngine Touch started {0} {1} {2}", info.id, info.normalizedX, info.normalizedY);
        }

        void multiTouch_AllTouchesCanceled()
        {
            fingers.Clear();
            //Log.Debug("All touches canceled");
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
            processGestures(clock);
        }

        #endregion
    }
}
