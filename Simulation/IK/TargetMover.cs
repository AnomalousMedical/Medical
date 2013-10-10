using Engine;
using Engine.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.IK
{
    class TargetMover : Behavior
    {
        enum Events
        {
            Left,
            Right,
            Up,
            Down,
            In,
            Out
        }

        static TargetMover()
        {
            MessageEvent left = new MessageEvent(Events.Left);
            left.addButton(KeyboardButtonCode.KC_A);
            DefaultEvents.registerDefaultEvent(left);

            MessageEvent right = new MessageEvent(Events.Right);
            right.addButton(KeyboardButtonCode.KC_D);
            DefaultEvents.registerDefaultEvent(right);

            MessageEvent up = new MessageEvent(Events.Up);
            up.addButton(KeyboardButtonCode.KC_W);
            DefaultEvents.registerDefaultEvent(up);

            MessageEvent down = new MessageEvent(Events.Down);
            down.addButton(KeyboardButtonCode.KC_S);
            DefaultEvents.registerDefaultEvent(down);

            MessageEvent inEvt = new MessageEvent(Events.In);
            inEvt.addButton(KeyboardButtonCode.KC_E);
            DefaultEvents.registerDefaultEvent(inEvt);

            MessageEvent outEvt = new MessageEvent(Events.Out);
            outEvt.addButton(KeyboardButtonCode.KC_Q);
            DefaultEvents.registerDefaultEvent(outEvt);
        }

        private const float velocity = 3.0f;

        public override void update(Clock clock, EventManager eventManager)
        {
            Vector3 updatedPos = Owner.Translation;
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
            if (eventManager[Events.In].Down)
            {
                updatedPos.z += velocity * clock.fSeconds;
            }
            if (eventManager[Events.Out].Down)
            {
                updatedPos.z -= velocity * clock.fSeconds;
            }
            updateTranslation(ref updatedPos);
        }
    }
}
