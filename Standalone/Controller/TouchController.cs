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
        private OneFingerScrollGesture oneFingerScroll;

        public TouchController(OSWindow window, UpdateTimer mainTimer, SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
            this.mainTimer = mainTimer;
            multiTouch = new MultiTouch(window);
            gestureEngine = new GestureEngine(multiTouch);
            mainTimer.addFixedUpdateListener(gestureEngine);

            oneFingerScroll = new OneFingerScrollGesture();
            oneFingerScroll.Scroll += new OneFingerScrollGesture.ScrollDelegate(oneFingerScroll_Scroll);
            gestureEngine.addGesture(oneFingerScroll);
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
    }
}
