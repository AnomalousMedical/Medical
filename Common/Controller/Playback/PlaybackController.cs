using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Logging;

namespace Medical
{
    public class PlaybackController : UpdateListener
    {
        private PlaybackState startState;
        private PlaybackState currentState;
        private float currentTime = 0.0f;
        private UpdateTimer timer;
        private bool playing = false;

        public PlaybackController(UpdateTimer timer)
        {
            this.timer = timer;
        }

        public void setCurrentState(PlaybackState current)
        {
            this.currentState = current;
            currentState = currentState.blend(currentState.StartTime);
        }

        public void setTime(float time)
        {
            currentTime = time;
            currentState = currentState.blend(time);
        }

        public void startPlayback(float time)
        {
            setTime(time);
            if (!playing)
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
            }
        }
    }
}
