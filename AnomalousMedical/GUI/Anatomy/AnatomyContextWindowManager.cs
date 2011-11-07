using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Medical.Controller;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class AnatomyContextWindowManager : IDisposable
    {
        private AnatomyContextWindow currentAnatomyWindow;
        private SceneViewController sceneViewController;
        private AnatomyFinder anatomyFinder;

        private LayerState beforeFocusLayerState = null;
        private AnatomyContextWindow lastHighlightRequestWindow = null;
        private AnatomyController anatomyController;
        private List<AnatomyContextWindow> pinnedWindows = new List<AnatomyContextWindow>();

        public AnatomyContextWindowManager(SceneViewController sceneViewController, AnatomyController anatomyController, AnatomyFinder anatomyFinder)
        {
            this.sceneViewController = sceneViewController;
            this.anatomyController = anatomyController;
            this.anatomyFinder = anatomyFinder;
        }

        public void Dispose()
        {
            if (currentAnatomyWindow != null)
            {
                currentAnatomyWindow.Dispose();
                currentAnatomyWindow = null;
            }
        }

        public void sceneUnloading()
        {
            foreach (AnatomyContextWindow window in pinnedWindows)
            {
                window.Dispose();
            }
            pinnedWindows.Clear();
            if (currentAnatomyWindow != null)
            {
                currentAnatomyWindow.Dispose();
                currentAnatomyWindow = null;
            }
        }

        public AnatomyContextWindow showWindow(Anatomy anatomy, int left, int top, int deadzoneLeft, int deadzoneRight, int deadzoneTop, int deadzoneBottom)
        {
            if (currentAnatomyWindow == null)
            {
                currentAnatomyWindow = new AnatomyContextWindow(this);
                currentAnatomyWindow.SmoothShow = true;
            }
            currentAnatomyWindow.show(left, top);
            currentAnatomyWindow.Anatomy = anatomy;
            int windowTop = currentAnatomyWindow.Top;
            int windowBottom = windowTop + currentAnatomyWindow.Height;
            int windowLeft = currentAnatomyWindow.Left;
            int windowWidth = currentAnatomyWindow.Width;
            int windowRight = windowLeft + windowWidth;
            //Check to see if the window is in the dead zone.
            if (((windowTop > deadzoneTop && windowTop < deadzoneBottom) ||
                (windowBottom > deadzoneTop && windowBottom < deadzoneBottom)) &&
                ((windowLeft > deadzoneLeft && windowLeft < deadzoneRight) ||
                (windowRight > deadzoneLeft && windowRight < deadzoneRight)))
            {
                if (windowWidth < Gui.Instance.getViewWidth() - deadzoneRight)
                {
                    currentAnatomyWindow.setPosition(deadzoneRight, currentAnatomyWindow.Top);
                }
                else
                {
                    currentAnatomyWindow.setPosition(deadzoneLeft - windowWidth, currentAnatomyWindow.Top);
                }
            }
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

        public bool ShowPremiumAnatomy
        {
            get
            {
                return anatomyController.ShowPremiumAnatomy;
            }
        }

        internal void showRelatedAnatomy(Anatomy anatomy)
        {
            anatomyFinder.showRelatedAnatomy(anatomy);
        }

        internal void alertWindowPinned(AnatomyContextWindow window)
        {
            currentAnatomyWindow = null;
            pinnedWindows.Add(window);
        }

        internal void alertPinnedWindowClosed(AnatomyContextWindow window)
        {
            pinnedWindows.Remove(window);
        }

        internal String getThumbnail(Anatomy anatomy)
        {
            Radian fovy = sceneViewController.ActiveWindow.Camera.getFOVy();
            String thumbnail;
            anatomyController.getThumbnail(anatomy, fovy, out thumbnail);
            return thumbnail;
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

                TransparencyController.smoothSetAllAlphas(0.0f, MedicalConfig.CameraTransitionTime);
                requestingWindow.Anatomy.TransparencyChanger.smoothBlend(1.0f, MedicalConfig.CameraTransitionTime);
                lastHighlightRequestWindow = requestingWindow;

                AxisAlignedBox boundingBox = requestingWindow.Anatomy.WorldBoundingBox;
                SceneViewWindow window = sceneViewController.ActiveWindow;
                Vector3 center = boundingBox.Center;

                float nearPlane = window.Camera.getNearClipDistance();
                float theta = window.Camera.getFOVy();
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
