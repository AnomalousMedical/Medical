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
        Int64 frameStartTime;
        Int64 lastTime;
        Int64 totalFrameTime;

        public NativeUpdateTimer(SystemTimer systemTimer)
            :base(systemTimer)
        {
            
        }

        public override bool startLoop()
        {
            if (!systemTimer.initialize())
            {
                return false;
            }
            systemTimer.Accurate = framerateCap > 0;

            fireLoopStarted();

            deltaTime = 0;
            frameStartTime = 0;
            lastTime = systemTimer.getCurrentTime();
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

                fireUpdate(frameStartTime, deltaTime);
                
                //cap the framerate if required
                PerformanceMonitor.start("Energy Saver");
                totalFrameTime = systemTimer.getCurrentTime() - frameStartTime;
                while (totalFrameTime < framerateCap)
                {
                    long sleepTime = framerateCap - totalFrameTime;
                    int sleepMs = (int)(sleepTime / 1000);
                    if (sleepMs > 0)
                    {
                        System.Threading.Thread.Sleep(sleepMs);
                    }
                    totalFrameTime = systemTimer.getCurrentTime() - frameStartTime;
                }
                PerformanceMonitor.stop("Energy Saver");

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
