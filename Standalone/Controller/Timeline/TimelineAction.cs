using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    interface TimelineAction
    {
        void started(float timelineTime, Clock clock);

        void stopped(float timelineTime, Clock clock);

        void update(float timelineTime, Clock clock);

        float StartTime { get; }

        bool Finished { get; }
    }
}
