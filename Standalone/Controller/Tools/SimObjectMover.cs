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
        private static readonly ButtonEvent PickEvent;
        private static readonly ButtonEvent IncreaseToolSize;
        private static readonly ButtonEvent DecreaseToolSize;

        static SimObjectMover()
        {
            PickEvent = new ButtonEvent(EventLayers.Tools);
            PickEvent.addButton(MouseButtonCode.MB_BUTTON0);
            DefaultEvents.registerDefaultEvent(PickEvent);

            IncreaseToolSize = new ButtonEvent(EventLayers.Tools);
            IncreaseToolSize.addButton(KeyboardButtonCode.KC_EQUALS);
            DefaultEvents.registerDefaultEvent(IncreaseToolSize);

            DecreaseToolSize = new ButtonEvent(EventLayers.Tools);
            DecreaseToolSize.addButton(KeyboardButtonCode.KC_MINUS);
            DefaultEvents.registerDefaultEvent(DecreaseToolSize);
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
        private bool visible = false;

        public SimObjectMover(String name, RendererPlugin rendererPlugin, EventManager eventManager, SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
            this.rendererPlugin = rendererPlugin;
            this.name = name;
            this.events = eventManager[EventLayers.Tools];
            events.OnUpdate += events_OnUpdate;

            ToolSizeIncrement = 1.0f;
            ToolSizeMinimum = 1.0f;

            IncreaseToolSize.FirstFrameUpEvent += IncreaseToolSize_FirstFrameUpEvent;
            DecreaseToolSize.FirstFrameUpEvent += DecreaseToolSize_FirstFrameUpEvent;
        }

        public void Dispose()
        {
            IncreaseToolSize.FirstFrameUpEvent -= IncreaseToolSize_FirstFrameUpEvent;
            DecreaseToolSize.FirstFrameUpEvent -= DecreaseToolSize_FirstFrameUpEvent;
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
                drawingSurface.setVisible(visible);
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
            Ray3 spaceRay = new Ray3();
            Vector3 cameraPos = Vector3.Zero;
            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            if (activeWindow != null)
            {
                IntVector3 mouseLoc = events.Mouse.AbsolutePosition;
                spaceRay = activeWindow.getCameraToViewportRayScreen(mouseLoc.x, mouseLoc.y);
                cameraPos = activeWindow.Translation;              
            }
            //Check collisions and draw shapes
            if (!PickEvent.HeldDown)
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
                if (PickEvent.FirstFrameDown)
                {
                    currentTools = closestTools;
                    if (closestTools != null)
                    {
                        closestTools.pickStarted(events, ref cameraPos, ref spaceRay);
                        events.alertEventsHandled();
                    }
                }
                else if(PickEvent.FirstFrameUp)
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
                    currentTools.pickHeld(events, ref cameraPos, ref spaceRay);
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

        public void setActivePlanes(MovementAxis axes, MovementPlane planes)
        {
            foreach (MovableObjectTools currentTools in movableObjects)
            {
                currentTools.setActiveAxes(axes);
                currentTools.setActivePlanes(planes);
            }
        }

        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                if (visible != value)
                {
                    this.visible = value;
                    if (drawingSurface != null)
                    {
                        drawingSurface.setVisible(visible);
                    }
                }
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

        /// <summary>
        /// How much to change the tool size on the increase / decrease size events.
        /// </summary>
        public float ToolSizeIncrement { get; set; }

        /// <summary>
        /// The minimum tool size.
        /// </summary>
        public float ToolSizeMinimum { get; set; }

        void DecreaseToolSize_FirstFrameUpEvent(EventLayer eventLayer)
        {
            if(eventLayer.EventProcessingAllowed && (showMoveTools || showRotateTools) && visible && toolSize - ToolSizeIncrement >= ToolSizeMinimum)
            {
                ToolSize -= ToolSizeIncrement;
            }
        }

        void IncreaseToolSize_FirstFrameUpEvent(EventLayer eventLayer)
        {
            if (eventLayer.EventProcessingAllowed && (showMoveTools || showRotateTools) && visible)
            {
                ToolSize += ToolSizeIncrement;
            }
        }
    }
}
