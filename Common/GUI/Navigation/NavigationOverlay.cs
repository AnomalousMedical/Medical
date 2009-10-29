using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;
using Engine.Platform;
using Engine;
using Logging;

namespace Medical
{
    enum NavigationEvents
    {
        ClickButton,
        Left,
        Right,
        Up,
        Down,
        None,
        ZoomIn,
        ZoomOut,
        ChangeFocus,
    }

    class NavigationOverlay : IDisposable, UpdateListener
    {
        private const int BUTTON_WIDTH = 30;
        private const int BUTTON_HEIGHT = 30;
        private const int BUTTON_HALF_WIDTH = BUTTON_WIDTH / 2;
        private const int BUTTON_HALF_HEIGHT = BUTTON_HEIGHT / 2;

        private Overlay mainOverlay;
        private List<NavigationButton> buttons = new List<NavigationButton>();
        private bool showOverlay = false;
        private EventManager eventManager;
        private NavigationButton currentButton = null;
        private NavigationController navigationController;
        private String name;
        private DrawingWindow window;
        private Vector3 lastLookAt = Vector3.Zero;
        private Vector3 lastCameraPos = Vector3.Zero;
        private int lastWindowWidth = 0;
        private int lastWindowHeight = 0;
        private NavigationState currentState;

        static NavigationOverlay()
        {
            MessageEvent clickButton = new MessageEvent(NavigationEvents.ClickButton);
            clickButton.addButton(MouseButtonCode.MB_BUTTON0);
            DefaultEvents.registerDefaultEvent(clickButton);

            MessageEvent upButton = new MessageEvent(NavigationEvents.Up);
            upButton.addButton(KeyboardButtonCode.KC_UP);
            DefaultEvents.registerDefaultEvent(upButton);

            MessageEvent downButton = new MessageEvent(NavigationEvents.Down);
            downButton.addButton(KeyboardButtonCode.KC_DOWN);
            DefaultEvents.registerDefaultEvent(downButton);

            MessageEvent leftButton = new MessageEvent(NavigationEvents.Left);
            leftButton.addButton(KeyboardButtonCode.KC_LEFT);
            DefaultEvents.registerDefaultEvent(leftButton);

            MessageEvent rightButton = new MessageEvent(NavigationEvents.Right);
            rightButton.addButton(KeyboardButtonCode.KC_RIGHT);
            DefaultEvents.registerDefaultEvent(rightButton);

            MessageEvent zoomIn = new MessageEvent(NavigationEvents.ZoomIn);
            zoomIn.addButton(KeyboardButtonCode.KC_EQUALS);
            DefaultEvents.registerDefaultEvent(zoomIn);

            MessageEvent zoomOut = new MessageEvent(NavigationEvents.ZoomOut);
            zoomOut.addButton(KeyboardButtonCode.KC_MINUS);
            DefaultEvents.registerDefaultEvent(zoomOut);

            MessageEvent changeFocus = new MessageEvent(NavigationEvents.ChangeFocus);
            changeFocus.addButton(KeyboardButtonCode.KC_TAB);
            DefaultEvents.registerDefaultEvent(changeFocus);
        }

        static bool resourcesLoaded = false;

        static void loadResources()
        {
            if (!resourcesLoaded)
            {
                OgreResourceGroupManager.getInstance().addResourceLocation(Engine.Resources.Resource.ResourceRoot + "/GUI", "EngineArchive", "Embedded", false);
                OgreResourceGroupManager.getInstance().initializeAllResourceGroups();
                resourcesLoaded = true;
            }
        }

        public static NavigationButton CreateRightButton(String name)
        {
            return new NavigationButton(name, NavigationEvents.Right, "NavigationArrow", new OverlayRect(0, 0, BUTTON_WIDTH, BUTTON_HEIGHT), new OverlayRect(0f, 0.6f, .2f, 0.8f), new OverlayRect(.2f, 0.6f, .4f, 0.8f), new OverlayRect(.4f, 0.6f, .6f, 0.8f));
        }

        public static NavigationButton CreateLeftButton(String name)
        {
            return new NavigationButton(name, NavigationEvents.Left, "NavigationArrow", new OverlayRect(0, 0, BUTTON_WIDTH, BUTTON_HEIGHT), new OverlayRect(0f, 0.4f, .2f, 0.6f), new OverlayRect(.2f, 0.4f, .4f, 0.6f), new OverlayRect(.4f, 0.4f, .6f, 0.6f));
        }

        public static NavigationButton CreateUpButton(String name)
        {
            return new NavigationButton(name, NavigationEvents.Up, "NavigationArrow", new OverlayRect(0, 0, BUTTON_WIDTH, BUTTON_HEIGHT), new OverlayRect(0f, 0.0f, .2f, 0.2f), new OverlayRect(.2f, 0.0f, .4f, 0.2f), new OverlayRect(.4f, 0.0f, .6f, 0.2f));
        }

