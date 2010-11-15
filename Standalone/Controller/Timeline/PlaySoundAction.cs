using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using SoundPlugin;

namespace Medical
{
    [TimelineActionProperties("Play Sound", 128 / 255f, 0 / 255f, 255 / 255f)]
    class PlaySoundAction : TimelineAction
    {
        private bool finished = false;
        private Source source;

        public override void started(float timelineTime, Clock clock)
        {
            finished = false;
            source = TimelineController.playSound("S:/junk/TestOpenAL/shortsong.ogg");
            if (source != null)
            {
                source.PlaybackFinished += source_PlaybackFinished;
            }
            else
            {
                finished = true;
            }
        }

        void source_PlaybackFinished(Source source)
        {
            source.PlaybackFinished -= source_PlaybackFinished;
            finished = true;
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            
        }

        public override void editing()
        {
            
        }

        public override bool Finished
        {
            get
            {
                return finished;
            }
        }
    }
}
