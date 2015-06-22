using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class PTexPage
    {
        public PTexPage(int x, int y, int pageX, int pageY)
        {
            this.x = x;
            this.y = y;
            this.pageX = pageX;
            this.pageY = pageY;
        }

        public readonly int x;
        public readonly int y;
        public readonly int pageX;
        public readonly int pageY;

        public VTexPage VirtualTexturePage { get; set; }
    }
}