        public static NavigationButton CreateDownButton(String name)
        {
            return new NavigationButton(name, NavigationEvents.Down, "NavigationArrow", new OverlayRect(0, 0, BUTTON_WIDTH, BUTTON_HEIGHT), new OverlayRect(0f, 0.2f, .2f, 0.4f), new OverlayRect(.2f, 0.2f, .4f, 0.4f), new OverlayRect(.4f, 0.2f, .6f, 0.4f));
        }

        public static NavigationButton CreateZoomInButton(String name)
        {
            return new NavigationButton(name, NavigationEvents.ZoomIn, "NavigationArrow", new OverlayRect(0, 0, BUTTON_WIDTH, BUTTON_HEIGHT), new OverlayRect(.6f, 0.0f, .8f, 0.2f), new OverlayRect(.6f, 0.2f, .8f, 0.4f), new OverlayRect(.6f, 0.4f, .8f, 0.6f));
        }

        public static NavigationButton CreateZoomOutButton(String name)
        {
            return new NavigationButton(name, NavigationEvents.ZoomOut, "NavigationArrow", new OverlayRect(0, 0, BUTTON_WIDTH, BUTTON_HEIGHT), new OverlayRect(0f, 0.8f, .2f, 1.0f), new OverlayRect(.2f, 0.8f, .4f, 1.0f), new OverlayRect(.4f, 0.8f, .6f, 1.0f));
        }

        public NavigationOverlay(String name, DrawingWindow window, NavigationController navigationController)
        {
            loadResources();
            this.name = name;
            this.eventManager = navigationController.EventManager;
            this.navigationController = navigationController;
            this.window = window;
            window.CameraCreated += new DrawingWindowEvent(window_CameraCreated);
            window.CameraDestroyed += new DrawingWindowEvent(window_CameraDestroyed);
            window.PreFindVisibleObjects += new OgrePlugin.OgreCameraCallback(window_PreFindVisibleObjects);

            mainOverlay = OverlayManager.getInstance().create(name + "_NavigationOverlay");
        }

        public void Dispose()
        {
            foreach (NavigationButton button in buttons)
            {
                button.Dispose();
            }
            OverlayManager.getInstance().destroy(mainOverlay);
        }

        public void setNavigationState(NavigationState state)
        {
            currentState = state;
            foreach (NavigationButton button in buttons)
            {
                mainOverlay.remove2d(button.PanelElement);
                button.Dispose();
            }
            buttons.Clear();
            foreach (NavigationLink link in state.AdjacentStates)
            {
                NavigationState adjacent = link.Destination;
                NavigationButton navButton = null;
                switch(link.Button)
                {
                    case NavigationButtons.Down:
                        navButton = CreateDownButton(name + "_Navigation_" + adjacent.Name);
                        break;
                    case NavigationButtons.Up:
                        navButton = CreateUpButton(name + "_Navigation_" + adjacent.Name);
                        break;
                    case NavigationButtons.Right:
                        navButton = CreateRightButton(name + "_Navigation_" + adjacent.Name);
                        break;
                    case NavigationButtons.Left:
                        navButton = CreateLeftButton(name + "_Navigation_" + adjacent.Name);
                        break;
                    case NavigationButtons.ZoomIn:
                        navButton = CreateZoomInButton(name + "_Navigation_" + adjacent.Name);
                        break;
                    case NavigationButtons.ZoomOut:
                        navButton = CreateZoomOutButton(name + "_Navigation_" + adjacent.Name);
                        break;
                }
                 
                navButton.Clicked += new NavigationButtonClicked(navButton_Clicked);
                navButton.Link = link;
                mainOverlay.add2d(navButton.PanelElement);
                buttons.Add(navButton);
            }
            //Force the buttons to update
            lastCameraPos = Vector3.Zero;
            window.SubText = state.Name;
            //Log.Debug("Current state is {0}.", state.Name);
        }

        public NavigationState getNavigationState()
        {
            return currentState;
        }

        /// <summary>
        /// Allow the overlay to render when it is being shown by its camera.
        /// </summary>
        public bool ShowOverlay
        {
            get
            {
                return showOverlay;
            }
            set
            {
                showOverlay = value;
                //hide the overlay if it is visible.
                if (!showOverlay && mainOverlay.isVisible())
                {
                    mainOverlay.hide();
                }
            }
        }

