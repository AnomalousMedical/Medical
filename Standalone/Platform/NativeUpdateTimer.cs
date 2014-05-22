using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;

namespace Medical
{
    public class NativeUpdateTimer : UpdateTimer
    {
        Int64 deltaTime;
        Int64 fixedFrameTime = 0;
        Int64 fullSpeedUpdateAccumulator;
        Int64 frameStartTime;
        Int64 lastTime;
        Int64 totalFrameTime;

        public NativeUpdateTimer(SystemTimer systemTimer)
            :base(systemTimer)
        {
            
        }

        public bool BatterySaver { get; set; }

        public override bool startLoop()
        {
            if (!systemTimer.initialize())
            {
                return false;
            }

            fireLoopStarted();

            deltaTime = 0;
            fixedFrameTime = 0;
            frameStartTime = 0;
            lastTime = systemTimer.getCurrentTime();
            fullSpeedUpdateAccumulator = 0;
            totalFrameTime = 0;

            started = true;

            return true;
        }

        public void OnIdle()
        {
            if (started)
            {
                frameStartTime = systemTimer.getCurrentTime();
                deltaTime = frameStartTime - lastTime;

                if (deltaTime > maxDelta)
                {
                    deltaTime = maxDelta;
                    fireExceededMaxDelta();
                }
                fixedFrameTime += deltaTime;
                if (fixedFrameTime > fixedFrequency * maxFrameSkip)
                {
                    fixedFrameTime = fixedFrequency * maxFrameSkip;
                }

                //Frame skipping
                while (fixedFrameTime >= fixedFrequency)
                {
                    fireFixedUpdate(fixedFrequency);
                    fixedFrameTime -= fixedFrequency;
                }

                fullSpeedUpdateAccumulator += deltaTime;

                if(fullSpeedUpdateAccumulator > framerateCap)
                {
                    fireFullSpeedUpdate(fullSpeedUpdateAccumulator);
                    fullSpeedUpdateAccumulator = 0;
                }
                else if(BatterySaver)
                {
                    //cap the framerate if required
                    PerformanceMonitor.start("Battery Saver");
                    totalFrameTime = systemTimer.getCurrentTime() - frameStartTime;
                    while (totalFrameTime < framerateCap)
                    {
                        long sleepTime = framerateCap - totalFrameTime;
                        System.Threading.Thread.Sleep((int)(sleepTime / 1000));
                        totalFrameTime = systemTimer.getCurrentTime() - frameStartTime;
                    }
                    PerformanceMonitor.stop("Battery Saver");
                }

                lastTime = frameStartTime;
            }
            else
            {
                startLoop();
            }
        }

        /// <summary>
        /// Reset the last time to be the current time. Call after a long delay to avoid frame skipping.
        /// </summary>
        public override void resetLastTime()
        {
            frameStartTime = systemTimer.getCurrentTime();
        }
    }
}
