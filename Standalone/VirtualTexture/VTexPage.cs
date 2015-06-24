using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class VTexPage
    {
        public VTexPage(int x, int y, int mip, int indirectionTexId)
        {
            this.x = x;
            this.y = y;
            this.mip = mip;
            this.indirectionTexId = indirectionTexId;
        }

        public readonly int x;
        public readonly int y;
        public readonly int mip;
        public readonly int indirectionTexId;

        public static bool operator ==(VTexPage p1, VTexPage p2)
        {
            Object o1 = p1;
            Object o2 = p2;
            if(o1 == null || o2 == null)
            {
                return o1 == o2;
            }
            return p1.x == p2.x && p1.y == p2.y && p1.mip == p2.mip && p1.indirectionTexId == p2.indirectionTexId;
        }

        public static bool operator !=(VTexPage p1, VTexPage p2)
        {
            return !(p1 == p2);
        }

        public override bool Equals(object obj)
        {
            return obj is VTexPage && this == (VTexPage)obj;
        }

        public override string ToString()
        {
            return String.Format("{0}, {1} m: {2} id: {3}", x, y, mip, indirectionTexId);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode(); //Don't use this to index stuff
        }
    }
}
