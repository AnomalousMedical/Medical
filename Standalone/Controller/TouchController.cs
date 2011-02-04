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

        public TouchController(OSWindow window, UpdateTimer mainTimer, SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
            this.mainTimer = mainTimer;
            multiTouch = new MultiTouch(window);
            gestureEngine = new GestureEngine(multiTouch);
            mainTimer.addFixedUpdateListener(gestureEngine);

            gestureEngine.addGesture(PlatformConfig.createGuiGesture());

            rotateGesture = PlatformConfig.createRotateGesture();
            rotateGesture.Scroll += new MultiFingerScrollGesture.ScrollDelegate(rotateGesture_Scroll);
            gestureEngine.addGesture(rotateGesture);

            zoomGesture = PlatformConfig.createZoomGesture();
            zoomGesture.Zoom += new TwoFingerZoom.ZoomDelegate(zoomGesture_Zoom);
            gestureEngine.addGesture(zoomGesture);

            panGesture = PlatformConfig.createPanGesture();
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
            SceneViewWindow sceneView = sceneViewController.ActiveWindow;
            if (sceneView != null)
            {
                float sensitivity = 10.0f;
                sceneView.rotate(-deltaX * sensitivity, deltaY * sensitivity);
            }
        }

        void panGesture_Scroll(float deltaX, float deltaY)
        {
            SceneViewWindow sceneView = sceneViewController.ActiveWindow;
            if (sceneView != null)
            {
                float sensitivity = 50.0f;
                sceneView.pan(deltaX * sensitivity, deltaY * sensitivity);
            }
        }

        void zoomGesture_Zoom(float zoomDelta)
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
