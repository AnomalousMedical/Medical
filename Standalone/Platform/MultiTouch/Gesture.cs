using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
