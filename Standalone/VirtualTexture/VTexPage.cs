using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    struct VTexPage
    {
        public VTexPage(int x, int y, int mip)
        {
            this.x = x;
            this.y = y;
            this.mip = mip;
        }

        public int x;
        public int y;
        public int mip;

        public static bool operator ==(VTexPage p1, VTexPage p2)
        {
            return p1.x == p2.x && p1.y == p2.y && p1.mip == p2.mip;
        }

        public static bool operator !=(VTexPage p1, VTexPage p2)
        {
            return !(p1.x == p2.x && p1.y == p2.y && p1.mip == p2.mip);
        }

        public override bool Equals(object obj)
        {
            return obj is VTexPage && this == (VTexPage)obj;
        }
    }
}
