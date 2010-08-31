using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OgrePlugin;
using OgreWrapper;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Engine;
using Logging;
using Medical.Controller;

namespace Medical
{
    class ImageException : Exception
    {
        public ImageException(String message)
            :base(message)
        {

        }
    }

    class ImageRenderer
    {
        /// <summary>
        /// Called before the image is rendered or any modifications for
        /// rendering of the scene have been made.
        /// </summary>
        public event EventHandler ImageRenderStarted;

        /// <summary>
        /// Called after the image has completed rendering and the scene has
        /// been put back to how it was before the render started.
        /// </summary>
        public event EventHandler ImageRenderCompleted;

        private MedicalController controller;
        private SceneViewController sceneViewController;
        private Watermark watermark;
        private ViewportBackground background;
        private LayerController layerController;
        private NavigationController navigationController;

        public ImageRenderer(MedicalController controller, SceneViewController sceneViewController, LayerController layerController, NavigationController navigationController)
        {
            this.controller = controller;
            this.sceneViewController = sceneViewController;
            this.layerController = layerController;
            this.navigationController = navigationController;
        }

        public Bitmap renderImage(ImageRendererProperties properties)
        {
            Bitmap bitmap = null;
            SceneViewWindow sceneWindow = sceneViewController.ActiveWindow;
            if (sceneWindow != null)
            {
                if (ImageRenderStarted != null)
                {
                    ImageRenderStarted.Invoke(this, EventArgs.Empty);
                }

                //Background color
                Engine.Color backgroundColor = properties.CustomBackgroundColor;
                if (properties.UseWindowBackgroundColor)
                {
                    backgroundColor = sceneWindow.BackColor;
                }

                //Size (with AA)
                int width = properties.Width * properties.AntiAliasingMode;
                int height = properties.Height * properties.AntiAliasingMode;

                //Camera position
                Vector3 cameraPosition = sceneWindow.Translation;
                Vector3 cameraLookAt = sceneWindow.LookAt;
                if (!properties.UseActiveViewportLocation)
                {
                    if (properties.UseNavigationStatePosition)
                    {
                        if (navigationController != null)
                        {
                            NavigationState state = navigationController.NavigationSet.getState(properties.NavigationStateName);
                            if (state != null)
                            {
                                cameraPosition = state.Translation;
                                cameraLookAt = state.LookAt;
                            }
                            else
                            {
                                Log.Error("Could not render image from navigation state \"{0}\" because it could not be found.", properties.NavigationStateName);
                            }
                        }
                    }
                    else if (properties.UseCustomPosition)
                    {
                        cameraPosition = properties.CameraPosition;
                        cameraLookAt = properties.CameraLookAt;
                    }
                }

                //Background Activation
                if (background != null && properties.ShowBackground)
                {
                    background.setVisible(true);
                }

                //Layer override
                LayerState currentLayers = null;
                if (properties.OverrideLayers && layerController != null)
                {
                    currentLayers = layerController.applyLayerStateTemporaryUndisruptive(properties.LayerState);
                }

                //Render
                bitmap = createRender(width, height, properties.NumGridTiles, properties.AntiAliasingMode, properties.ShowWatermark, properties.TransparentBackground, backgroundColor, sceneWindow.Camera, cameraPosition, cameraLookAt);

                //Turn off layer override
                if (properties.OverrideLayers && layerController != null)
                {
                    layerController.restoreConditions(currentLayers);
                }

                //Background deactivation
                if (background != null && properties.ShowBackground)
                {
                    background.setVisible(false);
                }

                //Transparent background
                if (properties.TransparentBackground)
                {
                    bitmap.MakeTransparent(System.Drawing.Color.FromArgb(backgroundColor.toARGB()));
                }

                if (ImageRenderCompleted != null)
                {
                    ImageRenderCompleted.Invoke(this, EventArgs.Empty);
                }
            }

            return bitmap;
        }

