using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Engine;

namespace Medical.GUI
{
    public class CameraControls : Dialog
    {
        private const float ROTATE_MAX_VELOCITY = 0.0314f;
        private const float PAN_MAX_VELOCITY = 0.4f;
        private const float ZOOM_MAX_VELOCITY = 2.0f;

        private VirtualJoystick rotateJoystick;
        private VirtualJoystick panJoystick;
        private VirtualJoystick zoomJoystick;
        private SceneViewController sceneViewController;

        public CameraControls(SceneViewController sceneViewController)
            :base("Medical.GUI.CameraControls.CameraControls.layout")
        {
            this.sceneViewController = sceneViewController;
            rotateJoystick = new VirtualJoystick(window.findWidget("RotateJoystick"));
            rotateJoystick.PositionChanged += new JoystickEvent(rotateJoystick_PositionChanged);

            panJoystick = new VirtualJoystick(window.findWidget("PanJoystick"));
            panJoystick.PositionChanged += new JoystickEvent(panJoystick_PositionChanged);

            zoomJoystick = new VirtualJoystick(window.findWidget("ZoomJoystick"));
            zoomJoystick.PositionChanged += new JoystickEvent(zoomJoystick_PositionChanged);
            zoomJoystick.MaxDelta = new IntVector2(0, int.MaxValue);

            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);
        }

        public override void deserialize(ConfigFile configFile)
        {
            base.deserialize(configFile);
            fixJoysticks();
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            fixJoysticks();
        }

        private void fixJoysticks()
        {
            rotateJoystick.findZeroPosition();
            panJoystick.findZeroPosition();
            zoomJoystick.findZeroPosition();
        }

        void rotateJoystick_PositionChanged(VirtualJoystick joystick)
        {
            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            if (activeWindow != null)
            {
                Vector2 joyPos = joystick.Position;
                if (Math.Abs(joyPos.x) < 0.1)
                {
                    joyPos.x = 0;
                }
                else
                {
                    joyPos.x -= 0.1f;
                }
                if (Math.Abs(joyPos.y) < 0.15)
                {
                    joyPos.y = 0;
                }
                else
                {
                    joyPos.y -= 0.15f;
                }
                activeWindow.YawVelocity = joyPos.x * -ROTATE_MAX_VELOCITY;
                activeWindow.PitchVelocity = joyPos.y * ROTATE_MAX_VELOCITY;
            }
        }

        void panJoystick_PositionChanged(VirtualJoystick joystick)
        {
            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            if (activeWindow != null)
            {
                Vector2 joyPos = joystick.Position;
                if (Math.Abs(joyPos.x) < 0.1)
                {
                    joyPos.x = 0;
                }
                else
                {
                    joyPos.x -= 0.1f;
                }
                if (Math.Abs(joyPos.y) < 0.15)
                {
                    joyPos.y = 0;
                }
                else
                {
                    joyPos.y -= 0.15f;
                }
                activeWindow.PanVelocity = joyPos * PAN_MAX_VELOCITY;
            }
        }

        void zoomJoystick_PositionChanged(VirtualJoystick joystick)
        {
            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            if (activeWindow != null)
            {
                Vector2 joyPos = joystick.Position;
                if (Math.Abs(joyPos.y) < 0.1)
                {
                    joyPos.y = 0;
                }
                activeWindow.ZoomVelocity = joyPos.y * ZOOM_MAX_VELOCITY;
            }
        }
    }
}
