using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using MyGUIPlugin;

namespace Medical.Controller
{
    /// <summary>
    /// This class watches the input system and updates the MDILayoutManager as needed.
    /// </summary>
    class MDIUpdate : UpdateListener
    {
        private EventManager eventManager;
        MDILayoutManager mdiManager;
        bool[] mouseButtonsDown = new bool[(int)MouseButtonCode.NUM_BUTTONS];

        public MDIUpdate(EventManager eventManager, MDILayoutManager mdiManager)
        {
            this.eventManager = eventManager;
            this.mdiManager = mdiManager;
        }

        public void exceededMaxDelta()
        {
            
        }

        public void loopStarting()
        {
            
        }

        public void sendUpdate(Clock clock)
        {
            //Mouse
            Mouse mouse = eventManager.Mouse;
            Vector3 mousePos = mouse.getAbsMouse();
            mdiManager.injectMouseLocation(mousePos);
            for (int i = 0; i < mouseButtonsDown.Length; i++)
            {
                bool down = mouse.buttonDown((MouseButtonCode)i);
                if (down != mouseButtonsDown[i])
                {
                    if (down)
                    {
                        mdiManager.injectMouseDown(mousePos, (MouseButtonCode)i, Gui.Instance.HandledMouse);
                    }
                    else
                    {
                        mdiManager.injectMouseUp(mousePos, (MouseButtonCode)i, Gui.Instance.HandledMouse);
                    }
                    mouseButtonsDown[i] = down;
                }
            }
        }
    }
}
