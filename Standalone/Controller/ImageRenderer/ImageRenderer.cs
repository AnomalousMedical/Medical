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
using FreeImageAPI;

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
        private const String TransparencyStateName = "ImageRenderer";
        private const String RenderTextureResourceGroup = "__ImageRendererTextureGroup";

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
        private BackgroundScene background;
        private IdleHandler idleHandler;

        private ImageRendererProgress imageRendererProgress;

        static ImageAttributes IMAGE_ATTRIBUTES = new ImageAttributes();

        public ImageRenderer(MedicalController controller, SceneViewController sceneViewController, IdleHandler idleHandler)
        {
            this.controller = controller;
            this.sceneViewController = sceneViewController;
            this.idleHandler = idleHandler;
            TransparencyController.createTransparencyState(TransparencyStateName);
            OgreResourceGroupManager.getInstance().createResourceGroup(RenderTextureResourceGroup);
        }

        /// <summary>
        /// A synchronous image render. It will happen on the calling thread like normal.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Bitmap renderImage(ImageRendererProperties properties)
        {
            Bitmap image = null;
            IEnumerable<IdleStatus> process = renderImage(properties, (product) =>
            {
                image = product;
            });
            IEnumerator<IdleStatus> runner = process.GetEnumerator();
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

        private IEnumerable<IdleStatus> renderImage(ImageRendererProperties properties, Action<Bitmap> renderingCompletedCallback)
        {
            if (imageRendererProgress != null)
            {
                imageRendererProgress.Visible = properties.ShowUIUpdates;
                imageRendererProgress.Cancel = false;
                imageRendererProgress.update(0, "Rendering Image");
            }

            FreeImageBitmap bitmap = null;
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

                //Turn off background if needed
                if (background != null)
                {
                    background.setVisible(properties.ShowBackground);
                }

                //Layer override
                String activeTransparencyState = TransparencyController.ActiveTransparencyState;
                if (properties.OverrideLayers)
                {
                    TransparencyController.ActiveTransparencyState = TransparencyStateName;
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
                IEnumerable<IdleStatus> process = createRender(properties.Width, properties.Height, properties.AntiAliasingMode, properties.ShowWatermark, properties.TransparentBackground, backgroundColor, sceneWindow.Camera, cameraPosition, cameraLookAt, sceneWindow.NearPlaneWorldPos, sceneWindow.FarPlaneWorldPos, properties,
                    (product) =>
                    {
                        bitmap = product;
                    });
                foreach (IdleStatus idleStatus in process)
                {
                    yield return idleStatus;
                }

                //Turn off layer override
                if (properties.OverrideLayers)
                {
                    TransparencyController.ActiveTransparencyState = activeTransparencyState;
                }

                //Reactivate background
                if (background != null)
                {
                    background.setVisible(true);
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

            renderingCompletedCallback(bitmap.ToBitmap());
            bitmap.Dispose();
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
            if (Logo != null)
            {
                using (Bitmap bmpLogo = Logo.ToBitmap())
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        ColorMatrix colorMatrix = new ColorMatrix();
                        colorMatrix.Matrix00 = colorMatrix.Matrix11 = colorMatrix.Matrix22 = colorMatrix.Matrix44 = 1;
                        colorMatrix.Matrix33 = 0.30f;
                        using (ImageAttributes ia = new ImageAttributes())
                        {
                            ia.SetColorMatrix(colorMatrix);

                            float sizeRatio = (float)bitmap.Width / Logo.Width;
                            int finalLogoWidth = (int)(Logo.Width * sizeRatio);
                            int finalLogoHeight = (int)(Logo.Height * sizeRatio);
                            int currentHeight = 0;
                            int imageHeight = bitmap.Height;
                            while (currentHeight < imageHeight)
                            {
                                g.DrawImage(bmpLogo, new Rectangle(0, currentHeight, finalLogoWidth, finalLogoHeight), 0, 0, Logo.Width, Logo.Height, GraphicsUnit.Pixel, ia);
                                currentHeight += finalLogoHeight;
                            }
                        }
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
                    backBufferTexture = TextureManager.getInstance().createManual("__PictureTexture", RenderTextureResourceGroup, TextureType.TEX_TYPE_2D, (uint)largeWidth, (uint)largeHeight, 1, 1, OgreWrapper.PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, false, 0);
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
                backBufferTexture = TextureManager.getInstance().createManual("__PictureTexture", RenderTextureResourceGroup, TextureType.TEX_TYPE_2D, (uint)backBufferPow2Size, (uint)backBufferPow2Size, 1, 1, OgreWrapper.PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, false, 0);
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

        private IEnumerable<IdleStatus> createRender(int finalWidth, int finalHeight, int aaMode, bool showWatermark, bool transparentBG, Engine.Color backColor, Camera cloneCamera, Vector3 position, Vector3 lookAt, float nearWorldPos, float farWorldPos, ImageRendererProperties properties, Action<FreeImageBitmap> renderingCompletedCallback)
        {
            FreeImageBitmap bitmap = null;
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
                            Viewport viewport = renderTexture.addViewport(camera, 1, 0.0f, 0.0f, 1.0f, 1.0f);

                            if (properties.UseIncludePoint)
                            {
                                Matrix4x4 viewMatrix = camera.getViewMatrix();
                                Matrix4x4 projectionMatrix = camera.getProjectionMatrix();
                                float aspect = camera.getAspectRatio();
                                float fovy = camera.getFOVy() * 0.5f;

                                float distance = SceneViewWindow.computeOffsetToIncludePoint(viewMatrix, projectionMatrix, properties.IncludePoint, aspect, fovy);
                                Vector3 direction = (position - lookAt).normalized();
                                node.setPosition(position - (direction * distance));
                                camera.lookAt(lookAt);
                            }

                            if (transparentBG)
                            {
                                backColor.a = 0.0f;
                            }

                            ViewportBackground bgViewport = null;
                            if (background != null)
                            {
                                bgViewport = new ViewportBackground("ImageRenderer", 0, background, renderTexture, false);
                                bgViewport.BackgroundColor = backColor;
                                bgViewport.Camera.setAutoAspectRatio(false);
                                bgViewport.Camera.setAspectRatio((float)finalWidth / finalHeight);
                            }
                            viewport.setBackgroundColor(backColor);
                            viewport.setOverlaysEnabled(false);
                            viewport.setClearEveryFrame(false);

                            if (properties.CustomizeCameraPosition != null)
                            {
                                properties.CustomizeCameraPosition(camera, viewport);
                            }

                            float near, far;
                            CameraPositioner.computeClipDistances(camera.getDerivedPosition().length(), nearWorldPos, farWorldPos, out near, out far);
                            camera.setNearClipDistance(near);
                            camera.setFarClipDistance(far);

                            if (doGridRender)
                            {
                                IEnumerable<IdleStatus> process = gridRender(finalWidth * aaMode, finalHeight * aaMode, backBufferWidth, backBufferHeight, aaMode, renderTexture, camera, bgViewport != null ? bgViewport.Camera : null, transparentBG, backColor,
                                    (product) =>
                                    {
                                        bitmap = product;
                                    });
                                foreach (IdleStatus idleStatus in process)
                                {
                                    yield return idleStatus;
                                }
                            }
                            else
                            {
                                bitmap = simpleRender(backBufferWidth, backBufferHeight, aaMode, transparentBG, backColor, renderTexture);
                            }

                            if (showWatermark && Logo != null)
                            {
                                float imageFinalHeight = bitmap.Height * 0.0447f;
                                float scale = imageFinalHeight / Logo.Height;
                                float imageFinalWidth = Logo.Width * scale;
                                if (imageFinalWidth > bitmap.Width)
                                {
                                    imageFinalWidth = bitmap.Width;
                                    scale = imageFinalWidth / Logo.Width;
                                    imageFinalHeight = Logo.Height * scale;
                                }
                                using(FreeImageBitmap resizedLogo = new FreeImageBitmap(Logo))
                                {
                                    resizedLogo.Rescale((int)imageFinalWidth, (int)imageFinalHeight, FREE_IMAGE_FILTER.FILTER_BILINEAR);
                                    //Have to composite the logo image first.
                                    using (FreeImageBitmap fullImageCorner = bitmap.Copy(0, bitmap.Height - (int)imageFinalHeight, (int)imageFinalWidth, bitmap.Height))
                                    {
                                        fullImageCorner.ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH.FICD_24_BPP);
                                        resizedLogo.Composite(false, null, fullImageCorner);
                                    }
                                    bitmap.Paste(resizedLogo, 0, bitmap.Height - (int)imageFinalHeight, int.MaxValue);
                                }
                            }

                            renderTexture.destroyViewport(viewport);
                            if (bgViewport != null)
                            {
                                bgViewport.Dispose();
                            }
                            sceneManager.SceneManager.getRootSceneNode().removeChild(node);
                            sceneManager.SceneManager.destroySceneNode(node);
                            sceneManager.SceneManager.destroyCamera(camera);

                            TextureManager.getInstance().remove(texture);
                        }
                    }
                    else
                    {
                        //An error making the render texture. Log it and return the error image.
                        Log.Error("Could not render image. Returning placeholder image. Reason: Could not create valid render to texture target.");
                        bitmap = new FreeImageBitmap(finalWidth, finalHeight);
                        bitmap.FillBackground(new RGBQUAD()
                        {
                            rgbRed = 255
                        });
                    }
	            }
	        }
            renderingCompletedCallback(bitmap);
            yield break;
        }

        private FreeImageBitmap simpleRender(int width, int height, int aaMode, bool transparentBG, Engine.Color bgColor, RenderTexture renderTexture)
        {
            renderTexture.getViewport(0).clear(FrameBufferType.FBT_COLOUR | FrameBufferType.FBT_DEPTH | FrameBufferType.FBT_STENCIL, bgColor);
            renderTexture.update();
            OgreWrapper.PixelFormat format = OgreWrapper.PixelFormat.PF_A8R8G8B8;
            System.Drawing.Imaging.PixelFormat bitmapFormat = System.Drawing.Imaging.PixelFormat.Format32bppRgb;
            if (transparentBG)
            {
                bitmapFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            }
            FreeImageBitmap bitmap = new FreeImageBitmap(width, height, bitmapFormat);
            renderTextureToFreeImageBitmap(renderTexture, format, bitmap);

            //Resize if aa is active
            if (aaMode > 1)
            {
                int smallWidth = width / aaMode;
                int smallHeight = height / aaMode;
                bitmap.Rescale(smallWidth, smallHeight, FREE_IMAGE_FILTER.FILTER_BILINEAR);
            }

            return bitmap;
        }

        private IEnumerable<IdleStatus> gridRender(int width, int height, int backBufferWidth, int backBufferHeight, int aaMode, RenderTexture renderTexture, Camera camera, Camera backgroundCamera, bool transparentBG, Engine.Color bgColor, Action<FreeImageBitmap> renderingCompletedCallback)
        {
            float originalLeft, originalRight, originalTop, originalBottom;
            camera.getFrustumExtents(out originalLeft, out originalRight, out originalTop, out originalBottom);

            int imageCountWidth = width % backBufferWidth == 0 ? width / backBufferWidth : width / backBufferWidth + 1;
            int imageCountHeight = height % backBufferHeight == 0 ? height / backBufferHeight : height / backBufferHeight + 1;

            float gridStepHoriz = (originalRight * 2) / imageCountWidth;
            float gridStepVert = (originalTop * 2) / imageCountHeight;

            float bgOriginalLeft = 0, bgOriginalRight = 0, bgOriginalTop = 0, bgOriginalBottom = 0, bgGridStepHoriz = 0, bgGridStepVert = 0;
            if (backgroundCamera != null)
            {
                backgroundCamera.getFrustumExtents(out bgOriginalLeft, out bgOriginalRight, out bgOriginalTop, out bgOriginalBottom);
                bgGridStepHoriz = (bgOriginalRight * 2) / imageCountWidth;
                bgGridStepVert = (bgOriginalTop * 2) / imageCountHeight;
            }

            int imageStepHoriz = backBufferWidth;
            int imageStepVert = backBufferHeight;

            int finalWidth = width / aaMode;
            int finalHeight = height / aaMode;
            int imageStepHorizSmall = finalWidth / imageCountWidth;
            int imageStepVertSmall = finalHeight / imageCountHeight;

            float left, right, top, bottom;
            float bgLeft, bgRight, bgTop, bgBottom;
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
            FreeImageBitmap fullBitmap = new FreeImageBitmap(finalWidth, finalHeight, bitmapFormat);

            using (FreeImageBitmap pieceBitmap = new FreeImageBitmap(imageStepHoriz, imageStepVert, bitmapFormat))
            {
                bool aaOn = aaMode > 1;
                Rectangle scalarRectangle = new Rectangle();
                if (aaOn)
                {
                    scalarRectangle = new Rectangle(0, 0, imageStepHorizSmall, imageStepVertSmall);
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
                    if (backgroundCamera != null)
                    {
                        bgLeft = bgOriginalLeft + bgGridStepHoriz * x;
                        bgRight = bgLeft + bgGridStepHoriz;
                        bgTop = bgOriginalTop - bgGridStepVert * y;
                        bgBottom = bgTop - bgGridStepVert;

                        backgroundCamera.setFrustumExtents(bgLeft, bgRight, bgTop, bgBottom);
                    }
                    Root.getSingleton().clearEventTimes();
                    renderTexture.getViewport(0).clear(FrameBufferType.FBT_COLOUR | FrameBufferType.FBT_DEPTH | FrameBufferType.FBT_STENCIL, bgColor);
                    renderTexture.update();

                    renderTextureToFreeImageBitmap(renderTexture, format, pieceBitmap);
                    destRect.X = x * imageStepHorizSmall;
                    destRect.Y = y * imageStepVertSmall;
                    destRect.Width = imageStepHorizSmall;
                    destRect.Height = imageStepVertSmall;
                    if (aaOn)
                    {
                        using(FreeImageBitmap scaled = new FreeImageBitmap(pieceBitmap))
                        {
                            scaled.Rescale(scalarRectangle.Width, scalarRectangle.Height, FREE_IMAGE_FILTER.FILTER_BILINEAR);
                            fullBitmap.Paste(scaled, destRect.X, destRect.Y, int.MaxValue);
                        }
                    }
                    else
                    {
                        fullBitmap.Paste(pieceBitmap, destRect.X, destRect.Y, int.MaxValue);
                    }

                    if (imageRendererProgress != null)
                    {
                        imageRendererProgress.update((uint)(((float)(i + 1) / totalSS) * 100.0f), String.Format(updateString, i + 1));
                        if (imageRendererProgress.Cancel)
                        {
                            break;
                        }
                    }

                    yield return IdleStatus.Ok;
                }
            }

            renderTexture.getViewport(0).setOverlaysEnabled(true);

            renderingCompletedCallback(fullBitmap);
            yield break;
        }

        unsafe private static void renderTextureToFreeImageBitmap(RenderTexture renderTexture, OgreWrapper.PixelFormat format, FreeImageBitmap bitmap)
        {
            using (PixelBox pixelBox = new PixelBox(0, 0, bitmap.Width, bitmap.Height, format, bitmap.GetScanlinePointer(0).ToPointer()))
            {
                renderTexture.copyContentsToMemory(pixelBox, RenderTarget.FrameBuffer.FB_AUTO);
            }
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }

        public BackgroundScene Background
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

        public FreeImageBitmap Logo { get; set; }
    }
}
