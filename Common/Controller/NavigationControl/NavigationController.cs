using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Logging;
using Engine.Platform;
using System.Drawing;
using Medical.Properties;
using Engine.Resources;
using System.Xml;

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
        private String currentCameraFile;

        public NavigationController(DrawingWindowController windowController, EventManager eventManager, UpdateTimer timer)
        {
            this.eventManager = eventManager;
            this.timer = timer;
            this.windowController = windowController;
            windowController.WindowCreated += windowController_WindowCreated;
            windowController.WindowDestroyed += windowController_WindowDestroyed;
        }

        public void loadNavigationSet(String cameraFile)
        {
            if (cameraFile != currentCameraFile)
            {
                currentCameraFile = cameraFile;
                using (Archive archive = FileSystem.OpenArchive(cameraFile))
                {
                    if (archive.exists(cameraFile))
                    {
                        try
                        {
                            using (XmlTextReader textReader = new XmlTextReader(archive.openStream(cameraFile, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read)))
                            {
                                NavigationStateSet navigation = NavigationSerializer.readNavigationStateSet(textReader);
                                this.NavigationSet = navigation;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Debug("Error loading navigation file.\n{0}", ex.Message);
                        }
                    }
                }
            }
        }

        public void saveNavigationSet(String cameraFile)
        {
            using (XmlTextWriter textWriter = new XmlTextWriter(cameraFile, Encoding.Default))
            {
                textWriter.Formatting = Formatting.Indented;
                NavigationSerializer.writeNavigationStateSet(navigationSet, textWriter);
            }
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

        public NavigationState getNavigationState(DrawingWindow window)
        {
            return overlays[window].getNavigationState();
        }

        public NavigationState findClosestState(Vector3 position)
        {
            if (navigationSet != null)
            {
                return navigationSet.findClosestState(position);
            }
            return null;
        }

        /// <summary>
        /// Figure out what the closest state to all cameras is.
        /// </summary>
        public void recalculateClosestStates()
        {
            foreach (DrawingWindow window in overlays.Keys)
            {
                overlays[window].setNavigationState(findClosestState(window.Translation));
            }
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

        public String CurrentCameraFile
        {
            get
            {
                return currentCameraFile;
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
                    if (navigationSet != null)
                    {
                        navigationSet.Dispose();
                    }
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
            if (window.AllowNavigation)
            {
                NavigationOverlay overlay = new NavigationOverlay(window.CameraName, window, this);
                overlay.ShowOverlay = showOverlays;
                NavigationState closestState = findClosestState(window.Translation);
                if (closestState != null)
                {
                    overlay.setNavigationState(closestState);
                }
                overlays.Add(window, overlay);
            }
        }

        void windowController_WindowDestroyed(DrawingWindow window)
        {
            NavigationOverlay overlay;
            overlays.TryGetValue(window, out overlay);
            if (overlay != null)
            {
                overlays.Remove(window);
                overlay.Dispose();
            }
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
