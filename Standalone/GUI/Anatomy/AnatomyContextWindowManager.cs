using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Medical.Controller;

namespace Medical.GUI
{
    public class AnatomyContextWindowManager
    {
        private AnatomyContextWindow currentAnatomyWindow;
        private SceneViewController sceneViewController;

        private LayerState beforeFocusLayerState = null;
        private AnatomyContextWindow lastHighlightRequestWindow = null;

        public AnatomyContextWindowManager(SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
        }

        public AnatomyContextWindow showWindow(Anatomy anatomy)
        {
            if (currentAnatomyWindow == null)
            {
                currentAnatomyWindow = new AnatomyContextWindow(this);
                currentAnatomyWindow.SmoothShow = true;
            }
            currentAnatomyWindow.Visible = true;
            currentAnatomyWindow.Anatomy = anatomy;
            beforeFocusLayerState = null;
            return currentAnatomyWindow;
        }

        public void closeUnpinnedWindow()
        {
            if (currentAnatomyWindow != null)
            {
                currentAnatomyWindow.Visible = false;
            }
        }

        internal void alertWindowPinned()
        {
            currentAnatomyWindow = null;
        }

        internal void centerAnatomy(AnatomyContextWindow requestingWindow)
        {
            SceneViewWindow window = sceneViewController.ActiveWindow;
            Vector3 center = requestingWindow.Anatomy.Center;
            Vector3 translation = center;

            float nearPlane = window.Camera.getNearClipDistance();
            float theta = window.Camera.getFOVy() * 0.0174532925f;
            float aspectRatio = window.Camera.getAspectRatio();
            if (aspectRatio < 1.0f)
            {
                theta *= aspectRatio;
            }

            translation.z += requestingWindow.Anatomy.BoundingRadius / (float)Math.Tan(theta) - nearPlane;

            window.setPosition(translation, center, MedicalConfig.CameraTransitionTime);
        }

        internal void highlightAnatomy(AnatomyContextWindow requestingWindow)
        {
            if (requestingWindow == lastHighlightRequestWindow && beforeFocusLayerState != null)
            {
                beforeFocusLayerState.apply();
                beforeFocusLayerState.Dispose();
                beforeFocusLayerState = null;
                lastHighlightRequestWindow = null;
            }
            else
            {
                if (beforeFocusLayerState == null)
                {
                    beforeFocusLayerState = new LayerState("");
                    beforeFocusLayerState.captureState();
                }

                TransparencyController.smoothSetAllAlphas(0.0f, MedicalConfig.TransparencyChangeMultiplier);
                requestingWindow.Anatomy.TransparencyChanger.smoothBlend(1.0f, MedicalConfig.TransparencyChangeMultiplier);
                lastHighlightRequestWindow = requestingWindow;
                centerAnatomy(requestingWindow);
            }
        }
    }
}
