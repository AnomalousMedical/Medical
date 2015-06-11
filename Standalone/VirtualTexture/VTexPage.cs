using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    struct VTexPage
    {
        public VTexPage(int x, int y, int mip, int indirectionTexId)
        {
            this.x = x;
            this.y = y;
            this.mip = mip;
            this.indirectionTexId = indirectionTexId;
        }

        public int x;
        public int y;
        public int mip;
        public int indirectionTexId;

        public static bool operator ==(VTexPage p1, VTexPage p2)
        {
            return p1.x == p2.x && p1.y == p2.y && p1.mip == p2.mip && p1.indirectionTexId == p2.indirectionTexId;
        }

        public static bool operator !=(VTexPage p1, VTexPage p2)
        {
            return !(p1.x == p2.x && p1.y == p2.y && p1.mip == p2.mip && p1.indirectionTexId == p2.indirectionTexId);
        }

        public override bool Equals(object obj)
        {
            return obj is VTexPage && this == (VTexPage)obj;
        }
    }
}
