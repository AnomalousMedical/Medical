using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public delegate void JoystickEvent(VirtualJoystick joystick);

    public class VirtualJoystick
    {
        public event JoystickEvent PositionChanged;

        private Widget joystickWidget;
        private Widget joystickParent;
        private IntVector2 clickStartPos;
        private IntVector2 joystickStartPos;
        private int travelRadius = 50;
        private int travelRadiusSquared;
        private int deadZoneRadius = 10;
        private int deadZoneRadiusSquared;
        private int xLimit = int.MaxValue;
        private int yLimit = int.MaxValue;
        private Vector2 joystickPosition = new Vector2(0.0f, 0.0f);

        public VirtualJoystick(Widget joystickWidget)
        {
            travelRadiusSquared = travelRadius * travelRadius;
            deadZoneRadiusSquared = deadZoneRadius * deadZoneRadius;
            this.joystickWidget = joystickWidget;
            joystickWidget.MouseButtonPressed += new MyGUIEvent(joystickWidget_MouseButtonPressed);
            joystickWidget.MouseButtonReleased += new MyGUIEvent(joystickWidget_MouseButtonReleased);
            joystickWidget.MouseDrag += new MyGUIEvent(joystickWidget_MouseDrag);

            joystickStartPos = new IntVector2(joystickWidget.Left, joystickWidget.Top);

            joystickParent = joystickWidget.Parent;
            joystickParent.MouseDrag += new MyGUIEvent(joystickParent_MouseDrag);
            joystickParent.MouseButtonReleased += new MyGUIEvent(joystickParent_MouseButtonReleased);
            joystickParent.MouseButtonPressed += new MyGUIEvent(joystickParent_MouseButtonPressed);
        }

        public Vector2 Position
        {
            get
            {
                return joystickPosition;
            }
        }

        private IntVector2 moveJoystick(IntVector2 newPos)
        {
            IntVector2 delta = newPos - joystickStartPos;
            if (delta.length2() > travelRadiusSquared)
            {
                Vector2 dirVec = delta.normalized();
                Vector2 maxPos = joystickStartPos + (dirVec * travelRadius);
                newPos = new IntVector2((int)maxPos.x, (int)maxPos.y);
            }

            joystickWidget.setPosition(newPos.x, newPos.y);
            computeJoystickValues();
            return newPos;
        }

        private void centerJoystick()
        {
            joystickWidget.setPosition(joystickStartPos.x, joystickStartPos.y);
            computeJoystickValues();
        }

        private void computeJoystickValues()
        {
            Vector2 buttonPos = new Vector2(joystickWidget.Left, joystickWidget.Top);
            joystickPosition = buttonPos - joystickStartPos;
            joystickPosition /= travelRadius;
            if (PositionChanged != null)
            {
                PositionChanged.Invoke(this);
            }
        }

        void joystickParent_MouseDrag(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            IntVector2 newPos = new IntVector2(me.Position.x - joystickParent.AbsoluteLeft - joystickWidget.Width / 2, me.Position.y - joystickParent.AbsoluteTop - joystickWidget.Height / 2);
            newPos = moveJoystick(newPos);
        }

        void joystickParent_MouseButtonPressed(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            IntVector2 newPos = new IntVector2(me.Position.x - joystickParent.AbsoluteLeft - joystickWidget.Width / 2, me.Position.y - joystickParent.AbsoluteTop - joystickWidget.Height / 2);
            newPos = moveJoystick(newPos);
        }

        void joystickParent_MouseButtonReleased(Widget source, EventArgs e)
        {
            centerJoystick();
        }

        void joystickWidget_MouseDrag(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            IntVector2 newPos = new IntVector2(me.Position.x - joystickParent.AbsoluteLeft - clickStartPos.x, me.Position.y - joystickParent.AbsoluteTop - clickStartPos.y);
            newPos = moveJoystick(newPos);
        }

        void joystickWidget_MouseButtonReleased(Widget source, EventArgs e)
        {
            centerJoystick();
        }

        void joystickWidget_MouseButtonPressed(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            clickStartPos = new IntVector2((int)(me.Position.x - joystickWidget.AbsoluteLeft), (int)(me.Position.y - joystickWidget.AbsoluteTop));
        }
    }
}
