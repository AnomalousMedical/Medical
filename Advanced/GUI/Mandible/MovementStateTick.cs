using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Muscles;

namespace Medical.GUI
{
    class MovementStateTick : TrackBarMark
    {
        private MovementSequenceState state;

        public MovementStateTick(MovementSequenceState state)
        {
            this.state = state;
        }

        public override float Location
        {
            get
            {
                return state.StartTime;
            }
            set
            {
                state.StartTime = value;
            }
        }
    }
}
