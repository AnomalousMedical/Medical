using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using SoundPlugin;
using Engine.Saving;

namespace Medical
{
    [TimelineActionProperties("Play Sound", 128 / 255f, 0 / 255f, 255 / 255f, GUIType=typeof(Medical.GUI.PlaySoundProperties))]
    class PlaySoundAction : TimelineAction
    {
        private bool finished = false;
        private Source source;
        private String soundFile = null;

        public PlaySoundAction()
        {

        }

        public PlaySoundAction(String soundFile)
        {
            this.soundFile = soundFile;
        }

        public override void started(float timelineTime, Clock clock)
        {
            if (soundFile != null)
            {
                finished = false;
                source = TimelineController.playSound(soundFile);
                if (source != null)
                {
                    source.PlaybackFinished += source_PlaybackFinished;
                }
                else
                {
                    finished = true;
                }
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
            if (source != null)
            {
                source.stop();
            }
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

        public override float Duration
        {
            get
            {
                if (soundFile != null)
                {
                    return (float)TimelineController.getSoundDuration(soundFile);
                }
                return 0.0f;
            }
            set
            {
                
            }
        }

        public String SoundFile
        {
            get
            {
                return soundFile;
            }
            set
            {
                soundFile = value;
            }
        }

        #region Saving

        private const String SOUND_FILE = "SoundFile";

        protected PlaySoundAction(LoadInfo info)
            : base(info)
        {
            SoundFile = info.GetString(SOUND_FILE, null);
        }

        public override void getInfo(SaveInfo info)
        {
            info.AddValue(SOUND_FILE, SoundFile);
            base.getInfo(info);
        }

        #endregion
    }
}
