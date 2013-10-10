using Engine;
using Engine.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.IK
{
    class BaseMover : Behavior
    {
        enum Events
        {
            Left,
            Right,
            Up,
            Down,
            RotLeft,
            RotRight
        }

        static BaseMover()
        {
            MessageEvent left = new MessageEvent(Events.Left);
            left.addButton(KeyboardButtonCode.KC_LEFT);
            DefaultEvents.registerDefaultEvent(left);

            MessageEvent right = new MessageEvent(Events.Right);
            right.addButton(KeyboardButtonCode.KC_RIGHT);
            DefaultEvents.registerDefaultEvent(right);

            MessageEvent up = new MessageEvent(Events.Up);
            up.addButton(KeyboardButtonCode.KC_UP);
            DefaultEvents.registerDefaultEvent(up);

            MessageEvent down = new MessageEvent(Events.Down);
            down.addButton(KeyboardButtonCode.KC_DOWN);
            DefaultEvents.registerDefaultEvent(down);

            MessageEvent rotLeft = new MessageEvent(Events.RotLeft);
            rotLeft.addButton(KeyboardButtonCode.KC_N);
            DefaultEvents.registerDefaultEvent(rotLeft);

            MessageEvent rotRight = new MessageEvent(Events.RotRight);
            rotRight.addButton(KeyboardButtonCode.KC_M);
            DefaultEvents.registerDefaultEvent(rotRight);
        }

        private const float velocity = 3.0f;

        public override void update(Clock clock, EventManager eventManager)
        {
            Vector3 updatedPos = Owner.Translation;
            Quaternion updatedRot = Owner.Rotation;
            if (eventManager[Events.Left].Down)
            {
                updatedPos.x -= velocity * clock.fSeconds;
            }
            if (eventManager[Events.Right].Down)
            {
                updatedPos.x += velocity * clock.fSeconds;
            }
            if (eventManager[Events.Down].Down)
            {
                updatedPos.y -= velocity * clock.fSeconds;
            }
            if (eventManager[Events.Up].Down)
            {
                updatedPos.y += velocity * clock.fSeconds;
            }
            if (eventManager[Events.RotLeft].Down)
            {
                updatedRot *= new Quaternion(0f, velocity * clock.fSeconds, 0f);
            }
            if (eventManager[Events.RotRight].Down)
            {
                updatedRot *= new Quaternion(0f, -velocity * clock.fSeconds, 0f);
            }
            updatePosition(ref updatedPos, ref updatedRot);
        }
    }
}
