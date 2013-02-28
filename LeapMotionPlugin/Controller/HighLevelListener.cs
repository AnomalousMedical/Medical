using Engine;
using Engine.Platform;
using Leap;
using Medical.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;

namespace LeapMotionPlugin
{
    class HighLevelListener : Listener
    {
        enum Events
        {
            Rotate,
            Pan,
            Zoom
        }

        static HighLevelListener()
        {
            MessageEvent rotateEvent = new MessageEvent(Events.Rotate);
            rotateEvent.addButton(KeyboardButtonCode.KC_LSHIFT);
            DefaultEvents.registerDefaultEvent(rotateEvent);

            MessageEvent panEvent = new MessageEvent(Events.Pan);
            panEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            DefaultEvents.registerDefaultEvent(panEvent);

            MessageEvent zoomEvent = new MessageEvent(Events.Zoom);
            zoomEvent.addButton(KeyboardButtonCode.KC_LMENU);
            DefaultEvents.registerDefaultEvent(zoomEvent);
        }

        Frame lastFrame;
        SceneViewController sceneViewController;
        EventManager eventManager;

        const float XDampingStart = -100;
        const float XDampingMax = -20;
        const float XDampingAccel = 5f;

        const float YDampingStart = -100;
        const float YDampingMax = -15;
        const float YDampingAccel = 5;

        const float ZDampingStart = -100;
        const float ZDampingMax = -1;
        const float ZDamingDir = 1;

        const float moveThreashold = 0.025f;

        Vector3 damping = new Vector3(XDampingStart, YDampingStart, ZDampingStart);

        public HighLevelListener(SceneViewController sceneViewController, EventManager eventManager)
        {
            this.sceneViewController = sceneViewController;
            this.eventManager = eventManager;
        }

        public override void OnFrame(Controller controller)
        {
            Frame frame = controller.Frame();
            if (lastFrame != null)
            {
                Vector3 frameTrans = MathHelper.GetVector3(frame.Translation(lastFrame));
                SceneViewWindow active = sceneViewController.ActiveWindow;
                float deltaTime = (frame.Timestamp - lastFrame.Timestamp) / 1000000.0f;
                ThreadManager.invoke(new Action(() =>
                    {
                        if (active != null)
                        {
                            if (eventManager[Events.Rotate].Down)
                            {
                                //Logging.Log.Debug("{0}, {1}", frameTrans.x, frameTrans.y);
                                //active.zoom(-frameTrans.z);
                                bool moving = !frameTrans.x.EpsilonEquals(0, moveThreashold) && !frameTrans.y.EpsilonEquals(0, moveThreashold);
                                if (moving)
                                {
                                    active.rotate(frameTrans.x / damping.x, frameTrans.y / damping.y);
                                }
                                damping.x = calculateDamping(damping.x, moving, XDampingStart, XDampingMax, XDampingAccel, deltaTime);
                                damping.y = calculateDamping(damping.y, moving, YDampingStart, YDampingMax, YDampingAccel, deltaTime);
                            }
                            if (eventManager[Events.Pan].Down)
                            {
                                active.pan(frameTrans.x / 10, frameTrans.y / -10);
                                //active.zoom(-frameTrans.z);
                            }
                            //if (eventManager[Events.Zoom].Down)
                            //{
                                
                            //}
                        }
                    }));

                lastFrame.Dispose();
            }

            lastFrame = frame;
        }

        private float calculateDamping(float current, bool acceleration, float min, float max, float accel, float deltaTime)
        {
            if (acceleration)
            {
                float newDamping = current + accel * deltaTime;
                if (newDamping > max)
                {
                    newDamping = max;
                }
                return max;
            }
            else
            {
                float newDamping = current - accel * deltaTime;
                if (newDamping < min)
                {
                    newDamping = min;
                }
                return max;
            }
        }
    }
}
