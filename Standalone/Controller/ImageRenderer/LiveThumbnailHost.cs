using Engine;
using Medical.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public abstract class LiveThumbnailHost
    {
        public abstract IntCoord Coord { get; }

        public abstract void setTextureInfo(String name, IntCoord coord);

        internal LiveThumbnailHostInfo _HostInfo { get; set; }
    }
}
