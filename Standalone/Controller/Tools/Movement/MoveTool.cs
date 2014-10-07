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
        private Box3 boundingBox = new Box3();

        public MoveTool(String name, MovableObject movable, float startingLength)
        {
            this.movable = movable;
            this.name = name;
            xAxisBox = new Axis(Vector3.Right, startingLength, new Color(1.0f, 0.0f, 0.0f));
            yAxisBox = new Axis(Vector3.Up, startingLength, new Color(0.0f, 0.0f, 1.0f));
            zAxisBox = new Axis(Vector3.Backward, startingLength, new Color(0.0f, 1.0f, 0.0f));
            xzAxisBox = new Axis(Vector3.Right + Vector3.Backward, startingLength / DOUBLE_AXIS_SCALE, new Color(1.0f, 0.0f, 1.0f));
            //xzAxisBox.Enabled = false;
            xyAxisBox = new Axis(Vector3.Right + Vector3.Up, startingLength / DOUBLE_AXIS_SCALE, new Color(1.0f, 0.0f, 1.0f));
            //xyAxisBox.Enabled = false;
            yzAxisBox = new Axis(Vector3.Up + Vector3.Backward, startingLength / DOUBLE_AXIS_SCALE, new Color(1.0f, 0.0f, 1.0f));
            //yzAxisBox.Enabled = false;

            //Bounding box
            Vector3[] axes = boundingBox.getAxes();
            axes[0] = Vector3.Right;
            axes[1] = Vector3.Up;
            axes[2] = Vector3.Forward;
            Vector3 boundsExtents = new Vector3(startingLength / 2.0f, startingLength / 2.0f, startingLength / 2.0f);
            boundingBox.setExtents(boundsExtents);
            Visible = false;
        }

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
            Vector3 boundsExtents = new Vector3(length / 2.0f, length / 2.0f, length / 2.0f);
            boundingBox.setExtents(boundsExtents);
        }

        public bool checkBoundingBoxCollision(ref Ray3 spaceRay)
        {
            boundingBox.setCenter(movable.ToolTranslation + boundingBox.getExtents());
            return boundingBox.testIntersection(spaceRay);
        }

        public void moveStarted(ref Vector3 spacePoint)
        {
            mouseOffset = -(spacePoint - movable.ToolTranslation);
        }

        public void move(ref Vector3 spacePoint)
        {
            spacePoint += -movable.ToolTranslation + mouseOffset;

            Vector3 newPos = xAxisBox.translate(spacePoint)
                + yAxisBox.translate(spacePoint)
                + zAxisBox.translate(spacePoint)
                + xzAxisBox.translate(spacePoint)
                + xyAxisBox.translate(spacePoint)
                + yzAxisBox.translate(spacePoint);

            movable.move(newPos);
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

            bool selected = xzAxisBox.isSelected()
                || xyAxisBox.isSelected()
                || yzAxisBox.isSelected()
                || xAxisBox.isSelected()
                || yAxisBox.isSelected()
                || zAxisBox.isSelected();
            movable.alertToolHighlightStatus(selected);

            return selected;
        }

        public void clearSelection()
        {
            xzAxisBox.clearSelection();
            xyAxisBox.clearSelection();
            yzAxisBox.clearSelection();
            xAxisBox.clearSelection();
            yAxisBox.clearSelection();
            zAxisBox.clearSelection();
            movable.alertToolHighlightStatus(false);
        }

        public void drawAxis(DebugDrawingSurface axisSurface)
        {
            Vector3 origin = movable.ToolTranslation;
            axisSurface.begin(name + "MoveTool", DrawingType.LineList);
            if (Visible && movable.ShowTools)
            {
                xAxisBox.drawLine(axisSurface, origin);
                yAxisBox.drawLine(axisSurface, origin);
                zAxisBox.drawLine(axisSurface, origin);
                xyAxisBox.drawSquare(axisSurface, origin);
                xzAxisBox.drawSquare(axisSurface, origin);
                yzAxisBox.drawSquare(axisSurface, origin);
            }
            //debug bounding box
            //Vector3 bbOrigin = boundingBox.getCenter();
            //axisSurface.setColor(Color.White);
            //axisSurface.drawLine(bbOrigin, bbOrigin + boundingBox.getExtents());
            //axisSurface.drawLine(bbOrigin, bbOrigin - boundingBox.getExtents());
            axisSurface.end();
        }

        public void destroyAxisDrawings(DebugDrawingSurface axisSurface)
        {
            axisSurface.destroySection(name + "MoveTool");
        }

        public void setActivePlanes(MovementPlane activePlanes)
        {
            clearSelection();
            xyAxisBox.Enabled = (activePlanes & MovementPlane.XY) == MovementPlane.XY;
            xzAxisBox.Enabled = (activePlanes & MovementPlane.XZ) == MovementPlane.XZ;
            yzAxisBox.Enabled = (activePlanes & MovementPlane.YZ) == MovementPlane.YZ;
        }

        public void setActiveAxes(MovementAxis activeAxes)
        {
            clearSelection();
            xAxisBox.Enabled = (activeAxes & MovementAxis.X) == MovementAxis.X;
            yAxisBox.Enabled = (activeAxes & MovementAxis.Y) == MovementAxis.Y;
            zAxisBox.Enabled = (activeAxes & MovementAxis.Z) == MovementAxis.Z;
        }

        public bool Visible { get; set; }
    }
}
