﻿using System;
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
        private static readonly int ThumbSize = ScaleHelper.Scaled(66);
        private static readonly int ThumbRenderSize = ThumbSize;

        private AnatomyContextWindow currentAnatomyWindow;
        private SceneViewController sceneViewController;
        private AnatomyFinder anatomyFinder;

        private AnatomyController anatomyController;
        private LayerController layerController;
        private List<AnatomyContextWindow> pinnedWindows = new List<AnatomyContextWindow>();

        private LiveThumbnailController liveThumbnailController;

        public AnatomyContextWindowManager(SceneViewController sceneViewController, AnatomyController anatomyController, LayerController layerController, AnatomyFinder anatomyFinder)
        {
            this.sceneViewController = sceneViewController;
            this.anatomyController = anatomyController;
            this.anatomyController.SelectedAnatomy.SelectedAnatomyChanged += anatomyController_SelectedAnatomyChanged;
            this.anatomyFinder = anatomyFinder;
            this.layerController = layerController;

            liveThumbnailController = new LiveThumbnailController("ContextWindows_", new IntSize2(ThumbRenderSize, ThumbRenderSize), sceneViewController);
            liveThumbnailController.MaxPoolSize = 1;
        }

        public void Dispose()
        {
            if (currentAnatomyWindow != null)
            {
                currentAnatomyWindow.Dispose();
                currentAnatomyWindow = null;
            }
            liveThumbnailController.Dispose();
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

        public AnatomyContextWindow showWindow(Anatomy anatomy, IntVector2 position, IntCoord deadZone)
        {
            if (currentAnatomyWindow == null)
            {
                currentAnatomyWindow = new AnatomyContextWindow(this, layerController);
                currentAnatomyWindow.SmoothShow = true;
            }
            currentAnatomyWindow.Position = new Vector2(position.x, position.y);
            currentAnatomyWindow.Visible = true;
            currentAnatomyWindow.Anatomy = anatomy;
            int windowTop = currentAnatomyWindow.Top;
            int windowBottom = windowTop + currentAnatomyWindow.Height;
            int windowLeft = currentAnatomyWindow.Left;
            int windowWidth = currentAnatomyWindow.Width;
            int windowRight = windowLeft + windowWidth;

            int deadzoneTop = deadZone.top;
            int deadzoneBottom = deadZone.Bottom;
            int deadzoneLeft = deadZone.left;
            int deadzoneRight = deadZone.Right;

            //Check to see if the window is in the dead zone.
            if (((windowTop > deadzoneTop && windowTop < deadzoneBottom) ||
                (windowBottom > deadzoneTop && windowBottom < deadzoneBottom)) &&
                ((windowLeft > deadzoneLeft && windowLeft < deadzoneRight) ||
                (windowRight > deadzoneLeft && windowRight < deadzoneRight)))
            {
                if (windowWidth < RenderManager.Instance.ViewWidth - deadzoneRight)
                {
                    currentAnatomyWindow.Position = new Vector2(deadzoneRight, currentAnatomyWindow.Top);
                }
                else
                {
                    currentAnatomyWindow.Position = new Vector2(deadzoneLeft - windowWidth, currentAnatomyWindow.Top);
                }
            }
            return currentAnatomyWindow;
        }

        public void closeUnpinnedWindow()
        {
            if (currentAnatomyWindow != null)
            {
                currentAnatomyWindow.Visible = false;
            }
        }

        public AnatomyCommandPermissions CommandPermissions
        {
            get
            {
                return anatomyController.CommandPermissions;
            }
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

        internal AnatomyContextWindowLiveThumbHost getThumbnail(AnatomyContextWindow window)
        {
            Anatomy anatomy = window.Anatomy;
            Radian theta = sceneViewController.ActiveWindow.Camera.getFOVy();

            //Generate thumbnail
            AxisAlignedBox boundingBox = anatomy.WorldBoundingBox;
            Vector3 center = boundingBox.Center;

            //PROBABLY DON'T NEED THIS, ASPECT IS A SQUARE
            float aspectRatio = (float)ThumbSize / ThumbSize;
            if (aspectRatio < 1.0f)
            {
                theta *= aspectRatio;
            }

            Vector3 translation = center;
            Vector3 direction = anatomy.PreviewCameraDirection;
            translation += direction * boundingBox.DiagonalDistance / (float)Math.Tan(theta);

            LayerState layers = new LayerState(anatomy.TransparencyNames, 1.0f);

            //Create a new thumb host or update an existing one
            if (window.ThumbHost == null)
            {
                AnatomyContextWindowLiveThumbHost host = new AnatomyContextWindowLiveThumbHost(window)
                {
                    Layers = layers,
                    Translation = translation,
                    LookAt = center
                };
                liveThumbnailController.addThumbnailHost(host);
                liveThumbnailController.setVisibility(host, true);
                return host;                
            }
            else
            {
                window.ThumbHost.Translation = translation;
                window.ThumbHost.LookAt = center;
                window.ThumbHost.Layers = layers;
                liveThumbnailController.updateCameraAndLayers(window.ThumbHost);
                return window.ThumbHost;
            }
        }

        internal void returnThumbnail(AnatomyContextWindow window)
        {
            liveThumbnailController.removeThumbnailHost(window.ThumbHost);
        }

        internal void centerAnatomy(AnatomyContextWindow requestingWindow)
        {
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
            CameraPosition cameraPosition = new CameraPosition()
            {
                Translation = translation,
                LookAt = center
            };

            window.setPosition(cameraPosition, MedicalConfig.CameraTransitionTime);
        }

        internal void showOnly(Anatomy anatomy)
        {
            LayerState currentLayers = LayerState.CreateAndCapture();
            TransparencyController.smoothSetAllAlphas(0.0f, MedicalConfig.CameraTransitionTime, EasingFunction.EaseOutQuadratic);
            anatomy.smoothBlend(1.0f, MedicalConfig.CameraTransitionTime, EasingFunction.EaseOutQuadratic);
            layerController.pushUndoState(currentLayers);
        }

        internal bool isContextWindowAtPoint(int x, int y)
        {
            if (currentAnatomyWindow != null && currentAnatomyWindow.Visible && currentAnatomyWindow.contains(x, y))
            {
                return true;
            }

            foreach(var window in pinnedWindows)
            {
                if(window.contains(x, y))
                {
                    return true;
                }
            }

            return false;
        }

        void anatomyController_SelectedAnatomyChanged(AnatomySelection anatomySelection)
        {
            Anatomy anatomy = anatomySelection.Anatomy;
            if (anatomy != null)
            {
                showWindow(anatomy, anatomyFinder.DisplayHintLocation, anatomyFinder.DeadZone);
            }
            else
            {
                closeUnpinnedWindow();
            }
        }
    }
}
