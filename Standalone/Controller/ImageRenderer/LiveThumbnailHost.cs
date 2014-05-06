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
        public abstract void setTextureInfo(String name, IntCoord coord);

        public LayerState Layers { get; set; }

        public Vector3 Translation { get; set; }

        public Vector3 LookAt { get; set; }

        internal LiveThumbnailHostInfo _HostInfo { get; set; }
    }
}
