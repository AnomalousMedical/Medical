using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Logging;

namespace Medical
{
    public delegate void PlaybackTimeChanged(float time);

    public delegate void StartStateChanged(PlaybackState startState);

    public class PlaybackController : UpdateListener
    {
        public event PlaybackTimeChanged PlaybackTimeChanged;
        public event StartStateChanged StartStateChanged;

        private PlaybackState startState;
        private PlaybackState currentState;
        private float currentTime = 0.0f;
        private UpdateTimer timer;
        private bool playing = false;

        public PlaybackController(UpdateTimer timer)
        {
            this.timer = timer;
        }

        public void setTime(float time)
        {
            currentTime = time;
            if (currentState != null)
            {
                currentState = currentState.blend(time);
            }
            fireTimeChange();
        }

        public void startPlayback()
        {
            if (!playing && currentState != null)
            {
                timer.addFixedUpdateListener(this);
                playing = true;
            }
        }

        public void stopPlayback()
        {
            if (playing)
            {
                timer.removeFixedUpdateListener(this);
                playing = false;
                setTime(0.0f);
            }
        }

        internal void pausePlayback()
        {
            if (playing)
            {
                timer.removeFixedUpdateListener(this);
                playing = false;
            }
        }

        public void exceededMaxDelta()
        {
            
        }

        public void loopStarting()
        {
            
        }

        public void sendUpdate(Clock clock)
        {
            currentTime += (float)clock.Seconds;
            currentState = currentState.blend(currentTime);
            fireTimeChange();
        }

        public PlaybackState StartState
        {
            get
            {
                return startState;
            }
            set
            {
                startState = value;
                currentState = startState;
                fireStartStateChanged();
            }
        }

        private void fireTimeChange()
        {
            if (PlaybackTimeChanged != null)
            {
                PlaybackTimeChanged.Invoke(currentTime);
            }
        }

        private void fireStartStateChanged()
        {
            if (StartStateChanged != null)
            {
                StartStateChanged.Invoke(startState);
            }
        }
    }
}
