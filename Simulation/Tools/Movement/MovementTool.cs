using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Engine.Attributes;
using Engine.Renderer;
using Engine;
using Engine.Platform;

namespace Medical
{
    public class MovementTool : Behavior
    {
        #region Static

        static MovementTool()
        {
            MessageEvent pickEvent = new MessageEvent(ToolEvents.Pick);
            pickEvent.addButton(MouseButtonCode.MB_BUTTON0);
            DefaultEvents.registerDefaultEvent(pickEvent);

            MessageEvent increaseToolSize = new MessageEvent(ToolEvents.IncreaseToolSize);
            increaseToolSize.addButton(KeyboardButtonCode.KC_EQUALS);
            DefaultEvents.registerDefaultEvent(increaseToolSize);

            MessageEvent decreaseToolSize = new MessageEvent(ToolEvents.DecreaseToolSize);
            decreaseToolSize.addButton(KeyboardButtonCode.KC_MINUS);
            DefaultEvents.registerDefaultEvent(decreaseToolSize);
        }

        const float LENGTH_DELTA = 1.0f;
        const float DOUBLE_AXIS_SCALE = 3.0f;

        #endregion Static

        #region Fields

        private DebugDrawingSurface axisSurface;
        private Axis xAxisBox;
        private Axis yAxisBox;
        private Axis zAxisBox;
        private Axis xzAxisBox;
        private Axis xyAxisBox;
        private Axis yzAxisBox;
        private Vector3 mouseOffset;
        private float currentLength = 10.0f;
        private String name;
        private bool enabled = true;
        private Vector3 savedOrigin = Vector3.Zero; //The origin of the tool when it was destroyed.

        #endregion Fields

        #region Constructors

        public MovementTool(String name)
        {
            this.name = name;
            xAxisBox = new Axis(Vector3.Right, currentLength, new Color(1.0f, 0.0f, 0.0f));
            yAxisBox = new Axis(Vector3.Up, currentLength, new Color(0.0f, 0.0f, 1.0f));
            zAxisBox = new Axis(Vector3.Backward, currentLength, new Color(0.0f, 1.0f, 0.0f));
            xzAxisBox = new Axis(Vector3.Right + Vector3.Backward, currentLength / DOUBLE_AXIS_SCALE, new Color(1.0f, 0.0f, 1.0f));
            xyAxisBox = new Axis(Vector3.Right + Vector3.Up, currentLength / DOUBLE_AXIS_SCALE, new Color(1.0f, 0.0f, 1.0f));
            yzAxisBox = new Axis(Vector3.Up + Vector3.Backward, currentLength / DOUBLE_AXIS_SCALE, new Color(1.0f, 0.0f, 1.0f));
        }

        #endregion Constructors

        #region Functions

        public void createSceneElement(SimSubScene subScene, PluginManager pluginManager)
        {
            axisSurface = pluginManager.RendererPlugin.createDebugDrawingSurface(name, subScene);
            if (axisSurface != null)
            {
                axisSurface.setVisible(enabled);
                axisSurface.moveOrigin(savedOrigin);
            }
        }

        public void destroySceneElement(SimSubScene subScene, PluginManager pluginManager)
        {
            if (axisSurface != null)
            {
                savedOrigin = axisSurface.getOrigin();
                pluginManager.RendererPlugin.destroyDebugDrawingSurface(axisSurface);
                axisSurface = null;
            }
        }

        public override void update(Clock clock, EventManager events)
        {
            if (axisSurface != null)
            {
                //Process the mouse
                Mouse mouse = events.Mouse;
                Vector3 mouseLoc = mouse.getAbsMouse();
                CameraMotionValidator validator = CameraResolver.getValidatorForLocation((int)mouseLoc.x, (int)mouseLoc.y);
                if (validator != null)
                {
                    validator.getLocalCoords(ref mouseLoc.x, ref mouseLoc.y);
                    processSelection(events, validator, ref mouseLoc);
                }

                //Check for resize
                if (events[ToolEvents.IncreaseToolSize].FirstFrameUp)
                {
                    currentLength += LENGTH_DELTA;
                    resizeAxes();
                }
                else if (events[ToolEvents.DecreaseToolSize].FirstFrameUp)
                {
                    if (currentLength - LENGTH_DELTA > 0)
                    {
                        currentLength -= LENGTH_DELTA;
                        resizeAxes();
                    }
                }
            }
        }

        public void setToolActive(bool enabled)
        {
            if (axisSurface != null)
            {
                axisSurface.setVisible(enabled);
            }
            this.enabled = enabled;
        }

        public void setTranslation(Vector3 newTranslation)
        {
            if (axisSurface != null)
            {
                axisSurface.moveOrigin(newTranslation);
            }
        }

        private void resizeAxes()
        {
            xAxisBox.setLength(currentLength);
            yAxisBox.setLength(currentLength);
            zAxisBox.setLength(currentLength);
            xyAxisBox.setLength(currentLength / DOUBLE_AXIS_SCALE);
            xzAxisBox.setLength(currentLength / DOUBLE_AXIS_SCALE);
            yzAxisBox.setLength(currentLength / DOUBLE_AXIS_SCALE);
        }

        private void processSelection(EventManager events, CameraMotionValidator validator, ref Vector3 mouseLoc)
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

                newPos += Owner.Translation;
                updateTranslation(ref newPos);
            }
            else
            {
                processAxis(ref spaceRay);
            }
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
            axisSurface.begin("MoveTool", DrawingType.LineList);
            xAxisBox.drawLine(axisSurface);
            yAxisBox.drawLine(axisSurface);
            zAxisBox.drawLine(axisSurface);
            xyAxisBox.drawSquare(axisSurface);
            xzAxisBox.drawSquare(axisSurface);
            yzAxisBox.drawSquare(axisSurface);
            axisSurface.end();
        }

        #endregion Functions
    }
}
