using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using OgreWrapper;
using OgrePlugin;
using Engine.Platform;
using Engine.Attributes;

namespace Medical
{
    public class MeasurementDrawer : Behavior
    {
        [Editable]
        String nodeName = "Node";

        [Editable]
        String manualObjectName = "ManualObject";

        [Editable]
        private bool drawLines = false;

        private SceneNodeElement sceneNode;
        private ManualObject manualObject;

        protected override void constructed()
        {
            sceneNode = Owner.getElement(nodeName) as SceneNodeElement;
            if (sceneNode == null)
            {
                blacklist("Cannot find scene node named {0}.", nodeName);
            }
            manualObject = sceneNode.getNodeObject(manualObjectName) as ManualObject;
            if (manualObject == null)
            {
                blacklist("Cannot find manual object named {0}.", manualObjectName);
            }
            manualObject.setVisible(drawLines);
            manualObject.begin("MeasurementNoDepth", OperationType.OT_LINE_LIST);
            manualObject.position(ref Vector3.Zero);
            manualObject.color(0, 0, 0, 0);
            manualObject.position(ref Vector3.Zero);
            manualObject.color(0, 0, 0, 0);
            manualObject.end();
            MeasurementController.setMeasurementDrawer(this);
        }

        protected override void destroy()
        {
            MeasurementController.setMeasurementDrawer(null);
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            if (drawLines)
            {
                manualObject.beginUpdate(0);
                foreach (Measurement measurement in MeasurementController.Measurements)
                {
                    measurement.draw(this);
                }
                manualObject.end();
            }
        }

        internal void drawLine(Color color, Vector3 startPoint, Vector3 endPoint)
        {
            manualObject.position(ref startPoint);
            manualObject.color(color.r, color.g, color.b, color.a);
            manualObject.position(ref endPoint);
            manualObject.color(color.r, color.g, color.b, color.a);
        }
        
        [DoNotCopy]
        public bool DrawLines
        {
            get
            {
                return drawLines;
            }
            set
            {
                drawLines = value;
                if (manualObject != null)
                {
                    manualObject.setVisible(value);
                }
            }
        }
    }
}
