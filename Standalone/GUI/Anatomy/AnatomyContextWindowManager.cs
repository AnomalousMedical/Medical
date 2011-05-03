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
        private AnatomyContextWindow lastFocusRequestWindow = null;

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

        internal void focusOnAnatomy(AnatomyContextWindow requestingWindow)
        {
            if (requestingWindow == lastFocusRequestWindow && beforeFocusLayerState != null)
            {
                beforeFocusLayerState.apply();
                beforeFocusLayerState.Dispose();
                beforeFocusLayerState = null;
                lastFocusRequestWindow = null;
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
                SceneViewWindow window = sceneViewController.ActiveWindow;
                window.setPosition(window.Translation, requestingWindow.Anatomy.Center, MedicalConfig.CameraTransitionTime);
                lastFocusRequestWindow = requestingWindow;
            }
        }
    }
}
