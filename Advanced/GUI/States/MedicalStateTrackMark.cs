using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class MedicalStateTrackMark : TrackBarMark
    {
        public MedicalStateTrackMark(MedicalState state)
        {
            State = state;
        }

        public MedicalState State { get; set; }
    }
}
