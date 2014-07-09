using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using MyGUIPlugin;
using Engine;

namespace Medical
{
    public class GuiGestures : Gesture
    {
        private bool didGesture;

        public GuiGestures()
        {

        }
        
        public bool processFingers(List<Finger> fingers)
        {
            if (Gui.Instance.HandledMouseButtons)
            {
                didGesture = true;
                if (fingers.Count == 1)
                {
                    InputManager.Instance.injectScrollGesture(fingers[0].PixelX, fingers[0].PixelY, fingers[0].PixelDeltaX, fingers[0].PixelDeltaY);
                }
            }
            return didGesture;
        }

        public void additionalProcessing(Clock clock)
        {
            didGesture = false;
        }
    }
}
