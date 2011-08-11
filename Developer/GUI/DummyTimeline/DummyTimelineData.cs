using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Developer
{
    class DummyTimelineData : TimelineData
    {
        public DummyTimelineData()
        {

        }

        public DummyTimelineData(float startTime, float duration)
        {
            _Duration = duration;
            _StartTime = startTime;
        }

        public override string Track
        {
            get { return "Dummy"; }
        }

        public override float _Duration { get; set; }

        public override float _StartTime { get; set; }
    }
}