        /// <summary>
        /// Called by the camera when it is rendering.
        /// </summary>
        /// <param name="visible"></param>
        internal void setVisible(bool visible)
        {
            //Ensure that the buttons are in the correct positions.
            if (visible && ShowOverlay && (window.LookAt != lastLookAt || window.Translation != lastCameraPos || window.RenderWidth != lastWindowWidth || window.RenderHeight != lastWindowHeight))
            {
                foreach (NavigationButton button in buttons)
                {
                    Vector3 screenPos = window.getScreenPosition(button.Link.Destination.LookAt + (button.Link.Destination.Translation - button.Link.Destination.LookAt).normalized() * button.Link.VisualRadius);
                    screenPos.x -= BUTTON_HALF_WIDTH;
                    screenPos.y -= BUTTON_HALF_HEIGHT;
                    if (screenPos.x < 0)
                    {
                        screenPos.x = 0;
                    }
                    else if (screenPos.x > window.getMouseAreaWidth() - BUTTON_WIDTH)
                    {
                        screenPos.x = window.RenderWidth - BUTTON_WIDTH;
                    }
                    if (screenPos.y < 0)
                    {
                        screenPos.y = 0;
                    }
                    else if (screenPos.y > window.RenderHeight - BUTTON_HEIGHT)
                    {
                        screenPos.y = window.getMouseAreaHeight() - BUTTON_HEIGHT;
                    }
                    button.BoundsRect = new OverlayRect(screenPos.x, screenPos.y, button.BoundsRect.X1, button.BoundsRect.Y1);
                }
                lastLookAt = window.LookAt;
                lastCameraPos = window.Translation;
                lastWindowWidth = window.RenderWidth;
                lastWindowHeight = window.RenderHeight;
            }

            if (showOverlay && visible && !mainOverlay.isVisible())
            {
                mainOverlay.show();
            }
            else if (showOverlay && !visible && mainOverlay.isVisible())
            {
                mainOverlay.hide();
            }
        }

        void navButton_Clicked(NavigationButton source)
        {
            window.setNewPosition(source.Link.Destination.Translation, source.Link.Destination.LookAt);
            setNavigationState(source.Link.Destination);
        }

        void window_CameraCreated(DrawingWindow window)
        {
            navigationController.Timer.addFixedUpdateListener(this);
        }

        void window_CameraDestroyed(DrawingWindow window)
        {
            navigationController.Timer.removeFixedUpdateListener(this);
        }

        void window_PreFindVisibleObjects(bool callingCameraRender)
        {
            Vector3 mousePos = eventManager.Mouse.getAbsMouse();
            this.setVisible(callingCameraRender && window.allowMotion((int)mousePos.x, (int)mousePos.y));
        }

        #region UpdateListener Members

        public void exceededMaxDelta()
        {
            
        }

        public void loopStarting()
        {
            
        }

        public void sendUpdate(Clock clock)
        {
            if (showOverlay)
            {
                bool firstFrame = eventManager[NavigationEvents.ClickButton].FirstFrameDown;
                if (firstFrame)
                {
                    currentButton = null;
                }
                bool mouseClicked = eventManager[NavigationEvents.ClickButton].Down || firstFrame || eventManager[NavigationEvents.ClickButton].FirstFrameUp;
                Vector3 mouseCoords = eventManager.Mouse.getAbsMouse();
                if (window.allowMotion((int)mouseCoords.x, (int)mouseCoords.y))
                {
                    window.getLocalCoords(ref mouseCoords.x, ref mouseCoords.y);
                    foreach (NavigationButton button in buttons)
                    {
                        if (button.process(mouseCoords, mouseClicked, window.getMouseAreaWidth(), window.getMouseAreaHeight()))
                        {
                            if (eventManager[NavigationEvents.ClickButton].FirstFrameDown)
                            {
                                currentButton = button;
                                //Log.Debug("Clicking {0}.", button.Link.Destination.Name);
                            }
                            else if (eventManager[NavigationEvents.ClickButton].FirstFrameUp && currentButton == button)
                            {
                                button.fireClickEvent();
                                currentButton = null;
                            }
                            break;
                        }
                        else if (window.Focused && button.ShortcutEvent != NavigationEvents.None && (currentButton == null || button == currentButton))
                        {
                            if (currentButton == null && eventManager[button.ShortcutEvent].Down)
                            {
                                currentButton = button;
                                button.setPressedLook();
                                break;
                            }
                            else if (eventManager[NavigationEvents.ChangeFocus].FirstFrameUp)
                            {
                                currentButton = null;
                            }
                            else if (button == currentButton)
                            {
                                if (eventManager[button.ShortcutEvent].Down)
                                {
                                    button.setPressedLook();
                                    break;
                                }
                                else if (eventManager[button.ShortcutEvent].FirstFrameUp)
                                {
                                    button.setNormalLook();
                                    button.fireClickEvent();
                                    currentButton = null;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
