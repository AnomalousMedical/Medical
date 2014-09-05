using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.ObjectManagement;
using Engine.Renderer;
using Engine.Platform;
using Medical.Controller;

namespace Medical
{
    public class SimObjectMover : IDisposable
    {
        static SimObjectMover()
        {
            MessageEvent pickEvent = new MessageEvent(ToolEvents.Pick, EventLayers.Tools);
            pickEvent.addButton(MouseButtonCode.MB_BUTTON0);
            DefaultEvents.registerDefaultEvent(pickEvent);

            MessageEvent increaseToolSize = new MessageEvent(ToolEvents.IncreaseToolSize, EventLayers.Tools);
            increaseToolSize.addButton(KeyboardButtonCode.KC_EQUALS);
            DefaultEvents.registerDefaultEvent(increaseToolSize);

            MessageEvent decreaseToolSize = new MessageEvent(ToolEvents.DecreaseToolSize, EventLayers.Tools);
            decreaseToolSize.addButton(KeyboardButtonCode.KC_MINUS);
            DefaultEvents.registerDefaultEvent(decreaseToolSize);
        }

        private RendererPlugin rendererPlugin;
        private String name;
        private DebugDrawingSurface drawingSurface = null;
        private List<MovableObjectTools> movableObjects = new List<MovableObjectTools>();
        private EventLayer events;
        private MovableObjectTools currentTools = null;
        private bool showMoveTools = false;
        private bool showRotateTools = false;
        private float toolSize = 1.0f;
        private SceneViewController sceneViewController;

        public SimObjectMover(String name, RendererPlugin rendererPlugin, EventManager eventManager, SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
            this.rendererPlugin = rendererPlugin;
            this.name = name;
            this.events = eventManager[EventLayers.Tools];
            events.OnUpdate += events_OnUpdate;
        }

        public void Dispose()
        {
            events.OnUpdate -= events_OnUpdate;
            destroyDrawingSurface();
        }

        public void sceneLoaded(SimScene scene)
        {
            SimSubScene subScene = scene.getDefaultSubScene();
            if (subScene != null)
            {
                drawingSurface = rendererPlugin.createDebugDrawingSurface(name + "DebugSurface", subScene);
                drawingSurface.setDepthTesting(false);
            }
        }

        public void sceneUnloading(SimScene scene)
        {
            destroyDrawingSurface();
        }

        private void destroyDrawingSurface()
        {
            if (drawingSurface != null)
            {
                rendererPlugin.destroyDebugDrawingSurface(drawingSurface);
                drawingSurface = null;
            }
        }

        void events_OnUpdate(EventLayer eventLayer)
        {
            //Process the mouse
            Mouse mouse = events.Mouse;
            IntVector3 mouseLoc = mouse.AbsolutePosition;
            Ray3 spaceRay = new Ray3();
            Vector3 cameraPos = Vector3.Zero;
            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            if (activeWindow != null)
            {
                spaceRay = activeWindow.getCameraToViewportRayScreen(mouseLoc.x, mouseLoc.y);
                cameraPos = activeWindow.Translation;              
            }
            //Check collisions and draw shapes
            if (!events[ToolEvents.Pick].HeldDown)
            {
                float closestDistance = float.MaxValue;
                MovableObjectTools closestTools = null;
                foreach (MovableObjectTools tools in movableObjects)
                {
                    if (tools.checkBoundingBoxCollision(ref spaceRay))
                    {
                        if (tools.processAxes(ref spaceRay))
                        {
                            float distance = (tools.Movable.ToolTranslation - cameraPos).length2();
                            if (distance < closestDistance)
                            {
                                //If we had a previous closer tool clear its selection.
                                if (closestTools != null)
                                {
                                    closestTools.clearSelection();
                                }
                                closestTools = tools;
                                closestDistance = distance;
                            }
                            //If this tool was not closer clear its selection.
                            else
                            {
                                tools.clearSelection();
                            }
                        }
                        else
                        {
                            tools.clearSelection();
                        }
                    }
                    else
                    {
                        tools.clearSelection();
                    }
                }
                if (events[ToolEvents.Pick].FirstFrameDown)
                {
                    currentTools = closestTools;
                    if (closestTools != null)
                    {
                        closestTools.processSelection(events, ref cameraPos, ref spaceRay);
                        events.alertEventsHandled();
                    }
                }
                else if(events[ToolEvents.Pick].FirstFrameUp)
                {
                    if(currentTools != null)
                    {
                        events.alertEventsHandled();
                    }
                    currentTools = null;
                }
            }
            else
            {
                if (currentTools != null)
                {
                    currentTools.processSelection(events, ref cameraPos, ref spaceRay);
                    events.alertEventsHandled();
                }
            }
            foreach (MovableObjectTools tools in movableObjects)
            {
                tools.drawTools(drawingSurface);
            }
        }

        public void addMovableObject(String name, MovableObject movable)
        {
            MovableObjectTools tools = new MovableObjectTools(name, movable, toolSize);
            movableObjects.Add(tools);
            tools.MoveToolVisible = showMoveTools;
            tools.RotateToolVisible = showRotateTools;
        }

        public void removeMovableObject(MovableObject movable)
        {
            MovableObjectTools tools = null;
            foreach (MovableObjectTools currentTools in movableObjects)
            {
                if (currentTools.Movable == movable)
                {
                    tools = currentTools;
                    break;
                }
            }
            if (tools != null)
            {
                movableObjects.Remove(tools);
            }
        }

        public void setDrawingSurfaceVisible(bool visible)
        {
            if (drawingSurface != null)
            {
                drawingSurface.setVisible(visible);
            }
        }

        public void setActivePlanes(MovementAxis axes, MovementPlane planes)
        {
            foreach (MovableObjectTools currentTools in movableObjects)
            {
                currentTools.setActiveAxes(axes);
                currentTools.setActivePlanes(planes);
            }
        }

        public bool ShowMoveTools
        {
            get
            {
                return showMoveTools;
            }
            set
            {
                showMoveTools = value;
                foreach (MovableObjectTools tools in movableObjects)
                {
                    tools.MoveToolVisible = value;
                }
            }
        }

        public bool ShowRotateTools
        {
            get
            {
                return showRotateTools;
            }
            set
            {
                showRotateTools = value;
                foreach (MovableObjectTools tools in movableObjects)
                {
                    tools.RotateToolVisible = value;
                }
            }
        }

        public float ToolSize
        {
            get
            {
                return toolSize;
            }
            set
            {
                toolSize = value;
                foreach (MovableObjectTools tools in movableObjects)
                {
                    tools.setToolSize(toolSize);
                }
            }
        }
    }
}
