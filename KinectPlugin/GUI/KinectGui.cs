using Engine;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Platform;
using Medical;
using Medical.GUI;
using MyGUIPlugin;
using OgreWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectPlugin
{
    class KinectGui : MDIDialog
    {
        static readonly int ColorIconSize = ScaleHelper.Scaled(32);

        CheckButton enableMotionButton;
        CheckButton showIkSkeleton;
        CheckButton showSensorSkeleton;
        CheckButton enableVideoFeed;
        TextBox statusLabel;
        ImageBox colorSensorImage;
        IntCoord colorSensorImageOriginalPos;
        IntCoord colorSensorImageIconPos;

        KinectIkController ikController;
        KinectSensorManager sensorManager;
        KinectDebugVisualizer debugVisualizer;

        TexturePtr colorTexture;
        HardwarePixelBufferSharedPtr hwBuffer;
        PixelBox pixelBox;

        public KinectGui(KinectIkController ikController, KinectSensorManager sensorManager, KinectDebugVisualizer debugVisualizer)
            : base("KinectPlugin.GUI.KinectGui.layout")
        {
            window.EventChangeCoord += window_EventChangeCoord;

            this.ikController = ikController;
            ikController.AllowMovementChanged += ikController_AllowMovementChanged;
            this.sensorManager = sensorManager;
            sensorManager.StatusChanged += sensorManager_StatusChanged;
            sensorManager.ColorFeedChanged += sensorManager_ColorFeedChanged;
            this.debugVisualizer = debugVisualizer;

            enableMotionButton = new CheckButton((Button)window.findWidget("EnableMotion"));
            enableMotionButton.Checked = ikController.AllowMovement;
            enableMotionButton.CheckedChanged += enableMotionButton_CheckedChanged;

            showIkSkeleton = new CheckButton((Button)window.findWidget("ShowIkSkeleton"));
            showIkSkeleton.Checked = ikController.DebugVisible;
            showIkSkeleton.CheckedChanged += showIkSkeleton_CheckedChanged;

            showSensorSkeleton = new CheckButton((Button)window.findWidget("ShowSensorSkeleton"));
            showSensorSkeleton.Checked = debugVisualizer.DebugVisible;
            showSensorSkeleton.CheckedChanged += showSensorSkeleton_CheckedChanged;

            enableVideoFeed = new CheckButton((Button)window.findWidget("EnableVideoFeed"));
            enableVideoFeed.Checked = sensorManager.UseColorFeed;
            enableVideoFeed.CheckedChanged += enableVideoFeed_CheckedChanged;

            statusLabel = (TextBox)window.findWidget("StatusLabel");
            setConnectedLabelText(sensorManager.Connected);

            colorSensorImage = (ImageBox)window.findWidget("ColorSensorImageBox");
            colorSensorImageOriginalPos = new IntCoord(colorSensorImage.Left, colorSensorImage.Top, colorSensorImage.Width, colorSensorImage.Height);
            colorSensorImageIconPos = new IntCoord(colorSensorImageOriginalPos.width / 2 - ColorIconSize / 2, colorSensorImageOriginalPos.height / 2 - ColorIconSize / 2, ColorIconSize, ColorIconSize);
            setColorImageIcon();
        }

        public override void Dispose()
        {
            sensorManager.StatusChanged -= sensorManager_StatusChanged;
            sensorManager.ColorFeedChanged -= sensorManager_ColorFeedChanged;
            destroyColorTexture();
            base.Dispose();
        }

        void enableMotionButton_CheckedChanged(Widget source, EventArgs e)
        {
            ikController.AllowMovement = enableMotionButton.Checked;
        }

        void showSensorSkeleton_CheckedChanged(Widget source, EventArgs e)
        {
            debugVisualizer.DebugVisible = showSensorSkeleton.Checked;
        }

        void showIkSkeleton_CheckedChanged(Widget source, EventArgs e)
        {
            ikController.DebugVisible = showIkSkeleton.Checked;
        }

        void enableVideoFeed_CheckedChanged(Widget source, EventArgs e)
        {
            sensorManager.UseColorFeed = enableVideoFeed.Checked;
        }

        void sensorManager_StatusChanged(KinectSensorManager sensorManager)
        {
            setConnectedLabelText(sensorManager.Connected);
            if (sensorManager.UseColorFeed)
            {
                if (sensorManager.Connected)
                {
                    createColorTexture();
                }
                else
                {
                    destroyColorTexture();
                }
            }
        }

        void sensorManager_ColorFeedChanged(KinectSensorManager obj)
        {
            if (sensorManager.HasColorFeed)
            {
                createColorTexture();
            }
            else
            {
                destroyColorTexture();
            }
        }

        private void setConnectedLabelText(bool connected)
        {
            statusLabel.Caption = connected ? "Connected" : "Disconnected";
        }

        void ikController_AllowMovementChanged(KinectIkController ikController)
        {
            enableMotionButton.Checked = ikController.AllowMovement;
        }

        private void createColorTexture()
        {
            if (colorTexture == null)
            {
                sensorManager.SensorColorFrameReady += sensorManager_SensorColorFrameReady;
                //This is an ok example of how to do video textuers, but its really only good enough for development right now
                //We are having to create the texture as a rendertarget when we should probably use a dynamic texture instead, however,
                //the D3D11 plugin will crash if we use any other kind of texture, so for now we are just using this.
                //If putting the texture in mygui it is important to tell its render manager to destroy the texture also (like the RocketWidget does).
                PixelFormat pixelFormat = PixelFormat.PF_BYTE_RGBA;
                IntSize2 colorFrameSize = sensorManager.ColorFrameSize;
                colorTexture = TextureManager.getInstance().createManual("KinectColorSensor", MyGUIInterface.Instance.CommonResourceGroup.FullName, TextureType.TEX_TYPE_2D, (uint)colorFrameSize.Width, (uint)colorFrameSize.Height, 1, 1, pixelFormat, TextureUsage.TU_RENDERTARGET, false, 0);
                hwBuffer = colorTexture.Value.getBuffer();
                pixelBox = new PixelBox(0, 0, colorFrameSize.Width, colorFrameSize.Height, pixelFormat);
                colorSensorImage.setItemResource(null); //Clear the "ItemResource" first since we are setting texture directly
                colorSensorImage.setCoord(colorSensorImageOriginalPos.left, colorSensorImageOriginalPos.top, colorSensorImageOriginalPos.width, colorSensorImageOriginalPos.height);
                colorSensorImage.setImageTexture(colorTexture.Value.getName());
                colorSensorImage.setImageCoord(new IntCoord(0, 0, colorFrameSize.Width, colorFrameSize.Height));
            }
        }

        private void destroyColorTexture()
        {
            if (colorTexture != null)
            {
                sensorManager.SensorColorFrameReady -= sensorManager_SensorColorFrameReady;
                setColorImageIcon(); //Because we set this to something with an actual texture, it will replace the texture set when created (that happens in the function called).
                RenderManager.Instance.destroyTexture(colorTexture.Value.getName());
                pixelBox.Dispose();
                hwBuffer.Dispose();
                colorTexture.Dispose();
                colorTexture = null;
            }
        }

        unsafe void sensorManager_SensorColorFrameReady(byte[] obj)
        {
            if (colorTexture != null)
            {
                fixed (byte* data = obj)
                {
                    pixelBox.Data = data;
                    hwBuffer.Value.blitFromMemory(pixelBox);
                    pixelBox.Data = null;
                }
            }
        }

        private void setColorImageIcon()
        {
            colorSensorImage.setItemResource("KinectPlugin.VideoStream");
            colorSensorImage.setCoord(colorSensorImageIconPos.left, colorSensorImageIconPos.top, colorSensorImageIconPos.width, colorSensorImageIconPos.height);
        }

        void window_EventChangeCoord(Widget source, EventArgs e)
        {
            sizeVideoOutput();
        }

        private void sizeVideoOutput()
        {
            IntCoord clientCoord = window.ClientCoord;
            int width = clientCoord.width;
            int height = (int)(width * (9.0f / 16.0f));
            if (height > clientCoord.height - 126)
            {
                height = clientCoord.height - 126;
                width = (int)(height * (16.0f / 9.0f));
            }
            if (colorTexture != null)
            {
                colorSensorImage.setSize(width, height);
            }
            colorSensorImageOriginalPos.width = width;
            colorSensorImageOriginalPos.height = height;
        }
    }
}
