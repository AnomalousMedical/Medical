using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    interface Gesture
    {
        /// <summary>
        /// Process the fingers that are down. Return true if this gesture was
        /// recognized and you want to skip processing any others.
        /// </summary>
        /// <param name="fingers">The list of fingers to process.</param>
        /// <returns></returns>
        bool processFingers(List<Finger> fingers);

        /// <summary>
        /// This method will be called after all processFingers for all methods
        /// has been called. This is where you could process momentum or other
        /// stuff that needs to happen each frame to the gestures.
        /// </summary>
        /// <param name="clock"></param>
        void additionalProcessing(Clock clock);
    }
}
