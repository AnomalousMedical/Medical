using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    class HeadPlaybackState : PlaybackState
    {
        private HeadPlaybackState previous;
        private HeadPlaybackState next;
        private BoneManipulatorState boneState;

        public HeadPlaybackState(float startTime)
            :base(startTime)
        {

        }

        protected override void doBlend(float percent)
        {
            boneState.blend(next.boneState, percent);
        }

        public override void update()
        {
            boneState = BoneManipulatorController.createBoneManipulatorState();
        }

        public override PlaybackState Previous
        {
            get
            {
                return previous;
            }
        }

        public HeadPlaybackState HeadPrevious
        {
            get
            {
                return previous;
            }
        }

        public override PlaybackState Next
        {
            get
            {
                return next;
            }
        }

        public HeadPlaybackState HeadNext
        {
            get
            {
                return next;
            }
            set
            {
                next = value;
                next.previous = this;
            }
        }

        public HeadPlaybackState Last
        {
            get
            {
                if (next == null)
                {
                    return this;
                }
                else
                {
                    return next.Last;
                }
            }
        }
    }
}
