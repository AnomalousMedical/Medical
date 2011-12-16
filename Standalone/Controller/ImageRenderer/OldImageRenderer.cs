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

namespace Medical.Old
{
    class ImageException : Exception
    {
        public ImageException(String message)
            : base(message)
        {

        }
    }

    public class ImageRenderer
    {
        private static readonly String TRANSPARENCY_STATE = "ImageRenderer";

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

        private ImageRendererProgress imageRendererProgress;

        static ImageAttributes IMAGE_ATTRIBUTES = new ImageAttributes();

        public ImageRenderer(MedicalController controller, SceneViewController sceneViewController)
        {
            this.controller = controller;
            this.sceneViewController = sceneViewController;
            TransparencyController.createTransparencyState(TRANSPARENCY_STATE);
        }

        public Bitmap renderImage(ImageRendererProperties properties)
        {
            if (imageRendererProgress != null)
            {
                imageRendererProgress.Visible = properties.ShowUIUpdates;
                imageRendererProgress.Cancel = false;
                imageRendererProgress.update(0, "Rendering Image");
            }

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
                    cameraPosition = properties.CameraPosition;
                    cameraLookAt = properties.CameraLookAt;
                }

                //Background Activation
                if (background != null && properties.ShowBackground)
                {
                    background.setVisible(true);
                }

                //Layer override
                String activeTransparencyState = TransparencyController.ActiveTransparencyState;
                if (properties.OverrideLayers)
                {
                    TransparencyController.ActiveTransparencyState = TRANSPARENCY_STATE;
                    if (properties.LayerState != null)
                    {
                        properties.LayerState.instantlyApply();
                    }
                    else
                    {
                        Log.Warning("ImageRenderer was told to override layers but no layer state was provided.");
                    }
                }

                TransparencyController.applyTransparencyState(TransparencyController.ActiveTransparencyState);

                //Render
                bitmap = createRender(width, height, properties.NumGridTiles, properties.AntiAliasingMode, properties.ShowWatermark, properties.TransparentBackground, backgroundColor, sceneWindow.Camera, cameraPosition, cameraLookAt);

                //Turn off layer override
                if (properties.OverrideLayers)
                {
                    TransparencyController.ActiveTransparencyState = activeTransparencyState;
                }

                //Background deactivation
                if (background != null && properties.ShowBackground)
                {
                    background.setVisible(false);
                }

                if (ImageRenderCompleted != null)
                {
                    ImageRenderCompleted.Invoke(this, EventArgs.Empty);
                }
            }

            if (imageRendererProgress != null && properties.ShowUIUpdates)
            {
                imageRendererProgress.Visible = false;
            }

