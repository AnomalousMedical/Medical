using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Renderer;
using Engine.Platform;

namespace Medical
{
    public class RotateTool : Tool
    {
        private static Vector3 YAW = new Vector3(1.0f, 0.0f, 0.0f);
        private static Vector3 PITCH = new Vector3(0.0f, 1.0f, 0.0f);
        private static Vector3 ROLL = new Vector3(0.0f, 0.0f, 1.0f);
        private const float RADIUS_DELTA = 1.0f;

        private DebugDrawingSurface circleSurface;

        private RotateController rotateController;
        private RotationAxis xAxis;
        private RotationAxis yAxis;
        private RotationAxis zAxis;
        private Vector3 currentEulerRotation;
        private Quaternion startingRotation = new Quaternion();
        private CameraMotionValidator activeValidator = null;
        private bool allowMotionUpdates = true;
        private float currentRadius = 5.0f;
        private Quaternion newRot = new Quaternion();
        private Vector3 translation = Vector3.Zero;
        private String name;
        private bool enabled = true;
        private Vector3 savedOrigin = Vector3.Zero; //The origin of the tool when it was destroyed.

        public RotateTool(String name, RotateController rotateController)
        {
            this.name = name;
            xAxis = new RotationAxis(Vector3.Backward, Vector3.Up, ROLL, currentRadius, new Color(1.0f, 0.0f, 0.0f));
            yAxis = new RotationAxis(Vector3.Backward, Vector3.Right, YAW, currentRadius, new Color(0.0f, 0.0f, 1.0f));
            zAxis = new RotationAxis(Vector3.Right, Vector3.Up, PITCH, currentRadius, new Color(0.0f, 1.0f, 0.0f));
            this.rotateController = rotateController;
            rotateController.OnRotationChanged += new RotationChanged(rotateController_OnRotationChanged);
        }

        public void update(EventManager events)
        {
            //Process the mouse
            Mouse mouse = events.Mouse;
            Vector3 mouseLoc = mouse.getAbsMouse();
            CameraMotionValidator validator = activeValidator;
            if (validator == null)
            {
                validator = CameraResolver.getValidatorForLocation((int)mouseLoc.x, (int)mouseLoc.y);
            }
            if (validator != null)
            {
                validator.getLocalCoords(ref mouseLoc.x, ref mouseLoc.y);
                processSelection(events, validator, mouse, ref mouseLoc);
            }

            //Check for resize
            if (events[ToolEvents.IncreaseToolSize].FirstFrameUp)
            {
                currentRadius += RADIUS_DELTA;
                resizeAxes();
            }
            else if (events[ToolEvents.DecreaseToolSize].FirstFrameUp)
            {
                if (currentRadius - RADIUS_DELTA > 0)
                {
                    currentRadius -= RADIUS_DELTA;
                    resizeAxes();
                }
            }
        }

        private void resizeAxes()
        {
            xAxis.setRadius(currentRadius);
            yAxis.setRadius(currentRadius);
            zAxis.setRadius(currentRadius);
        }

        public bool AllowPicking { get; set; }

        private void processSelection(EventManager events, CameraMotionValidator validator, Mouse mouse, ref Vector3 mouseLoc)
        {
            Vector3 trans = translation;
            CameraControl camera = validator.getCamera();
            Ray3 spaceRay = camera.getCameraToViewportRay(mouseLoc.x / validator.getMouseAreaWidth(), mouseLoc.y / validator.getMouseAreaHeight());
            if (events[ToolEvents.Pick].FirstFrameDown && (xAxis.isSelected() || yAxis.isSelected() || zAxis.isSelected()))
            {
                startingRotation.setEuler(currentEulerRotation.x, currentEulerRotation.y, currentEulerRotation.z);
                currentEulerRotation = Vector3.Zero;
                activeValidator = validator;
            }
            else if (events[ToolEvents.Pick].Down && (xAxis.isSelected() || yAxis.isSelected() || zAxis.isSelected()))
            {
                Vector3 relMouse = mouse.getRelMouse();
                float amount = relMouse.x + relMouse.y;
                amount /= 100;
                xAxis.computeRotation(ref currentEulerRotation, amount);
                yAxis.computeRotation(ref currentEulerRotation, amount);
                zAxis.computeRotation(ref currentEulerRotation, amount);
                newRot.setEuler(currentEulerRotation.x, currentEulerRotation.y, currentEulerRotation.z);
                newRot *= startingRotation;
                allowMotionUpdates = false;
                rotateController.setRotation(ref newRot, this);
                allowMotionUpdates = true;
            }
            else if (events[ToolEvents.Pick].FirstFrameUp && (xAxis.isSelected() || yAxis.isSelected() || zAxis.isSelected()))
            {
                newRot.setEuler(currentEulerRotation.x, currentEulerRotation.y, currentEulerRotation.z);
                newRot *= startingRotation;
                currentEulerRotation = newRot.getEuler();
                activeValidator = null;
            }
            else
            {
                processAxis(ref spaceRay);
            }
        }

        private void processAxis(ref Ray3 spaceRay)
        {
            xAxis.process(spaceRay, translation);
            yAxis.process(spaceRay, translation);
            zAxis.process(spaceRay, translation);

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

            drawCircles();
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

        private void drawCircles()
        {
            circleSurface.begin("RotateTool", DrawingType.LineList);
            xAxis.draw(circleSurface, Vector3.Zero);
            yAxis.draw(circleSurface, Vector3.Zero);
            zAxis.draw(circleSurface, Vector3.Zero);
            circleSurface.end();
        }

        void rotateController_OnRotationChanged(Quaternion newRotation, object sender)
        {
            if (allowMotionUpdates)
            {
                currentEulerRotation = newRotation.getEuler();
            }
        }

        #region Tool Members

        public void setEnabled(bool enabled)
        {
            if (circleSurface != null)
            {
                circleSurface.setVisible(enabled);
            }
            this.enabled = enabled;
        }

        public void createSceneElement(Engine.ObjectManagement.SimSubScene subScene, PluginManager pluginManager)
        {
            circleSurface = pluginManager.RendererPlugin.createDebugDrawingSurface(name, subScene);
            if (circleSurface != null)
            {
                circleSurface.setVisible(enabled);
                circleSurface.moveOrigin(savedOrigin);
            }
        }

        public void destroySceneElement(Engine.ObjectManagement.SimSubScene subScene, PluginManager pluginManager)
        {
            if (circleSurface != null)
            {
                savedOrigin = circleSurface.getOrigin();
                pluginManager.RendererPlugin.destroyDebugDrawingSurface(circleSurface);
                circleSurface = null;
            }
        }

        public void setTranslation(Vector3 newTranslation)
        {
            if (circleSurface != null)
            {
                circleSurface.moveOrigin(newTranslation);
                translation = newTranslation;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        #endregion
    }
}
