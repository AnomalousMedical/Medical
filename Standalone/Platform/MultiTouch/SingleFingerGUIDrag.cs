using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using MyGUIPlugin;
using Engine;

namespace Medical
{
    class GuiGestures : Gesture
    {
        private bool didGesture;
        private IntVector2 lastFingerPos;
        private bool fingerDown = false;

        public GuiGestures()
        {

        }
        
        public bool processFingers(List<Finger> fingers)
        {
            if (Gui.Instance.HandledMouseButtons)
            {
                if (fingers.Count == 1)
                {
                    lastFingerPos = new IntVector2((int)(fingers[0].X * Gui.Instance.getViewWidth()), (int)(fingers[0].Y * Gui.Instance.getViewHeight()));
                    if (fingerDown)
                    {
                        Gui.Instance.injectMousePress(lastFingerPos.x, lastFingerPos.y, MouseButtonCode.MB_BUTTON0);
                    }
                    else
                    {
                        Gui.Instance.injectMouseMove(lastFingerPos.x, lastFingerPos.y, 0);
                    }
                    didGesture = true;
                }
            }
            return didGesture;
        }

        public void additionalProcessing(Clock clock)
        {
            if (!didGesture && fingerDown)
            {
                Gui.Instance.injectMouseRelease(lastFingerPos.x, lastFingerPos.y, MouseButtonCode.MB_BUTTON0);
                fingerDown = false;
            }
            didGesture = false;
        }
    }
}
