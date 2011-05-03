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

        internal void moveActiveSceneView(Vector3 lookAt)
        {
            SceneViewWindow window = sceneViewController.ActiveWindow;
            window.setPosition(window.Translation, lookAt, MedicalConfig.CameraTransitionTime);
        }
    }
}
