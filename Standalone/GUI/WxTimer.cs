using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wx;
using Engine.Platform;

namespace Medical.GUI
{
    class WxTimer : UpdateTimer
    {
        class InternalTimer : Timer
        {
            private WxTimer timer;

            public InternalTimer(WxTimer timer)
            {
                this.timer = timer;
            }

            protected override void Notify()
            {
                timer.tick();
            }
        }

        private InternalTimer timer;

        Int64 deltaTime;
        Int64 totalTime = 0;
        Int64 frameStartTime;
        Int64 lastTime;
        Int64 totalFrameTime;

        public WxTimer(SystemTimer systemTimer)
            :base(systemTimer)
        {
            timer = new InternalTimer(this);
        }

        public override bool startLoop()
        {
            if (!systemTimer.initialize())
            {
                return false;
            }

            lastTime = systemTimer.getCurrentTime();
            fireLoopStarted();
            return timer.start(0, false);
        }

        public void stop()
        {
            timer.stop();
        }

        public void tick()
        {
            frameStartTime = systemTimer.getCurrentTime();
            deltaTime = frameStartTime - lastTime;

            if (deltaTime > maxDelta)
            {
                deltaTime = maxDelta;
                fireExceededMaxDelta();
            }
            totalTime += deltaTime;
            if (totalTime > fixedFrequency * maxFrameSkip)
            {
                totalTime = fixedFrequency * maxFrameSkip;
            }

            //Frame skipping
            while (totalTime >= fixedFrequency)
            {
                fireFixedUpdate(fixedFrequency);
                totalTime -= fixedFrequency;
            }

            fireFullSpeedUpdate(deltaTime);

            lastTime = frameStartTime;

            //cap the framerate if required
            totalFrameTime = systemTimer.getCurrentTime() - frameStartTime;
            //while (totalFrameTime < framerateCap)
            //{
            //    Thread.Sleep((int)((framerateCap - totalFrameTime) / 1000));
            //    totalFrameTime = systemTimer.getCurrentTime() - frameStartTime;
            //}
        }
    }
}
