using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Logging;
using Engine.Platform;
using System.Drawing;
using Medical.Properties;

namespace Medical
{
    public delegate void NavigationControllerEvent(NavigationController controller);

    public class NavigationController
    {
        public event NavigationControllerEvent NavigationStateSetChanged;

        private DrawingWindowController windowController;
        private EventManager eventManager;
        private UpdateTimer timer;
        private Dictionary<DrawingWindow, NavigationOverlay> overlays = new Dictionary<DrawingWindow,NavigationOverlay>();
        private bool showOverlays = false;
        private NavigationStateSet navigationSet;

        public NavigationController(DrawingWindowController windowController, EventManager eventManager, UpdateTimer timer)
        {
            this.eventManager = eventManager;
            this.timer = timer;
            this.windowController = windowController;
            windowController.WindowCreated += windowController_WindowCreated;
            windowController.WindowDestroyed += windowController_WindowDestroyed;
        }

        public NavigationState getState(String name)
        {
            if (navigationSet != null)
            {
                return navigationSet.getState(name);
            }
            return null;
        }

        public void setNavigationState(String name, DrawingWindow window)
        {
            NavigationState state = getState(name);
            if (state != null)
            {
                window.setCamera(state.Translation, state.LookAt);
                overlays[window].setNavigationState(state);
            }
        }

        public NavigationState findClosestState(Vector3 position)
        {
            if (navigationSet != null)
            {
                return navigationSet.findClosestState(position);
            }
            return null;
        }

        public EventManager EventManager
        {
            get
            {
                return eventManager;
            }
        }

        public UpdateTimer Timer
        {
            get
            {
                return timer;
            }
        }

        public NavigationStateSet NavigationSet
        {
            get
            {
                return navigationSet;
            }
            set
            {
                if (navigationSet != value)
                {
                    navigationSet = value;
                    if (NavigationStateSetChanged != null)
                    {
                        NavigationStateSetChanged.Invoke(this);
                    }
                }
            }
        }

        void windowController_WindowCreated(DrawingWindow window)
        {
            NavigationOverlay overlay = new NavigationOverlay(window.CameraName, window, this);
            overlay.ShowOverlay = showOverlays;
            overlay.setNavigationState(findClosestState(window.Translation));
            overlays.Add(window, overlay);
        }

        void windowController_WindowDestroyed(DrawingWindow window)
        {
            NavigationOverlay overlay = overlays[window];
            overlays.Remove(window);
            overlay.Dispose();
        }

        public bool ShowOverlays
        {
            get
            {
                return showOverlays;
            }
            set
            {
                showOverlays = value;
                foreach (NavigationOverlay overlay in overlays.Values)
                {
                    overlay.ShowOverlay = showOverlays;
                }
            }
        }
    }
}
