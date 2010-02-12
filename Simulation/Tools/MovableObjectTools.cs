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

        public MovableObjectTools(String name, MovableObject movable)
        {
            this.movable = movable;
            moveTool = new MoveTool(name, movable, 1.0f);
            MoveToolVisible = false;
            rotateTool = new RotateTool(name, movable, 0.3f);
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

        public void processSelection(EventManager events, ref Vector3 cameraPos, ref Ray3 spaceRay)
        {
            if (MoveToolVisible)
            {
                moveTool.processSelection(events, ref cameraPos, ref spaceRay);
            }
            if (RotateToolVisible)
            {
                rotateTool.processSelection(events, ref spaceRay);
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
