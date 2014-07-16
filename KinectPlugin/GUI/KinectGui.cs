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
        CheckButton enableMotionButton;
        CheckButton showIkSkeleton;
        CheckButton showSensorSkeleton;
        TextBox statusLabel;

        KinectIkController ikController;
        KinectSensorManager sensorManager;
        KinectDebugVisualizer debugVisualizer;

        TexturePtr colorTexture;
        HardwarePixelBufferSharedPtr hwBuffer;
        PixelBox pixelBox;

        public KinectGui(KinectIkController ikController, KinectSensorManager sensorManager, KinectDebugVisualizer debugVisualizer)
            : base("KinectPlugin.GUI.KinectGui.layout")
        {
            this.ikController = ikController;
            ikController.AllowMovementChanged += ikController_AllowMovementChanged;
            this.sensorManager = sensorManager;
            sensorManager.StatusChanged += sensorManager_StatusChanged;
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

            statusLabel = (TextBox)window.findWidget("StatusLabel");
            statusLabel.Caption = sensorManager.CurrentStatus.ToString();

            sensorManager.SensorColorFrameReady += sensorManager_SensorColorFrameReady;
            //Create a texture, this is a rendertarget because nothing else will work (maybe not a good idea).
            colorTexture = TextureManager.getInstance().createManual("KinectColorSensor", "MyGUI", TextureType.TEX_TYPE_2D, 640, 480, 1, 1, PixelFormat.PF_X8R8G8B8, TextureUsage.TU_RENDERTARGET, false, 0);
            hwBuffer = colorTexture.Value.getBuffer();
            pixelBox = new PixelBox(0, 0, 640, 480, PixelFormat.PF_X8R8G8B8);
            ImageBox imageBox = (ImageBox)window.findWidget("ColorSensorImageBox");
            imageBox.setImageTexture(colorTexture.Value.getName());
            imageBox.setImageCoord(new IntCoord(0, 0, 640, 480));
        }

        unsafe void sensorManager_SensorColorFrameReady(byte[] obj)
        {
            fixed(byte* data = obj)
            {
                pixelBox.Data = data;
                hwBuffer.Value.blitFromMemory(pixelBox);
                pixelBox.Data = null;
            }
        }

        public override void Dispose()
        {
            sensorManager.StatusChanged -= sensorManager_StatusChanged;
            pixelBox.Dispose();
            hwBuffer.Dispose();
            colorTexture.Dispose();
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

        void sensorManager_StatusChanged(KinectSensorManager sensorManager)
        {
            statusLabel.Caption = sensorManager.CurrentStatus.ToString();
        }

        void ikController_AllowMovementChanged(KinectIkController ikController)
        {
            enableMotionButton.Checked = ikController.AllowMovement;
        }
    }
}
