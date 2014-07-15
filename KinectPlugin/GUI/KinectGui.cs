using Engine;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Platform;
using Medical;
using Medical.GUI;
using MyGUIPlugin;
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

        public KinectGui(KinectIkController ikController, KinectSensorManager sensorManager, KinectDebugVisualizer debugVisualizer)
            : base("KinectPlugin.GUI.KinectGui.layout")
        {
            this.ikController = ikController;
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
        }

        public override void Dispose()
        {
            sensorManager.StatusChanged -= sensorManager_StatusChanged;
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
    }
}
