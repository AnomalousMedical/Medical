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
                    InputManager.Instance.injectScrollGesture((int)(fingers[0].X * Gui.Instance.getViewWidth()), (int)(fingers[0].Y * Gui.Instance.getViewHeight()), (int)(fingers[0].DeltaX * Gui.Instance.getViewWidth()), (int)(fingers[0].DeltaY * Gui.Instance.getViewHeight()));
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
