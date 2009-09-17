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
    public class NavigationController
    {
        private Dictionary<String, NavigationState> navigationStates = new Dictionary<String, NavigationState>();
        private DrawingWindowController windowController;
        private EventManager eventManager;
        private UpdateTimer timer;
        private Dictionary<DrawingWindow, NavigationOverlay> overlays = new Dictionary<DrawingWindow,NavigationOverlay>();
        private bool showOverlays = false;

        public NavigationController(DrawingWindowController windowController, EventManager eventManager, UpdateTimer timer)
        {
            this.eventManager = eventManager;
            this.timer = timer;
            this.windowController = windowController;
            windowController.WindowCreated += windowController_WindowCreated;
            windowController.WindowDestroyed += windowController_WindowDestroyed;
        }

        public void addState(NavigationState state)
        {
            navigationStates.Add(state.Name, state);
        }

        public void removeState(NavigationState state)
        {
            navigationStates.Remove(state.Name);
        }

        public void clearStates()
        {
            navigationStates.Clear();
        }

        public NavigationState getState(String name)
        {
            NavigationState state;
            navigationStates.TryGetValue(name, out state);
            if (state == null)
            {
                Log.Warning("Could not find Navigation State \"{0}\".", name);
            }
            return state;
        }

        public NavigationState findClosestState(Vector3 position)
        {
            NavigationState closest = null;
            float closestDistanceSq = float.MaxValue;
            foreach (NavigationState state in navigationStates.Values)
            {
                float distanceSq = (position - state.Translation).length2();
                if (distanceSq < closestDistanceSq)
                {
                    closestDistanceSq = distanceSq;
                    closest = state;
                }
            }
            return closest;
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
