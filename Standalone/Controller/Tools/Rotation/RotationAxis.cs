using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Renderer;

namespace Medical
{
    class RotationAxis
    {
        private static Color HIGHLIGHT = new Color(1.0f, 0.5f, 0.0f);
        private const float DELTA = 0.1f;

        Vector3 xAxis;
        Vector3 yAxis;
        float radius;
        Color color;
        bool selected;
        Box3 axisBox;
        Vector3 eulerAxis;
        Vector3 intersection;

        public RotationAxis(Vector3 xAxis, Vector3 yAxis, Vector3 eulerAxis, float radius, Color color)
        {
            this.eulerAxis = eulerAxis;
            axisBox = new Box3();
            Vector3[] axes = axisBox.getAxes();
            axes[0] = Vector3.Right;
            axes[1] = Vector3.Up;
            axes[2] = Vector3.Forward;
            selected = false;
            this.color = color;
            this.xAxis = xAxis;
            this.yAxis = yAxis;
            setRadius(radius);
        }

        public void process(Ray3 worldRay, Vector3 location)
        {
            Vector3 dontCare;
            int quantity;
            axisBox.setCenter(location);
            //Make sure the ray is within the radius of the circle and the hit box.
            selected = Distance.SqrDistance(ref location, ref worldRay) <= radius * radius && axisBox.findIntersection(ref worldRay, out quantity, out intersection, out dontCare);
        }

        public void draw(DebugDrawingSurface circleHelper, Vector3 origin)
        {
            float drawRadius = radius;
            if (selected)
            {
                circleHelper.Color = HIGHLIGHT;
                drawRadius += radius * .25f;
            }
            else
            {
                circleHelper.Color = color;
            }
            circleHelper.drawCircle(origin, xAxis, yAxis, drawRadius);
        }

        public bool isSelected()
        {
            return selected;
        }

        public void clearSelection()
        {
            selected = false;
        }

        public Vector3 Intersection
        {
            get
            {
                return intersection;
            }
        }

        public void computeRotation(ref Vector3 eulerRotation, float amount)
        {
            if (selected)
            {
                eulerRotation += eulerAxis * amount;
            }
        }

        public void setRadius(float radius)
        {
            this.radius = radius;
            Vector3 extents = xAxis * radius + yAxis * radius;
            if (extents.x == 0)
            {
                extents.x = DELTA;
            }
            if (extents.y == 0)
            {
                extents.y = DELTA;
            }
            if (extents.z == 0)
            {
                extents.z = DELTA;
            }
            axisBox.setExtents(extents.absolute());
        }
    }
}
