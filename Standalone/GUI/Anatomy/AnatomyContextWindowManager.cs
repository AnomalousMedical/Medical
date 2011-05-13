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
        private AnatomyController anatomyController;

        public AnatomyContextWindowManager(SceneViewController sceneViewController, AnatomyController anatomyController)
        {
            this.sceneViewController = sceneViewController;
            this.anatomyController = anatomyController;
        }

        public AnatomyContextWindow showWindow(Anatomy anatomy, int left, int top)
        {
            if (currentAnatomyWindow == null)
            {
                currentAnatomyWindow = new AnatomyContextWindow(this);
                currentAnatomyWindow.SmoothShow = true;
            }
            currentAnatomyWindow.show(left, top);
            currentAnatomyWindow.Anatomy = anatomy;
            beforeFocusLayerState = null;
            return currentAnatomyWindow;
        }

        public void closeUnpinnedWindow()
        {
            if (currentAnatomyWindow != null)
            {
                currentAnatomyWindow.hide();
            }
        }

        internal void alertWindowPinned()
        {
            currentAnatomyWindow = null;
        }

        internal String getThumbnail(Anatomy anatomy)
        {
            float fovy = sceneViewController.ActiveWindow.Camera.getFOVy() * 0.0174532925f;
            return anatomyController.getThumbnail(anatomy, fovy);
        }

        internal void centerAnatomy(AnatomyContextWindow requestingWindow)
        {
            AxisAlignedBox boundingBox = requestingWindow.Anatomy.WorldBoundingBox;
            SceneViewWindow window = sceneViewController.ActiveWindow;
            Vector3 center = boundingBox.Center;
            
            window.setPosition(window.Translation, center, MedicalConfig.CameraTransitionTime);
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

                AxisAlignedBox boundingBox = requestingWindow.Anatomy.WorldBoundingBox;
                SceneViewWindow window = sceneViewController.ActiveWindow;
                Vector3 center = boundingBox.Center;

                float nearPlane = window.Camera.getNearClipDistance();
                float theta = window.Camera.getFOVy() * 0.0174532925f;
                float aspectRatio = window.Camera.getAspectRatio();
                if (aspectRatio < 1.0f)
                {
                    theta *= aspectRatio;
                }

                Vector3 translation = center;
                Vector3 direction = (window.Translation - window.LookAt).normalized();
                translation += direction * boundingBox.DiagonalDistance / (float)Math.Tan(theta);

                window.setPosition(translation, center, MedicalConfig.CameraTransitionTime);
            }
        }
    }
}
