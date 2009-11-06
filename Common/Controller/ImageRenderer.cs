using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OgrePlugin;
using OgreWrapper;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Medical
{
    public class ImageException : Exception
    {
        public ImageException(String message)
            :base(message)
        {

        }
    }

    public class ImageRenderer
    {
        private MedicalController controller;
        DrawingWindowController drawingWindowController;
        private Watermark watermark;

        public ImageRenderer(MedicalController controller, DrawingWindowController drawingWindowController)
        {
            this.controller = controller;
            this.drawingWindowController = drawingWindowController;
        }

        public Bitmap renderImage(int width, int height)
        {
            return renderImage(width, height, false);
        }

        public Bitmap renderImage(int width, int height, bool makeBGTransparent)
        {
            DrawingWindowHost drawingWindow = drawingWindowController.getActiveWindow();
            return renderImage(width, height, makeBGTransparent, drawingWindow.DrawingWindow.BackColor);
        }

        public Bitmap renderImage(int width, int height, bool makeBGTransparent, int antiAliasMode)
        {
            DrawingWindowHost drawingWindow = drawingWindowController.getActiveWindow();
            return renderImage(width, height, makeBGTransparent, drawingWindow.DrawingWindow.BackColor, antiAliasMode);
        }

        public Bitmap renderImage(int width, int height, bool makeBGTransparent, Color backColor, int antiAliasMode)
        {
            if (antiAliasMode < 1)
            {
                throw new ImageException("Cannot set an anti aliasing mode of less than 1.");
            }
            int aaWidth = width * antiAliasMode;
            int aaHeight = height * antiAliasMode;
            using (Bitmap largeImage = renderImage(aaWidth, aaHeight, makeBGTransparent, backColor))
            {
                Bitmap smallImage = new Bitmap(width, height);
                using (Graphics graph = Graphics.FromImage(smallImage))
                {
                    graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    graph.CompositingQuality = CompositingQuality.HighQuality;
                    graph.SmoothingMode = SmoothingMode.AntiAlias;
                    graph.DrawImage(largeImage, new Rectangle(0, 0, width, height));
                }
                return smallImage;
            }
        }

        public Bitmap renderImage(int width, int height, bool makeBGTransparent, Color backColor)
        {
            OgreSceneManager sceneManager = controller.CurrentScene.getDefaultSubScene().getSimElementManager<OgreSceneManager>();
            DrawingWindowHost drawingWindow = drawingWindowController.getActiveWindow();
            if (sceneManager != null && drawingWindow != null)
            {
                using (TexturePtr texture = TextureManager.getInstance().createManual("__PictureTexture", "__InternalMedical", TextureType.TEX_TYPE_2D, (uint)width, (uint)height, 1, 1, OgreWrapper.PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, false, 0))
                {
                    using (HardwarePixelBufferSharedPtr pixelBuffer = texture.Value.getBuffer())
                    {
                        RenderTexture renderTexture = pixelBuffer.Value.getRenderTarget();
                        Camera camera = sceneManager.SceneManager.createCamera("__PictureCamera");
                        Camera cloneCamera = drawingWindow.DrawingWindow.Camera;
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
                        node.setPosition(drawingWindow.DrawingWindow.Translation);
                        sceneManager.SceneManager.getRootSceneNode().addChild(node);
                        camera.lookAt(drawingWindow.DrawingWindow.LookAt);
                        Light light = sceneManager.SceneManager.createLight("__PictureCameraLight");
                        node.attachObject(light);
                        Viewport viewport = renderTexture.addViewport(camera);
                        viewport.setBackgroundColor(Engine.Color.FromARGB(backColor.ToArgb()));

                        if (watermark != null)
                        {
                            watermark.sizeChanged(width, height);
                            watermark.setVisible(true);
                        }

                        renderTexture.update();
                        OgreWrapper.PixelFormat format = OgreWrapper.PixelFormat.PF_A8R8G8B8;
                        System.Drawing.Imaging.PixelFormat bitmapFormat = System.Drawing.Imaging.PixelFormat.Format32bppRgb;
                        Bitmap bitmap = new Bitmap(width, height, bitmapFormat);
                        BitmapData bmpData = bitmap.LockBits(new Rectangle(new Point(), bitmap.Size), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                        unsafe
                        {
                            PixelBox pixelBox = new PixelBox(0, 0, (uint)bmpData.Width, (uint)bmpData.Height, format, bmpData.Scan0.ToPointer());
                            renderTexture.copyContentsToMemory(pixelBox, RenderTarget.FrameBuffer.FB_AUTO);
                        }
                        bitmap.UnlockBits(bmpData);

                        if (watermark != null)
                        {
                            watermark.setVisible(false);
                        }

                        renderTexture.destroyViewport(viewport);
                        sceneManager.SceneManager.getRootSceneNode().removeChild(node);
                        sceneManager.SceneManager.destroyLight(light);
                        sceneManager.SceneManager.destroySceneNode(node);
                        sceneManager.SceneManager.destroyCamera(camera);

                        if (makeBGTransparent)
                        {
                            bitmap.MakeTransparent(backColor);
                        }

                        return bitmap;
                    }
                }
            }
            return null;
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
    }
}
