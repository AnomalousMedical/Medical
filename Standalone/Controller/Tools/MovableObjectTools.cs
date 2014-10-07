using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.Renderer;

namespace Medical
{
    public enum MovementPlane
    {
        XY = 1,
        XZ = 2,
        YZ = 4,
        All = XY | XZ | YZ,
    }

    public enum MovementAxis
    {
        X = 1,
        Y = 2,
        Z = 4,
        All = X | Y | Z,
    }

    class MovableObjectTools
    {
        private MovableObject movable;
        private MoveTool moveTool;
        private RotateTool rotateTool;

        public MovableObjectTools(String name, MovableObject movable, float size)
        {
            this.movable = movable;
            moveTool = new MoveTool(name, movable, size);
            MoveToolVisible = false;
            rotateTool = new RotateTool(name, movable, size * 0.3f);
        }

        public bool checkBoundingBoxCollision(ref Ray3 spaceRay)
        {
            return movable.ShowTools && ((MoveToolVisible && moveTool.checkBoundingBoxCollision(ref spaceRay)) || (RotateToolVisible && rotateTool.checkBoundingBoxCollision(ref spaceRay)));
        }

        public bool processAxes(ref Ray3 spaceRay)
        {
            if (MoveToolVisible)
            {
                return moveTool.processAxis(ref spaceRay);
            }
            if (RotateToolVisible)
            {
                return rotateTool.processAxis(ref spaceRay);
            }
            return false;
        }

        public void pickStarted(EventLayer events, ref Vector3 cameraPos, ref Ray3 spaceRay)
        {
            if (MoveToolVisible)
            {
                float distance = (cameraPos - movable.ToolTranslation).length();
                Vector3 spacePoint = spaceRay.Direction * distance + spaceRay.Origin;

                moveTool.moveStarted(ref spacePoint);
            }
            if (RotateToolVisible)
            {
                rotateTool.rotateStarted();
            }
        }

        public void pickHeld(EventLayer events, ref Vector3 cameraPos, ref Ray3 spaceRay)
        {
            if (MoveToolVisible)
            {
                float distance = (cameraPos - movable.ToolTranslation).length();
                Vector3 spacePoint = spaceRay.Direction * distance + spaceRay.Origin;

                moveTool.move(ref spacePoint);
            }
            if (RotateToolVisible)
            {
                Mouse mouse = events.Mouse;
                IntVector3 relMouse = mouse.RelativePosition;
                float amount = relMouse.x + relMouse.y;
                amount /= 100;

                rotateTool.addRotation(amount);
            }
        }

        public void clearSelection()
        {
            moveTool.clearSelection();
            rotateTool.clearSelection();
        }

        public void setActivePlanes(MovementPlane activePlanes)
        {
            moveTool.setActivePlanes(activePlanes);
        }

        public void setActiveAxes(MovementAxis activeAxes)
        {
            moveTool.setActiveAxes(activeAxes);
        }

        public void drawTools(DebugDrawingSurface axisSurface)
        {
            moveTool.drawAxis(axisSurface);
            rotateTool.drawCircles(axisSurface);
        }

        public void destroyToolDrawings(DebugDrawingSurface axisSurface)
        {
            moveTool.destroyAxisDrawings(axisSurface);
            rotateTool.destroyCircleDrawings(axisSurface);
        }

        public void setToolSize(float size)
        {
            moveTool.resizeAxes(size);
            rotateTool.resizeAxes(size * 0.3f);
        }

        public MovableObject Movable
        {
            get
            {
                return movable;
            }
        }

        public bool MoveToolVisible
        {
            get
            {
                return moveTool.Visible;
            }
            set
            {
                moveTool.Visible = value;
            }
        }

        public bool RotateToolVisible
        {
            get
            {
                return rotateTool.Visible;
            }
            set
            {
                rotateTool.Visible = value;
            }
        }
    }
}
