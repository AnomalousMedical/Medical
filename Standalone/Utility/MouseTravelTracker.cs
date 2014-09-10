using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class MouseTravelTracker
    {
        private const int MoveGracePixels = 3;

        private IntVector2 travelDistance;

        public MouseTravelTracker()
        {

        }

        public void reset()
        {
            travelDistance = new IntVector2(0, 0);
        }

        public void traveled(IntVector3 travelAmount)
        {
            travelDistance.x += Math.Abs(travelAmount.x);
            travelDistance.y += Math.Abs(travelAmount.y);
        }

        public void traveled(int x, int y)
        {
            travelDistance.x += Math.Abs(x);
            travelDistance.y += Math.Abs(y);
        }

        public bool TraveledOverLimit
        {
            get
            {
                return travelDistance.x > MoveGracePixels || travelDistance.y > MoveGracePixels;
            }
        }
    }
}
