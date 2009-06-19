using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using Engine.ObjectManagement;
using OgreWrapper;

namespace Medical.Teeth
{
    class ToothMover
    {
        const float LENGTH_DELTA = 1.0f;
        const float DOUBLE_AXIS_SCALE = 3.0f;

        private Axis xAxisBox;
        private Axis yAxisBox;
        private Axis zAxisBox;
        private Axis xzAxisBox;
        private Axis xyAxisBox;
        private Axis yzAxisBox;
        private Vector3 mouseOffset;
        private float currentLength = 10.0f;
        private SimObject Owner;
        private ManualObject manualObject;

        public ToothMover(SimObject owner, ManualObject manualObject)
        {
            this.Owner = owner;
            this.manualObject = manualObject;
            xAxisBox = new Axis(Vector3.Right, currentLength, new Color(1.0f, 0.0f, 0.0f));
            yAxisBox = new Axis(Vector3.Up, currentLength, new Color(0.0f, 0.0f, 1.0f));
            zAxisBox = new Axis(Vector3.Backward, currentLength, new Color(0.0f, 1.0f, 0.0f));
            xzAxisBox = new Axis(Vector3.Right + Vector3.Backward, currentLength / DOUBLE_AXIS_SCALE, new Color(1.0f, 0.0f, 1.0f));
            xyAxisBox = new Axis(Vector3.Right + Vector3.Up, currentLength / DOUBLE_AXIS_SCALE, new Color(1.0f, 0.0f, 1.0f));
            yzAxisBox = new Axis(Vector3.Up + Vector3.Backward, currentLength / DOUBLE_AXIS_SCALE, new Color(1.0f, 0.0f, 1.0f));
        }

        public Vector3 processSelection(EventManager events, CameraMotionValidator validator, ref Vector3 mouseLoc)
        {
            Vector3 trans = Owner.Translation;
            CameraControl camera = validator.getCamera();
            Ray3 spaceRay = camera.getCameraToViewportRay(mouseLoc.x / validator.getMouseAreaWidth(), mouseLoc.y / validator.getMouseAreaHeight());
            float distance = (camera.Translation - Owner.Translation).length();
            Vector3 spacePoint = spaceRay.Direction * distance + spaceRay.Origin;
            if (events[ToolEvents.Pick].FirstFrameDown)
            {
                mouseOffset = -(spacePoint - Owner.Translation);
            }
            else if (events[ToolEvents.Pick].Down)
            {
                spacePoint += -Owner.Translation + mouseOffset;

                Vector3 newPos = xAxisBox.translate(spacePoint)
                    + yAxisBox.translate(spacePoint)
                    + zAxisBox.translate(spacePoint)
                    + xzAxisBox.translate(spacePoint)
                    + xyAxisBox.translate(spacePoint)
                    + yzAxisBox.translate(spacePoint);

                return newPos;
            }
            else
            {
                processAxis(ref spaceRay);
            }
            return Vector3.Zero;
        }

        private void processAxis(ref Ray3 spaceRay)
        {
            xzAxisBox.process(spaceRay, Owner.Translation);
            xyAxisBox.process(spaceRay, Owner.Translation);
            yzAxisBox.process(spaceRay, Owner.Translation);
            xAxisBox.process(spaceRay, Owner.Translation);
            yAxisBox.process(spaceRay, Owner.Translation);
            zAxisBox.process(spaceRay, Owner.Translation);

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

            drawAxis();
        }

        private void drawAxis()
        {
            manualObject.beginUpdate(0);
            xAxisBox.drawLine(manualObject);
            yAxisBox.drawLine(manualObject);
            zAxisBox.drawLine(manualObject);
            xyAxisBox.drawSquare(manualObject);
            xzAxisBox.drawSquare(manualObject);
            yzAxisBox.drawSquare(manualObject);
            manualObject.end();
        }
    }
}
