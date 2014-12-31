using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Attributes;
using Engine;
using Engine.Renderer;
using OgrePlugin;

namespace Medical
{
    public class Axis
    {
        private static Color HIGHLIGHT = new Color(1.0f, 0.5f, 0.0f);
        const float DELTA = 0.3f;

        Box3 axisBox;
        Vector3 direction;
        float length;
        Color color;
        bool selected;
        Vector3 centerOffset;
        bool enabled = true;

        public Axis(Vector3 direction, float length, Color color)
        {
            selected = false;
            this.color = color;
            this.direction = direction;
            this.length = length;
            axisBox = new Box3();
            Vector3[] axes = axisBox.getAxes();
            axes[0] = Vector3.Right;
            axes[1] = Vector3.Up;
            axes[2] = Vector3.Forward;
            setupBox();
        }

        public void setLength(float length)
        {
            this.length = length;
            setupBox();
        }

        private void setupBox()
        {
            Vector3 longExtent = direction * length / 2.0f;
            centerOffset = longExtent;
            if (longExtent.x == 0)
            {
                longExtent.x = DELTA;
            }
            if (longExtent.y == 0)
            {
                longExtent.y = DELTA;
            }
            if (longExtent.z == 0)
            {
                longExtent.z = DELTA;
            }
            axisBox.setExtents(longExtent);
        }

        public void process(Ray3 worldRay, Vector3 boxLocation)
        {
            if (enabled)
            {
                axisBox.setCenter(boxLocation + centerOffset);
                selected = axisBox.testIntersection(ref worldRay);
            }
        }

        public void clearSelection()
        {
            selected = false;
        }

        public Vector3 translate(Vector3 worldTargetPoint)
        {
            if (isSelected())
            {
                return worldTargetPoint * direction;
            }
            else
            {
                return Vector3.Zero;
            }
        }

        public bool isSelected()
        {
            return enabled && selected;
        }

        public void drawLine(DebugDrawingSurface drawSurface, Vector3 origin)
        {
            if (enabled)
            {
                float drawLength = length;
                if (selected)
                {
                    drawSurface.Color = HIGHLIGHT;
                    drawLength += length * .25f;
                }
                else
                {
                    drawSurface.Color = color;
                }
                drawSurface.drawLine(origin, origin + direction * drawLength);
            }
        }

        public void drawSquare(DebugDrawingSurface drawSurface, Vector3 origin)
        {
            if (enabled)
            {
                float drawLength = length;
                if (selected)
                {
                    drawSurface.Color = HIGHLIGHT;
                    drawLength += length * .25f;
                }
                else
                {
                    drawSurface.Color = color;
                }
                Vector3 outPoint = origin + direction * drawLength;
                Vector3 sourcePoint = origin;
                if (direction.x != 0)
                {
                    sourcePoint = origin;
                    sourcePoint.x = outPoint.x;
                    drawSurface.drawLine(sourcePoint, outPoint);
                }
                if (direction.y != 0)
                {
                    sourcePoint = origin;
                    sourcePoint.y = outPoint.y;
                    drawSurface.drawLine(sourcePoint, outPoint);
                }
                if (direction.z != 0)
                {
                    sourcePoint = origin;
                    sourcePoint.z = outPoint.z;
                    drawSurface.drawLine(sourcePoint, outPoint);
                }
            }
        }

        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
                if (!enabled)
                {
                    selected = false;
                }
            }
        }
    }
}
