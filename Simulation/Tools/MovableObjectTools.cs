using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.Renderer;

namespace Medical
{
    class MovableObjectTools
    {
        private MovableObject movable;
        private MoveTool moveTool;

        public MovableObjectTools(String name, MovableObject movable)
        {
            this.movable = movable;
            moveTool = new MoveTool(name, movable, 1.0f);
        }

        public bool processAxes(ref Ray3 spaceRay)
        {
            return moveTool.processAxis(ref spaceRay);
        }

        public void processSelection(EventManager events, ref Vector3 cameraPos, ref Ray3 spaceRay)
        {
            moveTool.processSelection(events, ref cameraPos, ref spaceRay);
        }

        public void drawTools(DebugDrawingSurface axisSurface)
        {
            moveTool.drawAxis(axisSurface);
        }

        public MovableObject Movable
        {
            get
            {
                return movable;
            }
        }
    }
}
