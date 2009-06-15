using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public abstract class PlaybackState
    {
        #region Constructors

        public PlaybackState(float startTime)
        {
            StartTime = startTime;
        }

        #endregion Constructors

        #region Functions

        public PlaybackState blend(float time)
        {
            if (time < StartTime)
            {
                if (Previous != null)
                {
                    return Previous.blend(time);
                }
                else
                {
                    return this;
                }
            }
            else if (time > StopTime)
            {
                if (Next != null)
                {
                    return Next.blend(time);
                }
                else
                {
                    return this;
                }
            }
            else
            {
                if (Next != null)
                {
                    doBlend((time - StartTime) / (StopTime - StartTime));
                }
                return this;
            }
        }

        protected abstract void doBlend(float percent);

        public abstract void update();

        #endregion Functions

        #region Properties

        public float StartTime { get; set; }

        public float StopTime
        {
            get
            {
                if (Next != null)
                {
                    return Next.StartTime;
                }
                else
                {
                    return float.MaxValue;
                }
            }
        }

        public abstract PlaybackState Previous { get; }

        public abstract PlaybackState Next { get;}

        #endregion Properties
    }
}