        private Bitmap createRender(int width, int height, int gridSize, int aaMode, bool showWatermark, bool transparentBG, Engine.Color backColor, Camera cloneCamera, Vector3 position, Vector3 lookAt)
        {
            OgreSceneManager sceneManager = controller.CurrentScene.getDefaultSubScene().getSimElementManager<OgreSceneManager>();
            if (sceneManager != null)
            {
                using (TexturePtr texture = TextureManager.getInstance().createManual("__PictureTexture", "__InternalMedical", TextureType.TEX_TYPE_2D, (uint)(width / gridSize), (uint)(height / gridSize), 1, 1, OgreWrapper.PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, false, 0))
                {
                    using (HardwarePixelBufferSharedPtr pixelBuffer = texture.Value.getBuffer())
                    {
                        RenderTexture renderTexture = pixelBuffer.Value.getRenderTarget();
                        Camera camera = sceneManager.SceneManager.createCamera("__PictureCamera");
                        camera.setAutoAspectRatio(cloneCamera.getAutoAspectRatio());
                        camera.setLodBias(cloneCamera.getLodBias());
                        camera.setUseRenderingDistance(cloneCamera.getUseRenderingDistance());
                        camera.setNearClipDistance(cloneCamera.getNearClipDistance());
                        camera.setFarClipDistance(cloneCamera.getFarClipDistance());
                        camera.setPolygonMode(cloneCamera.getPolygonMode());
                        camera.setRenderingDistance(cloneCamera.getRenderingDistance());
                        camera.setAspectRatio(cloneCamera.getAspectRatio());
                        camera.setProjectionType(cloneCamera.getProjectionType());
                        camera.setFOVy(cloneCamera.getFOVy());
                        SceneNode node = sceneManager.SceneManager.createSceneNode("__PictureCameraNode");
                        node.attachObject(camera);
                        node.setPosition(position);
                        sceneManager.SceneManager.getRootSceneNode().addChild(node);
                        camera.lookAt(lookAt);
                        Light light = sceneManager.SceneManager.createLight("__PictureCameraLight");
                        node.attachObject(light);
                        Viewport viewport = renderTexture.addViewport(camera);
                        viewport.setBackgroundColor(backColor);

                        //Update background position (if applicable)
                        if (background != null)
                        {
                            background.updatePosition(camera.getRealPosition(), camera.getRealDirection(), camera.getRealOrientation());
                        }

                        Bitmap bitmap = null;
                        if (gridSize <= 1)
                        {
                            bitmap = simpleRender(width, height, aaMode, showWatermark, transparentBG, backColor, renderTexture);
                        }
                        else
                        {
                            bitmap = gridRender(width, height, gridSize, aaMode, renderTexture, camera);
                        }

                        renderTexture.destroyViewport(viewport);
                        sceneManager.SceneManager.getRootSceneNode().removeChild(node);
                        sceneManager.SceneManager.destroyLight(light);
                        sceneManager.SceneManager.destroySceneNode(node);
                        sceneManager.SceneManager.destroyCamera(camera);

                        TextureManager.getInstance().remove(texture);

                        return bitmap;
                    }
                }
            }
            return null;
        }

        private Bitmap simpleRender(int width, int height, int aaMode, bool showWatermark, bool transparentBG, Engine.Color bgColor, RenderTexture renderTexture)
        {
            //Toggle watermark if required.
            bool watermarkStatusChanged = false;
            if (watermark != null && (showWatermark != watermark.Visible))
            {
                watermarkStatusChanged = true;
                watermark.Visible = !watermark.Visible;
            }

            renderTexture.update();
            OgreWrapper.PixelFormat format = OgreWrapper.PixelFormat.PF_A8R8G8B8;
            System.Drawing.Imaging.PixelFormat bitmapFormat = System.Drawing.Imaging.PixelFormat.Format32bppRgb;
            Bitmap bitmap = new Bitmap(width, height, bitmapFormat);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(new Point(), bitmap.Size), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            unsafe
            {
                PixelBox pixelBox = new PixelBox(0, 0, bmpData.Width, bmpData.Height, format, bmpData.Scan0.ToPointer());
                renderTexture.copyContentsToMemory(pixelBox, RenderTarget.FrameBuffer.FB_AUTO);
            }
            bitmap.UnlockBits(bmpData);

            //Resize if aa is active
            if (aaMode > 1)
            {
                if (transparentBG)
                {
                    bitmap.MakeTransparent(System.Drawing.Color.FromArgb(bgColor.toARGB()));
                }
                int smallWidth = width / aaMode;
                int smallHeight = height / aaMode;
                Bitmap largeImage = bitmap;
                bitmap = new Bitmap(smallWidth, smallHeight);
                using (Graphics graph = Graphics.FromImage(bitmap))
                {
                    graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    graph.CompositingQuality = CompositingQuality.HighQuality;
                    graph.SmoothingMode = SmoothingMode.AntiAlias;
                    graph.DrawImage(largeImage, new Rectangle(0, 0, smallWidth, smallHeight));
                }
                largeImage.Dispose();
            }

            //Toggle watermark back
            if (watermarkStatusChanged)
            {
                watermark.Visible = !watermark.Visible;
            }

            return bitmap;
        }