            return bitmap;
        }

        public void addLicenseText(Bitmap bitmap, String text, int textPixelHeight)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                using (Font font = new Font("Tahoma", textPixelHeight, GraphicsUnit.Pixel))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    SizeF textSize = g.MeasureString(text, font, bitmap.Width);
                    g.DrawString(text, font, Brushes.White, new RectangleF(0, 0, textSize.Width, textSize.Height));
                }
            }
        }

        public void makeSampleImage(Bitmap bitmap)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                Bitmap logo = Medical.Properties.Resources.AnomalousMedical;
                ColorMatrix colorMatrix = new ColorMatrix();
                colorMatrix.Matrix00 = colorMatrix.Matrix11 = colorMatrix.Matrix22 = colorMatrix.Matrix44 = 1;
                colorMatrix.Matrix33 = 0.30f;
                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetColorMatrix(colorMatrix);

                    float sizeRatio = (float)bitmap.Width / logo.Width;
                    int finalLogoWidth = (int)(logo.Width * sizeRatio);
                    int finalLogoHeight = (int)(logo.Height * sizeRatio);
                    int currentHeight = 0;
                    int imageHeight = bitmap.Height;
                    while (currentHeight < imageHeight)
                    {
                        g.DrawImage(Medical.Properties.Resources.AnomalousMedical, new Rectangle(0, currentHeight, finalLogoWidth, finalLogoHeight), 0, 0, logo.Width, logo.Height, GraphicsUnit.Pixel, ia);
                        currentHeight += finalLogoHeight;
                    }
                }
            }
        }

        public ImageRendererProgress ImageRendererProgress
        {
            get
            {
                return imageRendererProgress;
            }
            set
            {
                imageRendererProgress = value;
            }
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
                            bitmap = gridRender(width, height, gridSize, aaMode, showWatermark, renderTexture, camera, transparentBG, backColor);
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
                using (PixelBox pixelBox = new PixelBox(0, 0, bmpData.Width, bmpData.Height, format, bmpData.Scan0.ToPointer()))
                {
                    renderTexture.copyContentsToMemory(pixelBox, RenderTarget.FrameBuffer.FB_AUTO);
                }
            }
            bitmap.UnlockBits(bmpData);

            //Resize if aa is active
            if (aaMode > 1)
            {
                int smallWidth = width / aaMode;
                int smallHeight = height / aaMode;
                Bitmap largeImage = bitmap;
                bitmap = new Bitmap(smallWidth, smallHeight);
                using (Graphics graph = Graphics.FromImage(bitmap))
                {
                    graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    graph.CompositingQuality = CompositingQuality.HighQuality;
                    graph.SmoothingMode = SmoothingMode.AntiAlias;
                    this.renderBitmaps(graph, new Rectangle(0, 0, smallWidth, smallHeight), largeImage, largeImage.Width, largeImage.Height, transparentBG, bgColor);
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

        private Bitmap gridRender(int width, int height, int gridSize, int aaMode, bool showWatermark, RenderTexture renderTexture, Camera camera, bool transparentBG, Engine.Color bgColor)
        {
            renderTexture.getViewport(0).setOverlaysEnabled(false);

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

            float left, right, top, bottom;
            gridSize += 1; //Account for any extra space not covered by the grid size.
            int totalSS = gridSize * gridSize;

            String updateString = "Rendering piece {0} of " + totalSS;

            OgreWrapper.PixelFormat format = OgreWrapper.PixelFormat.PF_A8R8G8B8;
            System.Drawing.Imaging.PixelFormat bitmapFormat = System.Drawing.Imaging.PixelFormat.Format32bppRgb;

            Rectangle destRect = new Rectangle();
            Rectangle srcRect = new Rectangle(0, 0, imageStepHoriz, imageStepVert);
            Bitmap fullBitmap = new Bitmap(finalWidth, finalHeight, bitmapFormat);
            using (Graphics g = Graphics.FromImage(fullBitmap))
            {
                using (Bitmap pieceBitmap = new Bitmap(imageStepHoriz, imageStepVert, bitmapFormat))
                {
                    Bitmap scaledPiecewiseBitmap = null;
                    Graphics scalerGraphics = null; //Will remain null if AA is turned off.
                    Rectangle scalarRectangle = new Rectangle();
                    if (aaMode > 1)
                    {
                        scaledPiecewiseBitmap = new Bitmap(imageStepHorizSmall, imageStepVertSmall, bitmapFormat);
                        scalarRectangle = new Rectangle(0, 0, scaledPiecewiseBitmap.Width, scaledPiecewiseBitmap.Height);
                        scalerGraphics = Graphics.FromImage(scaledPiecewiseBitmap);
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
                            using (PixelBox pixelBox = new PixelBox(0, 0, bmpData.Width, bmpData.Height, format, bmpData.Scan0.ToPointer()))
                            {
                                renderTexture.copyContentsToMemory(pixelBox, RenderTarget.FrameBuffer.FB_AUTO);
                            }
                        }
                        pieceBitmap.UnlockBits(bmpData);
                        destRect.X = x * imageStepHorizSmall;
                        destRect.Y = y * imageStepVertSmall;
                        destRect.Width = imageStepHorizSmall;
                        destRect.Height = imageStepVertSmall;
                        if (scalerGraphics != null) //Meaning AA is turned on.
                        {
                            renderBitmaps(scalerGraphics, scalarRectangle, pieceBitmap, pieceBitmap.Width, pieceBitmap.Height, transparentBG, bgColor);
                            g.DrawImage(scaledPiecewiseBitmap, destRect);
                        }
                        else
                        {
                            renderBitmaps(g, destRect, pieceBitmap, imageStepHoriz, imageStepVert, transparentBG, bgColor);
                        }

                        if (imageRendererProgress != null)
                        {
                            imageRendererProgress.update((uint)(((float)(i + 1) / totalSS) * 100.0f), String.Format(updateString, i + 1));
                            if (imageRendererProgress.Cancel)
                            {
                                break;
                            }
                        }
                    }
                    if (scaledPiecewiseBitmap != null)
                    {
                        scalerGraphics.Dispose();
                        scaledPiecewiseBitmap.Dispose();
                    }
                }
                if (showWatermark)
                {
                    float imageFinalHeight = fullBitmap.Height * 0.0447f;
                    Bitmap logo = Medical.Properties.Resources.AnomalousMedical;
                    float scale = imageFinalHeight / logo.Height;
                    float imageFinalWidth = logo.Width * scale;
                    if (imageFinalWidth > fullBitmap.Width)
                    {
                        imageFinalWidth = fullBitmap.Width;
                        scale = imageFinalWidth / logo.Width;
                        imageFinalHeight = logo.Height * scale;
                    }
                    g.DrawImage(Medical.Properties.Resources.AnomalousMedical, new Rectangle(0, fullBitmap.Height - (int)imageFinalHeight, (int)imageFinalWidth, (int)imageFinalHeight));
                }
            }

            renderTexture.getViewport(0).setOverlaysEnabled(true);

            return fullBitmap;
        }

        private void renderBitmaps(Graphics destGraphics, Rectangle destRect, Bitmap source, int sourceWidth, int sourceHeight, bool transparentBG, Engine.Color bgColor)
        {
            if (transparentBG)
            {
                System.Drawing.Color colorKey = System.Drawing.Color.FromArgb(bgColor.toARGB());
                IMAGE_ATTRIBUTES.SetColorKey(colorKey, PlatformConfig.getSecondColorKey(colorKey));
                destGraphics.DrawImage(source, destRect, 0, 0, sourceWidth, sourceHeight, GraphicsUnit.Pixel, IMAGE_ATTRIBUTES, null, IntPtr.Zero);
            }
            else
            {
                destGraphics.DrawImage(source, destRect);
            }
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
