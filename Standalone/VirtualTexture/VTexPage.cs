using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class VTexPage
    {
        public VTexPage(byte x, byte y, byte mip, byte indirectionTexId, IntSize2 numPages)
        {
            this.x = x;
            this.y = y;
            this.mip = mip;
            this.indirectionTexId = indirectionTexId;
            this.hashCode = 
                  (int)((x / (float)(numPages.Width >> mip)) * 16.0f + 0.5f) << 12
                + (int)((y / (float)(numPages.Width >> mip)) * 16.0f + 0.5f) << 8
                + mip << 4
                + (int)((indirectionTexId / 255.0f) * 16.0f + 0.5f);
            this.hashCode = x << 8 + y ^ mip << 8 + indirectionTexId;
        }

        public readonly byte x;
        public readonly byte y;
        public readonly byte mip;
        public readonly byte indirectionTexId;
        private readonly int hashCode;

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
            return hashCode;
        }
    }
}
