using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Engine;
using OgreWrapper;
using OgrePlugin;
using System.Drawing.Imaging;

namespace Medical.GUI
{
    public partial class PictureControl : GUIElement
    {
        private MedicalController controller;
        private DrawingWindowController drawingWindowController;

        public PictureControl()
        {
            InitializeComponent();
        }

        public void initialize(MedicalController medicalController, DrawingWindowController drawingWindowController)
        {
            this.controller = medicalController;
            this.drawingWindowController = drawingWindowController;
        }

        private void renderSingleButton_Click(object sender, EventArgs e)
        {
            OgreSceneManager sceneManager = controller.CurrentScene.getDefaultSubScene().getSimElementManager<OgreSceneManager>();
            DrawingWindowHost drawingWindow = drawingWindowController.getActiveWindow();
            if (sceneManager != null && drawingWindow != null)
            {
                int width = (int)resolutionWidth.Value;
                int height = (int)resolutionHeight.Value;
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
                        viewport.setBackgroundColor(Engine.Color.FromARGB(drawingWindow.DrawingWindow.BackColor.ToArgb()));
                        
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
                        
                        renderTexture.destroyViewport(viewport);
                        sceneManager.SceneManager.getRootSceneNode().removeChild(node);
                        sceneManager.SceneManager.destroyLight(light);
                        sceneManager.SceneManager.destroySceneNode(node);
                        sceneManager.SceneManager.destroyCamera(camera);

                        PictureWindow picture = new PictureWindow();
                        picture.initialize(bitmap);
                        picture.Text = String.Format("{0} - {1}x{2}", drawingWindow.Text, width, height);
                        picture.Show(DockPanel, DockState.Float);
                    }
                }
            }
        }

        private void megapixel1dot3_CheckedChanged(object sender, EventArgs e)
        {
            if (megapixel1dot3.Checked)
            {
                resolutionWidth.Value = 1280;
                resolutionHeight.Value = 1024;
            }
        }

        private void megapixel4_CheckedChanged(object sender, EventArgs e)
        {
            if (megapixel4.Checked)
            {
                resolutionWidth.Value = 2448;
                resolutionHeight.Value = 1632;
            }
        }

        private void megapixel6_CheckedChanged(object sender, EventArgs e)
        {
            if (megapixel6.Checked)
            {
                resolutionWidth.Value = 3000;
                resolutionHeight.Value = 2000;
            }
        }

        private void megapixel8_CheckedChanged(object sender, EventArgs e)
        {
            if (megapixel8.Checked)
            {
                resolutionWidth.Value = 3456;
                resolutionHeight.Value = 2304;
            }
        }

        private void megapixel10_CheckedChanged(object sender, EventArgs e)
        {
            if (megapixel10.Checked)
            {
                resolutionWidth.Value = 3648;
                resolutionHeight.Value = 2736;
            }
        }

        private void megapixel12_CheckedChanged(object sender, EventArgs e)
        {
            if (megapixel12.Checked)
            {
                resolutionWidth.Value = 4000;
                resolutionHeight.Value = 3000;
            }
        }

        private void custom_CheckedChanged(object sender, EventArgs e)
        {
            resolutionWidth.Enabled = custom.Checked;
            resolutionHeight.Enabled = custom.Checked;
        }
    }
}
