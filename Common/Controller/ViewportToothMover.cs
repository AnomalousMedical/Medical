using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using OgreWrapper;

namespace Medical.Controller
{
    enum ToothMoverEvents
    {
        ClickButton
    }

    public class ViewportToothMover : UpdateListener
    {
        static ViewportToothMover()
        {
            MessageEvent clickButton = new MessageEvent(ToothMoverEvents.ClickButton);
            clickButton.addButton(MouseButtonCode.MB_BUTTON0);
            DefaultEvents.registerDefaultEvent(clickButton);
        }

        DrawingWindowController windowController;
        EventManager eventManager;

        public ViewportToothMover(DrawingWindowController windowController, EventManager eventManager)
        {
            this.windowController = windowController;
            this.eventManager = eventManager;
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
            MessageEvent clickButton = eventManager[ToothMoverEvents.ClickButton];
            if (clickButton.FirstFrameDown)
            {
                DrawingWindowHost currentWindowHost = windowController.getActiveWindow();
                if (currentWindowHost != null)
                {
                    DrawingWindow drawingWindow = currentWindowHost.DrawingWindow;
                    Vector3 mousePos = eventManager.Mouse.getAbsMouse();
                    if (drawingWindow.allowMotion((int)mousePos.x, (int)mousePos.y))
                    {
                        drawingWindow.getLocalCoords(ref mousePos.x, ref mousePos.y);
                        Ray3 vpRay = drawingWindow.Camera.getCameraToViewportRay(mousePos.x, mousePos.y);
                        
                    }
                }
            }
        }

        #endregion
    }
}
