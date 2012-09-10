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
    /// <summary>
    /// I apologize in advance for this class. It has evolved to be far more complex than originally intended. It tries to do way too much work itself.
    /// 
    /// The biggest issue is the async way that it works. It uses IEnumerable functions to be able to yield at various points in rendering. There is a normal
    /// synchronous call you can make, but for large images we need to be able to unwind back to the message pump and continue work on the next idle call.
    /// This class can handle all of that, but the implementation is very complex as a result.
    /// </summary>
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
        private IdleHandler idleHandler;

        private ImageRendererProgress imageRendererProgress;

        static ImageAttributes IMAGE_ATTRIBUTES = new ImageAttributes();

        public ImageRenderer(MedicalController controller, SceneViewController sceneViewController, IdleHandler idleHandler)
        {
            this.controller = controller;
            this.sceneViewController = sceneViewController;
            this.idleHandler = idleHandler;
            TransparencyController.createTransparencyState(TRANSPARENCY_STATE);
        }

        /// <summary>
        /// A synchronous image render. It will happen on the calling thread like normal.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Bitmap renderImage(ImageRendererProperties properties)
        {
            Bitmap image = null;
            IEnumerable<Object> process = renderImage(properties, (product) =>
            {
                image = product;
            });
            IEnumerator<Object> runner = process.GetEnumerator();
            while (runner.MoveNext()) ;
            return image;
        }

        /// <summary>
        /// An async image render. This will hijack the idle handler's onIdle to
        /// render the image. You supply a callback that will be called when the
        /// image completes rendering. This funciton will return immediately.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="renderingCompletedCallback"></param>
        public void renderImageAsync(ImageRendererProperties properties, Action<Bitmap> renderingCompletedCallback)
        {
            idleHandler.runTemporaryIdle(renderImage(properties, renderingCompletedCallback));
        }

        private IEnumerable<Object> renderImage(ImageRendererProperties properties, Action<Bitmap> renderingCompletedCallback)
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
                //int width = properties.Width * properties.AntiAliasingMode;
                //int height = properties.Height * properties.AntiAliasingMode;

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
                IEnumerable<Object> process = createRender(properties.Width, properties.Height, properties.AntiAliasingMode, properties.ShowWatermark, properties.TransparentBackground, backgroundColor, sceneWindow.Camera, cameraPosition, cameraLookAt,
                    (product) =>
                    {
                        bitmap = product;
                    });
                foreach (Object obj in process)
                {
                    yield return obj;
                }

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

            renderingCompletedCallback(bitmap);
            yield break;
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

        //Acutal rendering
        static int maxBackBufferSize = 2048;

        private static TexturePtr createOgreTexture(int finalWidth, int finalHeight, int aaMode, out bool gridRender, out int backBufferWidth, out int backBufferHeight)
        {
            int largeWidth = finalWidth * aaMode;
            int largeHeight = finalHeight * aaMode;
            TexturePtr backBufferTexture;

            if (largeWidth <= maxBackBufferSize && largeHeight <= maxBackBufferSize)
            {
                //Try to create a texture at the large size specified
                try
                {
                    backBufferTexture = TextureManager.getInstance().createManual("__PictureTexture", "__InternalMedical", TextureType.TEX_TYPE_2D, (uint)largeWidth, (uint)largeHeight, 1, 1, OgreWrapper.PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, false, 0);
                    backBufferWidth = largeWidth;
                    backBufferHeight = largeHeight;
                    gridRender = false;
                }
                catch (OgreException)
                {
                    gridRender = true;
                    createPow2BackBuffer(largeWidth, largeHeight, out backBufferWidth, out backBufferHeight, out backBufferTexture);
                }
            }
            else
            {
                gridRender = true;
                createPow2BackBuffer(largeWidth, largeHeight, out backBufferWidth, out backBufferHeight, out backBufferTexture);
            }
            return backBufferTexture;
        }

        private static void createPow2BackBuffer(int largeWidth, int largeHeight, out int backBufferWidth, out int backBufferHeight, out TexturePtr backBufferTexture)
        {
            //The texture creation failed. We will have to use grid rendering with power of two textures.
            int backBufferPow2Size = 2;
            while (backBufferPow2Size <= maxBackBufferSize && backBufferPow2Size <= largeWidth && backBufferPow2Size <= largeHeight)
            {
                backBufferPow2Size *= 2;
            }
            backBufferPow2Size /= 2; //We go one extra step to divide back down
            try
            {
                backBufferTexture = TextureManager.getInstance().createManual("__PictureTexture", "__InternalMedical", TextureType.TEX_TYPE_2D, (uint)backBufferPow2Size, (uint)backBufferPow2Size, 1, 1, OgreWrapper.PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, false, 0);
                backBufferWidth = backBufferPow2Size;
                backBufferHeight = backBufferPow2Size;
            }
            catch (OgreException)
            {
                backBufferWidth = 0;
                backBufferHeight = 0;
                backBufferTexture = null;
            }
        }

        private IEnumerable<Object> createRender(int finalWidth, int finalHeight, int aaMode, bool showWatermark, bool transparentBG, Engine.Color backColor, Camera cloneCamera, Vector3 position, Vector3 lookAt, Action<Bitmap> renderingCompletedCallback)
        {
            Bitmap bitmap = null;
	        OgreSceneManager sceneManager = controller.CurrentScene.getDefaultSubScene().getSimElementManager<OgreSceneManager>();
	        if (sceneManager != null)
	        {
                bool doGridRender;
                int backBufferWidth;
                int backBufferHeight;

                using (TexturePtr texture = createOgreTexture(finalWidth, finalHeight, aaMode, out doGridRender, out backBufferWidth, out backBufferHeight))
	            {
                    if (texture != null)
                    {
                        using (HardwarePixelBufferSharedPtr pixelBuffer = texture.Value.getBuffer())
                        {
                            RenderTexture renderTexture = pixelBuffer.Value.getRenderTarget();
                            Camera camera = sceneManager.SceneManager.createCamera("__PictureCamera");
                            camera.setLodBias(cloneCamera.getLodBias());
                            camera.setUseRenderingDistance(cloneCamera.getUseRenderingDistance());
                            camera.setNearClipDistance(cloneCamera.getNearClipDistance());
                            camera.setFarClipDistance(cloneCamera.getFarClipDistance());
                            camera.setPolygonMode(cloneCamera.getPolygonMode());
                            camera.setRenderingDistance(cloneCamera.getRenderingDistance());
                            camera.setProjectionType(cloneCamera.getProjectionType());
                            camera.setFOVy(cloneCamera.getFOVy());

                            camera.setAutoAspectRatio(false);
                            camera.setAspectRatio((float)finalWidth / finalHeight);

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

                            if (doGridRender)
                            {
                                IEnumerable<Object> process = gridRender(finalWidth * aaMode, finalHeight * aaMode, backBufferWidth, backBufferHeight, aaMode, showWatermark, renderTexture, camera, transparentBG, backColor,
                                    (product) =>
                                    {
                                        bitmap = product;
                                    });
                                foreach (Object obj in process)
                                {
                                    yield return obj;
                                }
                            }
                            else
                            {
                                bitmap = simpleRender(backBufferWidth, backBufferHeight, aaMode, showWatermark, transparentBG, backColor, renderTexture);
                            }

                            renderTexture.destroyViewport(viewport);
                            sceneManager.SceneManager.getRootSceneNode().removeChild(node);
                            sceneManager.SceneManager.destroyLight(light);
                            sceneManager.SceneManager.destroySceneNode(node);
                            sceneManager.SceneManager.destroyCamera(camera);

                            TextureManager.getInstance().remove(texture);
                        }
                    }
                    else
                    {
                        //An error making the render texture. Log it and return the error image.
                        Log.Error("Could not render image. Returning placeholder image. Reason: Could not create valid render to texture target.");
                        bitmap = new Bitmap(finalWidth, finalHeight);
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            using (Brush brush = new SolidBrush(System.Drawing.Color.Black))
                            {
                                g.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
                            }
                            int fontSize = bitmap.Width / 5;
                            using (Font font = new Font("Tahoma", fontSize, GraphicsUnit.Pixel))
                            {
                                String text = "Error";
                                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                                SizeF textSize = g.MeasureString(text, font, bitmap.Width);
                                g.DrawString(text, font, Brushes.Red, new RectangleF(0, 0, textSize.Width, textSize.Height));
                            }
                        }
                    }
	            }
	        }
            renderingCompletedCallback(bitmap);
            yield break;
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
                    renderBitmaps(graph, new Rectangle(0, 0, smallWidth, smallHeight), largeImage, largeImage.Width, largeImage.Height, transparentBG, bgColor);
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

        private IEnumerable<Object> gridRender(int width, int height, int backBufferWidth, int backBufferHeight, int aaMode, bool showWatermark, RenderTexture renderTexture, Camera camera, bool transparentBG, Engine.Color bgColor, Action<Bitmap> renderingCompletedCallback)
        {
            renderTexture.getViewport(0).setOverlaysEnabled(false);

            float originalLeft, originalRight, originalTop, originalBottom;
            camera.getFrustumExtents(out originalLeft, out originalRight, out originalTop, out originalBottom);

            int imageCountWidth = width % backBufferWidth == 0 ? width / backBufferWidth : width / backBufferWidth + 1;
            int imageCountHeight = height % backBufferHeight == 0 ? height / backBufferHeight : height / backBufferHeight + 1;

            //these two lines are likely what is distorting the image
            float gridStepHoriz = (originalRight * 2) / imageCountWidth;
            float gridStepVert = (originalTop * 2) / imageCountHeight;

            int imageStepHoriz = backBufferWidth;
            int imageStepVert = backBufferHeight;

            int finalWidth = width / aaMode;
            int finalHeight = height / aaMode;
            int imageStepHorizSmall = finalWidth / imageCountWidth;
            int imageStepVertSmall = finalHeight / imageCountHeight;

            float left, right, top, bottom;
            int totalSS = imageCountWidth * imageCountHeight;

            String updateString = "Rendering piece {0} of " + totalSS;

            OgreWrapper.PixelFormat format = OgreWrapper.PixelFormat.PF_A8R8G8B8;
            System.Drawing.Imaging.PixelFormat bitmapFormat = System.Drawing.Imaging.PixelFormat.Format32bppRgb;
            if (transparentBG)
            {
                bitmapFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            }

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
                        scalerGraphics.CompositingMode = CompositingMode.SourceCopy;
                    }
                    for (int i = 0; i < totalSS; ++i)
                    {
                        int y = i / imageCountWidth;
                        int x = i - y * imageCountWidth;

                        left = originalLeft + gridStepHoriz * x;
                        right = left + gridStepHoriz;
                        top = originalTop - gridStepVert * y;
                        bottom = top - gridStepVert;

                        camera.setFrustumExtents(left, right, top, bottom);
                        Root.getSingleton().clearEventTimes();
                        renderTexture.update();

                        BitmapData bmpData = pieceBitmap.LockBits(new Rectangle(new Point(), pieceBitmap.Size), ImageLockMode.WriteOnly, pieceBitmap.PixelFormat);
                        unsafeAsyncBufferCopy(renderTexture, format, bmpData);
                        //unsafe
                        //{
                        //    using (PixelBox pixelBox = new PixelBox(0, 0, bmpData.Width, bmpData.Height, format, bmpData.Scan0.ToPointer()))
                        //    {
                        //        renderTexture.copyContentsToMemory(pixelBox, RenderTarget.FrameBuffer.FB_AUTO);
                        //    }
                        //}
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

                        yield return null;
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

            renderingCompletedCallback(fullBitmap);
            yield break;
        }

        private static void renderBitmaps(Graphics destGraphics, Rectangle destRect, Bitmap source, int sourceWidth, int sourceHeight, bool transparentBG, Engine.Color bgColor)
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

        private static unsafe void unsafeAsyncBufferCopy(RenderTexture renderTexture, OgreWrapper.PixelFormat format, BitmapData bmpData)
        {
            using (PixelBox pixelBox = new PixelBox(0, 0, bmpData.Width, bmpData.Height, format, bmpData.Scan0.ToPointer()))
            {
                renderTexture.copyContentsToMemory(pixelBox, RenderTarget.FrameBuffer.FB_AUTO);
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
