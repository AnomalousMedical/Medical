using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Renderer;
using Engine.ObjectManagement;
using Engine.Platform;

namespace Medical
{
    class MoveTool
    {
        #region Fields

        const float LENGTH_DELTA = 1.0f;
        const float DOUBLE_AXIS_SCALE = 3.0f;

        private Axis xAxisBox;
        private Axis yAxisBox;
        private Axis zAxisBox;
        private Axis xzAxisBox;
        private Axis xyAxisBox;
        private Axis yzAxisBox;
        private Vector3 mouseOffset;
        private String name;
        private bool enabled = true;
        private MovableObject movable;

        #endregion Fields

        #region Constructors

        public MoveTool(String name, MovableObject movable, float startingLength)
        {
            this.movable = movable;
            this.name = name;
            xAxisBox = new Axis(Vector3.Right, startingLength, new Color(1.0f, 0.0f, 0.0f));
            yAxisBox = new Axis(Vector3.Up, startingLength, new Color(0.0f, 0.0f, 1.0f));
            zAxisBox = new Axis(Vector3.Backward, startingLength, new Color(0.0f, 1.0f, 0.0f));
            xzAxisBox = new Axis(Vector3.Right + Vector3.Backward, startingLength / DOUBLE_AXIS_SCALE, new Color(1.0f, 0.0f, 1.0f));
            xyAxisBox = new Axis(Vector3.Right + Vector3.Up, startingLength / DOUBLE_AXIS_SCALE, new Color(1.0f, 0.0f, 1.0f));
            yzAxisBox = new Axis(Vector3.Up + Vector3.Backward, startingLength / DOUBLE_AXIS_SCALE, new Color(1.0f, 0.0f, 1.0f));
        }

        #endregion Constructors

        #region Functions

        public void setEnabled(bool enabled)
        {
            
            this.enabled = enabled;
        }

        public void resizeAxes(float length)
        {
            xAxisBox.setLength(length);
            yAxisBox.setLength(length);
            zAxisBox.setLength(length);
            xyAxisBox.setLength(length / DOUBLE_AXIS_SCALE);
            xzAxisBox.setLength(length / DOUBLE_AXIS_SCALE);
            yzAxisBox.setLength(length / DOUBLE_AXIS_SCALE);
        }

        public void processSelection(EventManager events, ref Vector3 cameraPos, ref Ray3 spaceRay)
        {
            float distance = (cameraPos - movable.ToolTranslation).length();
            Vector3 spacePoint = spaceRay.Direction * distance + spaceRay.Origin;
            if (events[ToolEvents.Pick].FirstFrameDown)
            {
                mouseOffset = -(spacePoint - movable.ToolTranslation);
            }
            else if (events[ToolEvents.Pick].Down)
            {
                spacePoint += -movable.ToolTranslation + mouseOffset;

                Vector3 newPos = xAxisBox.translate(spacePoint)
                    + yAxisBox.translate(spacePoint)
                    + zAxisBox.translate(spacePoint)
                    + xzAxisBox.translate(spacePoint)
                    + xyAxisBox.translate(spacePoint)
                    + yzAxisBox.translate(spacePoint);

                //newPos += movable.ToolTranslation;
                movable.move(newPos);// = newPos;
            }
        }

        public bool processAxis(ref Ray3 spaceRay)
        {
            Vector3 origin = movable.ToolTranslation;
            xzAxisBox.process(spaceRay, origin);
            xyAxisBox.process(spaceRay, origin);
            yzAxisBox.process(spaceRay, origin);
            xAxisBox.process(spaceRay, origin);
            yAxisBox.process(spaceRay, origin);
            zAxisBox.process(spaceRay, origin);

            if (xzAxisBox.isSelected())
            {
                xyAxisBox.clearSelection();
                yzAxisBox.clearSelection();
                xAxisBox.clearSelection();
                yAxisBox.clearSelection();
                zAxisBox.clearSelection();
            }
            else if (xyAxisBox.isSelected())
            {
                xzAxisBox.clearSelection();
                yzAxisBox.clearSelection();
                xAxisBox.clearSelection();
                yAxisBox.clearSelection();
                zAxisBox.clearSelection();
            }
            else if (yzAxisBox.isSelected())
            {
                xzAxisBox.clearSelection();
                xyAxisBox.clearSelection();
                xAxisBox.clearSelection();
                yAxisBox.clearSelection();
                zAxisBox.clearSelection();
            }

            return xzAxisBox.isSelected()
                || xyAxisBox.isSelected()
                || yzAxisBox.isSelected()
                || xAxisBox.isSelected()
                || yAxisBox.isSelected()
                || zAxisBox.isSelected();
        }

        public void drawAxis(DebugDrawingSurface axisSurface)
        {
            Vector3 origin = movable.ToolTranslation;
            axisSurface.begin(name + "MoveTool", DrawingType.LineList);
            if (movable.ShowTools)
            {
                xAxisBox.drawLine(axisSurface, origin);
                yAxisBox.drawLine(axisSurface, origin);
                zAxisBox.drawLine(axisSurface, origin);
                xyAxisBox.drawSquare(axisSurface);
                xzAxisBox.drawSquare(axisSurface);
                yzAxisBox.drawSquare(axisSurface);
            }
            axisSurface.end();
        }

        #endregion
    }
}
