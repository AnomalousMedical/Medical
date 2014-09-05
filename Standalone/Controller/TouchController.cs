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

        public TouchController(NativeOSWindow window, UpdateTimer mainTimer, SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
            this.mainTimer = mainTimer;
            multiTouch = new MultiTouch(window);
            gestureEngine = new GestureEngine(multiTouch);
            mainTimer.addUpdateListener(gestureEngine);

            gestureEngine.addGesture(PlatformConfig.createGuiGesture());

            zoomGesture = PlatformConfig.createZoomGesture();
            zoomGesture.Zoom += new TwoFingerZoom.ZoomDelegate(zoomGesture_Zoom);
            gestureEngine.addGesture(zoomGesture);

            rotateGesture = PlatformConfig.createRotateGesture();
            rotateGesture.Scroll += new MultiFingerScrollGesture.ScrollDelegate(rotateGesture_Scroll);
            gestureEngine.addGesture(rotateGesture);

            panGesture = PlatformConfig.createPanGesture();
            panGesture.Scroll += new MultiFingerScrollGesture.ScrollDelegate(panGesture_Scroll);
            gestureEngine.addGesture(panGesture);
        }

        public void Dispose()
        {
            mainTimer.removeUpdateListener(gestureEngine);
            multiTouch.Dispose();
        }

        void rotateGesture_Scroll(float deltaX, float deltaY)
        {
            SceneViewWindow sceneView = sceneViewController.ActiveWindow;
            if (sceneView != null)
            {
                float sensitivity = 4.0f;
                sceneView.CameraMover.rotate(-deltaX * sensitivity, deltaY * sensitivity);
                sceneView.stopMaintainingIncludePoint();
            }
        }

        void panGesture_Scroll(float deltaX, float deltaY)
        {
            SceneViewWindow sceneView = sceneViewController.ActiveWindow;
            if (sceneView != null)
            {
                float sensitivity = 15.0f;
                sceneView.CameraMover.pan(deltaX * sensitivity, deltaY * sensitivity);
                sceneView.stopMaintainingIncludePoint();
            }
        }

        void zoomGesture_Zoom(float zoomDelta)
        {
            SceneViewWindow sceneView = sceneViewController.ActiveWindow;
            if (sceneView != null)
            {
                float sensitivity = 80.0f;
                sceneView.CameraMover.zoom(zoomDelta * sensitivity);
                sceneView.stopMaintainingIncludePoint();
            }
        }
    }
}
