using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Renderer;
using Engine.Platform;

namespace Medical
{
    public class RotateTool
    {
        private static Vector3 YAW = new Vector3(1.0f, 0.0f, 0.0f);
        private static Vector3 PITCH = new Vector3(0.0f, 1.0f, 0.0f);
        private static Vector3 ROLL = new Vector3(0.0f, 0.0f, 1.0f);

        private RotationAxis xAxis;
        private RotationAxis yAxis;
        private RotationAxis zAxis;
        private Vector3 currentEulerRotation;
        private Quaternion startingRotation = new Quaternion();
        private Quaternion newRot = new Quaternion();
        private String name;
        private MovableObject movable;
        private Box3 boundingBox = new Box3();

        public RotateTool(String name, MovableObject movable, float radius)
        {
            this.name = name;
            this.movable = movable;
            xAxis = new RotationAxis(Vector3.Backward, Vector3.Up, ROLL, radius, new Color(1.0f, 0.0f, 0.0f));
            yAxis = new RotationAxis(Vector3.Backward, Vector3.Right, YAW, radius, new Color(0.0f, 0.0f, 1.0f));
            zAxis = new RotationAxis(Vector3.Right, Vector3.Up, PITCH, radius, new Color(0.0f, 1.0f, 0.0f));
            //Bounding box
            Vector3[] axes = boundingBox.getAxes();
            axes[0] = Vector3.Right;
            axes[1] = Vector3.Up;
            axes[2] = Vector3.Forward;
            Vector3 boundsExtents = new Vector3(radius, radius, radius);
            boundingBox.setExtents(boundsExtents);
            Visible = false;
        }

        public void resizeAxes(float radius)
        {
            xAxis.setRadius(radius);
            yAxis.setRadius(radius);
            zAxis.setRadius(radius);
            Vector3 boundsExtents = new Vector3(radius, radius, radius);
            boundingBox.setExtents(boundsExtents);
        }

        /// <summary>
        /// Call before using addRotation to rotate
        /// </summary>
        public void rotateStarted()
        {
            Vector3 trans = movable.ToolTranslation;

            startingRotation.setEuler(currentEulerRotation.x, currentEulerRotation.y, currentEulerRotation.z);

            Vector3 stRot = movable.ToolRotation.getEuler();
            startingRotation.setEuler(stRot.x, stRot.y, stRot.z);

            currentEulerRotation = Vector3.Zero;
        }

        /// <summary>
        /// Add more rotation specified by amount on the current selected axis. Use rotateStarted before calling this since
        /// the addition is based on the starting rotation.
        /// </summary>
        /// <param name="amount"></param>
        public void addRotation(float amount)
        {
            xAxis.computeRotation(ref currentEulerRotation, amount);
            yAxis.computeRotation(ref currentEulerRotation, amount);
            zAxis.computeRotation(ref currentEulerRotation, amount);
            newRot.setEuler(currentEulerRotation.x, currentEulerRotation.y, currentEulerRotation.z);
            newRot *= startingRotation;
            movable.rotate(newRot);
        }

        public bool processAxis(ref Ray3 spaceRay)
        {
            Vector3 trans = movable.ToolTranslation;
            xAxis.process(spaceRay, trans);
            yAxis.process(spaceRay, trans);
            zAxis.process(spaceRay, trans);

            Vector3 origin = spaceRay.Origin;

            if (xAxis.isSelected() && yAxis.isSelected() && zAxis.isSelected())
            {
                float xAxisDistance = (xAxis.Intersection - origin).length2();
                float yAxisDistance = (yAxis.Intersection - origin).length2();
                float zAxisDistance = (zAxis.Intersection - origin).length2();
                //X closest
                if (xAxisDistance < yAxisDistance && xAxisDistance < zAxisDistance)
                {
                    yAxis.clearSelection();
                    zAxis.clearSelection();
                }
                //Y closest
                else if (yAxisDistance < zAxisDistance && yAxisDistance < xAxisDistance)
                {
                    xAxis.clearSelection();
                    zAxis.clearSelection();
                }
                //Z closest
                else if (zAxisDistance < yAxisDistance && zAxisDistance < xAxisDistance)
                {
                    yAxis.clearSelection();
                    xAxis.clearSelection();
                }
                else
                {
                    xAxis.clearSelection();
                    zAxis.clearSelection();
                }
            }
            else if (xAxis.isSelected() && yAxis.isSelected())
            {
                twoWayAxisComparison(xAxis, yAxis, ref origin);
            }
            else if (xAxis.isSelected() && zAxis.isSelected())
            {
                twoWayAxisComparison(xAxis, zAxis, ref origin);
            }
            else if (zAxis.isSelected() && yAxis.isSelected())
            {
                twoWayAxisComparison(zAxis, yAxis, ref origin);
            }

            bool selected = xAxis.isSelected() || yAxis.isSelected() || zAxis.isSelected();
            movable.alertToolHighlightStatus(selected);

            return selected;
        }

        private void twoWayAxisComparison(RotationAxis axis0, RotationAxis axis1, ref Vector3 origin)
        {
            float axis0Distance = (axis0.Intersection - origin).length2();
            float axis1Distance = (axis1.Intersection - origin).length2();
            if (axis0Distance < axis1Distance)
            {
                axis1.clearSelection();
            }
            else
            {
                axis0.clearSelection();
            }
        }

        public void drawCircles(DebugDrawingSurface circleSurface)
        {
            circleSurface.begin(name + "RotateTool", DrawingType.LineList);
            if (Visible && movable.ShowTools)
            {
                xAxis.draw(circleSurface, movable.ToolTranslation);
                yAxis.draw(circleSurface, movable.ToolTranslation);
                zAxis.draw(circleSurface, movable.ToolTranslation);
            }
            circleSurface.end();
        }

        public void destroyCircleDrawings(DebugDrawingSurface axisSurface)
        {
            axisSurface.destroySection(name + "RotateTool");
        }

        public bool Visible { get; set; }

        internal void clearSelection()
        {
            xAxis.clearSelection();
            yAxis.clearSelection();
            zAxis.clearSelection();
            movable.alertToolHighlightStatus(false);
        }

        internal bool checkBoundingBoxCollision(ref Ray3 spaceRay)
        {
            boundingBox.setCenter(movable.ToolTranslation);
            return boundingBox.testIntersection(spaceRay);
        }
    }
}
