using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical
{
    class GUIGestureBlocker : Gesture
    {
        public bool processFingers(IReadOnlyList<Finger> fingers)
        {
            return Gui.Instance.HandledMouseButtons;
        }

        public void additionalProcessing(Engine.Platform.Clock clock)
        {
            
        }
    }
}
