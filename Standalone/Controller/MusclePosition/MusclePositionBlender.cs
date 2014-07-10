using Engine.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class MusclePositionBlender
    {
        private SubscribingUpdateListener updateListener;
        private long blendPositionMicro;
        private long durationMicro;

        private MusclePosition start;
        private MusclePosition end;

        public MusclePositionBlender(UpdateTimer timer)
        {
            updateListener = new SubscribingUpdateListener(timer);
            updateListener.OnUpdate += updateListener_OnUpdate;
        }

        /// <summary>
        /// Start blending between two states. The duration is in seconds.
        /// </summary>
        /// <param name="start">The starting muscle position.</param>
        /// <param name="end">The ending muscle position.</param>
        /// <param name="duration">The duration to blend.</param>
        public void startBlend(MusclePosition start, MusclePosition end, float duration)
        {
            this.start = start;
            this.end = end;
            updateListener.subscribeToUpdates();
            blendPositionMicro = 0;
            durationMicro = Clock.SecondsToMicroseconds(duration);
        }

        void updateListener_OnUpdate(Clock clock)
        {
            blendPositionMicro += clock.DeltaTimeMicro;
            if (blendPositionMicro < durationMicro)
            {
                float percent = blendPositionMicro / (float)durationMicro;
                start.blend(end, percent);
            }
            else
            {
                end.blend(end, 1.0f);
                updateListener.unsubscribeFromUpdates();
            }
        }
    }
}