        private Bitmap gridRender(int width, int height, int gridSize, int aaMode, RenderTexture renderTexture, Camera camera)
        {
            bool turnedOffWatermark = false;
            if (watermark != null && watermark.Visible)
            {
                turnedOffWatermark = true;
                watermark.Visible = false;
            }

            float originalLeft, originalRight, originalTop, originalBottom;
            camera.getFrustumExtents(out originalLeft, out originalRight, out originalTop, out originalBottom);

            float gridStepHoriz = (originalRight * 2) / gridSize;
            float gridStepVert = (originalTop * 2) / gridSize;

            int imageStepHoriz = width / gridSize;
            int imageStepVert = height / gridSize;

            int finalWidth = width / aaMode;
            int finalHeight = height / aaMode;
            int imageStepHorizSmall = finalWidth / gridSize;
            int imageStepVertSmall = finalHeight / gridSize;
            //Log.Debug("Step {0}, {1}", imageStepHorizSmall, imageStepVertSmall);

            float left, right, top, bottom;
            int totalSS = gridSize * gridSize;

            OgreWrapper.PixelFormat format = OgreWrapper.PixelFormat.PF_A8R8G8B8;
            System.Drawing.Imaging.PixelFormat bitmapFormat = System.Drawing.Imaging.PixelFormat.Format32bppRgb;

            Rectangle destRect = new Rectangle();
            Rectangle srcRect = new Rectangle(0, 0, imageStepHoriz, imageStepVert);
            Bitmap fullBitmap = new Bitmap(finalWidth, finalHeight, bitmapFormat);
            using (Graphics g = Graphics.FromImage(fullBitmap))
            {
                //g.Clear(System.Drawing.Color.LimeGreen);
                using (Bitmap pieceBitmap = new Bitmap(imageStepHoriz, imageStepVert, bitmapFormat))
                {
                    Bitmap scaledPiecewiseBitmap = null;
                    Graphics scalerGraphics = null;
                    Rectangle scalarRectangle = new Rectangle();
                    if (aaMode > 1)
                    {
                        scaledPiecewiseBitmap = new Bitmap(imageStepHorizSmall, imageStepVertSmall, bitmapFormat);
                        scalarRectangle = new Rectangle(0, 0, scaledPiecewiseBitmap.Width, scaledPiecewiseBitmap.Height);
                        scalerGraphics = Graphics.FromImage(scaledPiecewiseBitmap);
                        //scalerGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                        //scalerGraphics.CompositingQuality = CompositingQuality.HighQuality;
                        //scalerGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                    }
                    for (int i = 0; i < totalSS; ++i)
                    {
                        int y = i / gridSize;
                        int x = i - y * gridSize;

                        left = originalLeft + gridStepHoriz * x;
                        right = left + gridStepHoriz;
                        top = originalTop - gridStepVert * y;
                        bottom = top - gridStepVert;

                        camera.setFrustumExtents(left, right, top, bottom);
                        Root.getSingleton().clearEventTimes();
                        renderTexture.update();

                        BitmapData bmpData = pieceBitmap.LockBits(new Rectangle(new Point(), pieceBitmap.Size), ImageLockMode.WriteOnly, pieceBitmap.PixelFormat);
                        unsafe
                        {
                            PixelBox pixelBox = new PixelBox(0, 0, bmpData.Width, bmpData.Height, format, bmpData.Scan0.ToPointer());
                            renderTexture.copyContentsToMemory(pixelBox, RenderTarget.FrameBuffer.FB_AUTO);
                        }
                        pieceBitmap.UnlockBits(bmpData);
                        destRect.X = x * imageStepHorizSmall;
                        destRect.Y = y * imageStepVertSmall;
                        destRect.Width = imageStepHorizSmall;
                        destRect.Height = imageStepVertSmall;
                        //destRect, x * imageStepHorizSmall, y * imageStepVertSmall, imageStepHorizSmall, imageStepVertSmall
                        if (scalerGraphics != null)
                        {
                            //scalerGraphics.Clear(System.Drawing.Color.HotPink);
                            scalerGraphics.DrawImage(pieceBitmap, scalarRectangle);
                            g.DrawImage(scaledPiecewiseBitmap, destRect);
                        }
                        else
                        {
                            g.DrawImage(pieceBitmap, destRect, 0, 0, imageStepHoriz, imageStepVert, GraphicsUnit.Pixel);
                        }
                        //Log.Debug("{0}, {1} - {2}, {3}", x * imageStepHorizSmall, y * imageStepVertSmall, x * imageStepHorizSmall + imageStepHorizSmall, y * imageStepVertSmall + imageStepVertSmall);
                    }
                    if (scaledPiecewiseBitmap != null)
                    {
                        scalerGraphics.Dispose();
                        scaledPiecewiseBitmap.Dispose();
                    }
                }
                if (turnedOffWatermark)
                {
                    float imageFinalHeight = fullBitmap.Height * 0.0447f;
                    Bitmap logo = Medical.Properties.Resources.AnomalousMedical;
                    float scale = imageFinalHeight / logo.Height;
                    float imageFinalWidth = logo.Width * scale;
                    g.DrawImage(Medical.Properties.Resources.AnomalousMedical, new Rectangle(0, fullBitmap.Height - (int)imageFinalHeight, (int)imageFinalWidth, (int)imageFinalHeight));
                }
            }

            if (turnedOffWatermark)
            {
                watermark.Visible = true;
            }

            return fullBitmap;
        }

        public Watermark Watermark
        {
            get
            {
                return watermark;
            }
            set
            {
                watermark = value;
            }
        }

        public ViewportBackground Background
        {
            get
            {
                return background;
            }
            set
            {
                this.background = value;
            }
        }
    }
}
