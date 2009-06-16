using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.Animation
{
    class KeyFrameMark
    {
        public KeyFrameMark(int time)
        {
            this.Time = time;
        }

        public int Time { get; set; }
    }
}
