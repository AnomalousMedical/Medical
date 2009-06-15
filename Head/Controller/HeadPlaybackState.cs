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

        public void insert(HeadPlaybackState state)
        {
            if (next == null)
            {
                state.previous = this;
                next = state;
            }
            else if (state.StartTime > StartTime && state.StartTime < StopTime)
            {
                next.previous = state;
                state.next = next;

                state.previous = this;
                next = state;
            }
            else
            {
                next.insert(state);
            }
        }

        public override PlaybackState Previous
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
    }
}
