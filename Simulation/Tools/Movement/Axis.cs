using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Attributes;
using Engine;
using Engine.Renderer;
using OgreWrapper;

namespace Medical
{
    public class Axis
    {
        private static Color HIGHLIGHT = new Color(1.0f, 1.0f, 0.0f);
        const float DELTA = 0.3f;

        Box3 axisBox;
        Vector3 direction;
        float length;
        Color color;
        bool selected;
        Vector3 centerOffset;

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
            axisBox.setCenter(boxLocation + centerOffset);
            selected = axisBox.testIntersection(ref worldRay);
        }

        public void clearSelection()
        {
            selected = false;
        }

        public Vector3 translate(Vector3 worldTargetPoint)
        {
            if (selected)
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
            return selected;
        }

        public void drawLine(DebugDrawingSurface drawSurface)
        {
            if (selected)
            {
                drawSurface.setColor(HIGHLIGHT);
            }
            else
            {
                drawSurface.setColor(color);
            }
            drawSurface.drawLine(Vector3.Zero, direction * length);
        }

        public void drawLine(ManualObject drawSurface)
        {
            if (selected)
            {
                drawLine(drawSurface, HIGHLIGHT, Vector3.Zero, direction * length);
            }
            else
            {
                drawLine(drawSurface, HIGHLIGHT, Vector3.Zero, direction * length);
            }

        }

        public void drawSquare(DebugDrawingSurface drawSurface)
        {
            if (selected)
            {
                drawSurface.setColor(HIGHLIGHT);
            }
            else
            {
                drawSurface.setColor(color);
            }
            Vector3 outPoint = direction * length;
            Vector3 sourcePoint;
            if (outPoint.x != 0)
            {
                sourcePoint = Vector3.Zero;
                sourcePoint.x = outPoint.x;
                drawSurface.drawLine(sourcePoint, outPoint);
            }
            if (outPoint.y != 0)
            {
                sourcePoint = Vector3.Zero;
                sourcePoint.y = outPoint.y;
                drawSurface.drawLine(sourcePoint, outPoint);
            }
            if (outPoint.z != 0)
            {
                sourcePoint = Vector3.Zero;
                sourcePoint.z = outPoint.z;
                drawSurface.drawLine(sourcePoint, outPoint);
            }
        }

        public void drawSquare(ManualObject drawSurface)
        {
            Color drawColor;
            if (selected)
            {
                drawColor = HIGHLIGHT;
            }
            else
            {
                drawColor = color;
            }
            Vector3 outPoint = direction * length;
            Vector3 sourcePoint;
            if (outPoint.x != 0)
            {
                sourcePoint = Vector3.Zero;
                sourcePoint.x = outPoint.x;
                drawLine(drawSurface, drawColor, sourcePoint, outPoint);
            }
            if (outPoint.y != 0)
            {
                sourcePoint = Vector3.Zero;
                sourcePoint.y = outPoint.y;
                drawLine(drawSurface, drawColor, sourcePoint, outPoint);
            }
            if (outPoint.z != 0)
            {
                sourcePoint = Vector3.Zero;
                sourcePoint.z = outPoint.z;
                drawLine(drawSurface, drawColor, sourcePoint, outPoint);
            }
        }

        private void drawLine(ManualObject manual, Color drawColor, Vector3 p1, Vector3 p2)
        {
            manual.position(ref p1);
            manual.color(drawColor.r, drawColor.g, drawColor.b, drawColor.a);
            manual.position(ref p2);
            manual.color(drawColor.r, drawColor.g, drawColor.b, drawColor.a);
        }
    }
}
