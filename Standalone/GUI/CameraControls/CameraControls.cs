using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;

namespace Medical.GUI
{
    public class CameraControls : Dialog
    {
        private const float MAX_VELOCITY = 0.0314f;

        private VirtualJoystick rotateJoystick;
        private SceneViewController sceneViewController;

        public CameraControls(SceneViewController sceneViewController)
            :base("Medical.GUI.CameraControls.CameraControls.layout")
        {
            this.sceneViewController = sceneViewController;
            rotateJoystick = new VirtualJoystick(window.findWidget("RotateJoystick"));
            rotateJoystick.PositionChanged += new JoystickEvent(rotateJoystick_PositionChanged);
        }

        void rotateJoystick_PositionChanged(VirtualJoystick joystick)
        {
            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            if (activeWindow != null)
            {
                activeWindow.YawVelocity = joystick.Position.x * -MAX_VELOCITY;
                activeWindow.PitchVelocity = joystick.Position.y * MAX_VELOCITY;
            }
        }
    }
}
