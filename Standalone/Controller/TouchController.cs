using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical.Controller;
using MyGUIPlugin;

namespace Medical
{
    class TouchController : IDisposable
    {
        private MultiTouch multiTouch;
        private GestureEngine gestureEngine;
        private UpdateTimer mainTimer;
        private SceneViewController sceneViewController;
        private MultiFingerScrollGesture rotateGesture;
        private MultiFingerScrollGesture panGesture;
        private TwoFingerZoom zoomGesture;

#if WINDOWS
        private const int ROTATE_FINGER_COUNT = 1;
        private const float ROTATE_DECEL_TIME = 0.5f;
        private const float ROTATE_MIN_MOMENTUM = 0.01f;

        private const float ZOOM_DECEL_TIME = 1.0f;
        private const float ZOOM_MIN_MOMENTUM = 0.01f;

        private const int PAN_FINGER_COUNT = 2;
        private const float PAN_DECEL_TIME = 0.5f;
        private const float PAN_MIN_MOMENTUM = 0.01f;
#elif MAC_OSX
        private const int ROTATE_FINGER_COUNT = 2;
        private const float ROTATE_DECEL_TIME = 0.5f;
        private const float ROTATE_MIN_MOMENTUM = 0.01f;

        private const float ZOOM_DECEL_TIME = 0.5f;
        private const float ZOOM_MIN_MOMENTUM = 0.01f;

        private const int PAN_FINGER_COUNT = 3;
        private const float PAN_DECEL_TIME = 0.5f;
        private const float PAN_MIN_MOMENTUM = 0.01f;
#endif

        public TouchController(OSWindow window, UpdateTimer mainTimer, SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
            this.mainTimer = mainTimer;
            multiTouch = new MultiTouch(window);
            gestureEngine = new GestureEngine(multiTouch);
            mainTimer.addFixedUpdateListener(gestureEngine);

            rotateGesture = new MultiFingerScrollGesture(ROTATE_FINGER_COUNT, ROTATE_DECEL_TIME, ROTATE_MIN_MOMENTUM);
            rotateGesture.Scroll += new MultiFingerScrollGesture.ScrollDelegate(rotateGesture_Scroll);
            gestureEngine.addGesture(rotateGesture);

            zoomGesture = new TwoFingerZoom(ZOOM_DECEL_TIME, ZOOM_MIN_MOMENTUM);
            zoomGesture.Zoom += new TwoFingerZoom.ZoomDelegate(zoomGesture_Zoom);
            gestureEngine.addGesture(zoomGesture);

            panGesture = new MultiFingerScrollGesture(PAN_FINGER_COUNT, PAN_DECEL_TIME, PAN_MIN_MOMENTUM);
            panGesture.Scroll += new MultiFingerScrollGesture.ScrollDelegate(panGesture_Scroll);
            gestureEngine.addGesture(panGesture);
        }

        public void Dispose()
        {
            mainTimer.removeFixedUpdateListener(gestureEngine);
            multiTouch.Dispose();
        }

        void rotateGesture_Scroll(float deltaX, float deltaY)
        {
            if (!Gui.Instance.HandledMouseButtons)
            {
                SceneViewWindow sceneView = sceneViewController.ActiveWindow;
                if (sceneView != null)
                {
                    float sensitivity = 10.0f;
                    sceneView.rotate(-deltaX * sensitivity, deltaY * sensitivity);
                }
            }
        }

        void panGesture_Scroll(float deltaX, float deltaY)
        {
            if (!Gui.Instance.HandledMouseButtons)
            {
                SceneViewWindow sceneView = sceneViewController.ActiveWindow;
                if (sceneView != null)
                {
                    float sensitivity = 50.0f;
                    sceneView.pan(deltaX * sensitivity, deltaY * sensitivity);
                }
            }
        }

        void zoomGesture_Zoom(float zoomDelta)
        {
            if (!Gui.Instance.HandledMouseButtons)
            {
                SceneViewWindow sceneView = sceneViewController.ActiveWindow;
                if (sceneView != null)
                {
                    float sensitivity = 200.0f;
                    sceneView.zoom(zoomDelta * sensitivity);
                }
            }
        }
    }
}
