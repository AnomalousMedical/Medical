using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using Logging;

namespace Medical
{
    class TeethTimerUpdate : UpdateListener
    {
        enum TeethEvents
        {
            VertexSelect,
        }

        static TeethTimerUpdate()
        {
            MessageEvent vertexSelect = new MessageEvent(TeethEvents.VertexSelect);
            vertexSelect.addButton(MouseButtonCode.MB_BUTTON0);
            DefaultEvents.registerDefaultEvent(vertexSelect);
        }

        EventLayer eventLayer;

        public TeethTimerUpdate(EventLayer eventLayer)
        {
            this.eventLayer = eventLayer;
        }

        public void exceededMaxDelta()
        {
            
        }

        public void loopStarting()
        {
            
        }

        public void sendUpdate(Clock clock)
        {
            if (eventLayer[TeethEvents.VertexSelect].Down)
            {
                //Process the mouse
                Mouse mouse = eventLayer.Mouse;
                Vector3 mouseLoc = mouse.getAbsMouse();
                Ray3 spaceRay = new Ray3();
                Vector3 cameraPos = Vector3.Zero;
                CameraMotionValidator validator = CameraResolver.getValidatorForLocation((int)mouseLoc.x, (int)mouseLoc.y);
                if (validator != null)
                {
                    validator.getLocalCoords(ref mouseLoc.x, ref mouseLoc.y);
                    SceneView camera = validator.getCamera();
                    spaceRay = camera.getCameraToViewportRay(mouseLoc.x / validator.getMouseAreaWidth(), mouseLoc.y / validator.getMouseAreaHeight());
                    cameraPos = camera.Translation;
                    throw new NotImplementedException();
                    //Tooth tooth = TeethController.pickTooth(spaceRay, cameraPos);
                    //if (tooth != null)
                    //{
                    //    Log.Debug("{0}", tooth.Owner.Name);
                    //}
                }
            }
        }
    }
}
