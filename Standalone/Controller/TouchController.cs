using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical.Controller;

namespace Medical
{
    class TouchController : IDisposable
    {
        private MultiTouch multiTouch;
        private GestureEngine gestureEngine;
        private UpdateTimer mainTimer;
        private SceneViewController sceneViewController;
        private MultiFingerScrollGesture oneFingerScroll;
        private MultiFingerScrollGesture twoFingerScroll;
        private TwoFingerZoom twoFingerZoom;

        public TouchController(OSWindow window, UpdateTimer mainTimer, SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
            this.mainTimer = mainTimer;
            multiTouch = new MultiTouch(window);
            gestureEngine = new GestureEngine(multiTouch);
            mainTimer.addFixedUpdateListener(gestureEngine);

            oneFingerScroll = new MultiFingerScrollGesture(1);
            oneFingerScroll.Scroll += new MultiFingerScrollGesture.ScrollDelegate(oneFingerScroll_Scroll);
            gestureEngine.addGesture(oneFingerScroll);

            twoFingerZoom = new TwoFingerZoom();
            twoFingerZoom.Zoom += new TwoFingerZoom.ZoomDelegate(twoFingerZoom_Zoom);
            gestureEngine.addGesture(twoFingerZoom);

            twoFingerScroll = new MultiFingerScrollGesture(2);
            twoFingerScroll.Scroll += new MultiFingerScrollGesture.ScrollDelegate(twoFingerScroll_Scroll);
            gestureEngine.addGesture(twoFingerScroll);
        }

        public void Dispose()
        {
            mainTimer.removeFixedUpdateListener(gestureEngine);
            multiTouch.Dispose();
        }

        void oneFingerScroll_Scroll(float deltaX, float deltaY)
        {
            SceneViewWindow sceneView = sceneViewController.ActiveWindow;
            if (sceneView != null)
            {
                float sensitivity = 10.0f;
                sceneView.rotate(-deltaX * sensitivity, deltaY * sensitivity);
            }
        }

        void twoFingerScroll_Scroll(float deltaX, float deltaY)
        {
            SceneViewWindow sceneView = sceneViewController.ActiveWindow;
            if (sceneView != null)
            {
                float sensitivity = 50.0f;
                sceneView.pan(deltaX * sensitivity, deltaY * sensitivity);
            }
        }

        void twoFingerZoom_Zoom(float zoomDelta)
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
