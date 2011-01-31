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
        private const int PAN_FINGER_COUNT = 2;
#elif MAC_OSX
        private const int ROTATE_FINGER_COUNT = 2;
        private const int PAN_FINGER_COUNT = 3;
#endif

        public TouchController(OSWindow window, UpdateTimer mainTimer, SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
            this.mainTimer = mainTimer;
            multiTouch = new MultiTouch(window);
            gestureEngine = new GestureEngine(multiTouch);
            mainTimer.addFixedUpdateListener(gestureEngine);

            rotateGesture = new MultiFingerScrollGesture(ROTATE_FINGER_COUNT, 0.5f, 0.01f);
            rotateGesture.Scroll += new MultiFingerScrollGesture.ScrollDelegate(rotateGesture_Scroll);
            gestureEngine.addGesture(rotateGesture);

//#if WINDOWS
            zoomGesture = new TwoFingerZoom(1.0f, 0.01f);
            zoomGesture.Zoom += new TwoFingerZoom.ZoomDelegate(zoomGesture_Zoom);
            gestureEngine.addGesture(zoomGesture);
//#endif

            panGesture = new MultiFingerScrollGesture(PAN_FINGER_COUNT, 0.5f, 0.01f);
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
