using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Logging;
using Engine.Platform;
using Engine.Resources;
using System.Xml;
using Medical.Controller;
using MyGUIPlugin;

namespace Medical.Controller
{
    delegate void NavigationControllerEvent(NavigationController controller);

    class NavigationController : IDisposable
    {
        static MessageEvent toggleNavigation;

        enum NavigationEvents
        {
            ToggleNavigation,
        }

        static NavigationController()
        {
            toggleNavigation = new MessageEvent(NavigationEvents.ToggleNavigation);
            toggleNavigation.addButton(KeyboardButtonCode.KC_SPACE);
            DefaultEvents.registerDefaultEvent(toggleNavigation);
        }

        public event NavigationControllerEvent NavigationStateSetChanged;
        public event NavigationControllerEvent ShowOverlaysChanged;

        private SceneViewController windowController;
        private EventManager eventManager;
        private UpdateTimer timer;
        private Dictionary<SceneViewWindow, NavigationOverlay> overlays = new Dictionary<SceneViewWindow, NavigationOverlay>();
        private bool showOverlays = false;
        private NavigationStateSet navigationSet;
        private String currentCameraFile;

        public NavigationController(SceneViewController windowController, EventManager eventManager, UpdateTimer timer)
        {
            this.eventManager = eventManager;
            this.timer = timer;
            this.windowController = windowController;
            windowController.WindowCreated += windowController_WindowCreated;
            windowController.WindowDestroyed += windowController_WindowDestroyed;

            toggleNavigation.FirstFrameUpEvent += new MessageEventCallback(toggleNavigation_FirstFrameUpEvent);
        }

        public void Dispose()
        {
            if (navigationSet != null)
            {
                navigationSet.Dispose();
            }
        }

        public bool loadNavigationSetIfDifferent(String cameraFile)
        {
            if (cameraFile != currentCameraFile)
            {
                loadNavigationSet(cameraFile);
                return true;
            }
            return false;
        }

        public void loadNavigationSet(String cameraFile)
        {
            currentCameraFile = cameraFile;
            NavigationSet = loadNavSet(cameraFile);
        }

        public void mergeNavigationSet(String cameraFile)
        {
            using (NavigationStateSet navSet = loadNavSet(cameraFile))
            {
                if (navSet != null)
                {
                    if (NavigationSet == null)
                    {
                        currentCameraFile = cameraFile;
                        NavigationSet = navSet;
                    }
                    else
                    {
                        NavigationSet.mergeStates(navSet);
                    }
                }
            }
        }

        private NavigationStateSet loadNavSet(String cameraFile)
        {
            VirtualFileSystem archive = VirtualFileSystem.Instance;
            if (archive.exists(cameraFile))
            {
                try
                {
                    using (XmlTextReader textReader = new XmlTextReader(archive.openStream(cameraFile, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read)))
                    {
                        return NavigationSerializer.readNavigationStateSet(textReader);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error loading navigation file.\n{0}", ex.Message);
                }
            }
            return null;
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

        public void setNavigationState(String name, SceneViewWindow window)
        {
            NavigationState state = getState(name);
            if (state != null)
            {
                window.setPosition(state.Translation, state.LookAt);
                overlays[window].setNavigationState(state);
            }
        }

        public NavigationState getNavigationState(SceneViewWindow window)
        {
            return overlays[window].getNavigationState();
        }

        public NavigationState findClosestNonHiddenState(Vector3 position)
        {
            if (navigationSet != null)
            {
                return navigationSet.findClosestNonHiddenState(position);
            }
            return null;
        }

        /// <summary>
        /// Figure out what the closest state to all cameras is.
        /// </summary>
        public void recalculateClosestNonHiddenStates()
        {
            if (navigationSet != null)
            {
                foreach (SceneViewWindow window in overlays.Keys)
                {
                    overlays[window].setNavigationState(findClosestNonHiddenState(window.Translation));
                }
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

        void windowController_WindowCreated(SceneViewWindow window)
        {
            if (window.AllowNavigation)
            {
                NavigationOverlay overlay = new NavigationOverlay(window.Name, window, this);
                overlay.ShowOverlay = showOverlays;
                NavigationState closestState = findClosestNonHiddenState(window.Translation);
                if (closestState != null)
                {
                    overlay.setNavigationState(closestState);
                }
                overlays.Add(window, overlay);
            }
        }

        void windowController_WindowDestroyed(SceneViewWindow window)
        {
            NavigationOverlay overlay;
            overlays.TryGetValue(window, out overlay);
            if (overlay != null)
            {
                overlays.Remove(window);
                overlay.Dispose();
            }
        }

        void toggleNavigation_FirstFrameUpEvent()
        {
            if (!Gui.Instance.HandledKeyboardButtons)
            {
                ShowOverlays = !ShowOverlays;
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
                if (showOverlays != value)
                {
                    showOverlays = value;
                    foreach (NavigationOverlay overlay in overlays.Values)
                    {
                        overlay.ShowOverlay = showOverlays;
                    }
                    if (ShowOverlaysChanged != null)
                    {
                        ShowOverlaysChanged.Invoke(this);
                    }
                }
            }
        }
    }
}
