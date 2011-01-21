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
        private Vector2 joystickPosition = new Vector2(0.0f, 0.0f);
        private IntVector2 maxDelta = new IntVector2(int.MaxValue, int.MaxValue);

        public VirtualJoystick(Widget joystickWidget)
        {
            travelRadiusSquared = travelRadius * travelRadius;
            this.joystickWidget = joystickWidget;
            joystickWidget.MouseButtonPressed += new MyGUIEvent(joystickWidget_MouseButtonPressed);
            joystickWidget.MouseButtonReleased += new MyGUIEvent(joystickWidget_MouseButtonReleased);
            joystickWidget.MouseDrag += new MyGUIEvent(joystickWidget_MouseDrag);

            joystickParent = joystickWidget.Parent;
            joystickParent.MouseDrag += new MyGUIEvent(joystickParent_MouseDrag);
            joystickParent.MouseButtonReleased += new MyGUIEvent(joystickParent_MouseButtonReleased);
            joystickParent.MouseButtonPressed += new MyGUIEvent(joystickParent_MouseButtonPressed);

            findZeroPosition();
        }

        public void findZeroPosition()
        {
            joystickStartPos = new IntVector2(joystickParent.Width / 2 - joystickWidget.Width / 2,
                                              joystickParent.Height / 2 - joystickWidget.Height / 2);
        }

        public Vector2 Position
        {
            get
            {
                return joystickPosition;
            }
        }

        public IntVector2 MaxDelta
        {
            get
            {
                return maxDelta;
            }
            set
            {
                maxDelta = value;
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
            int direction = 1;
            if (delta.x < 0.0f)
            {
                delta.x *= -1;
                direction = -1;
            }
            if (delta.x > maxDelta.x)
            {
                newPos.x = joystickStartPos.x + maxDelta.x * direction;
            }
            direction = 1;
            if (delta.y < 0.0f)
            {
                delta.y *= -1;
                direction = -1;
            }
            if (delta.y > maxDelta.y)
            {
                newPos.y = joystickStartPos.y + maxDelta.y;
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
