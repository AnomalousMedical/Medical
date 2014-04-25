using Engine;
using Medical.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public interface LiveThumbnailHost
    {
        IntCoord Coord { get; }

        void setTextureInfo(String name, IntCoord coord);
    }
}
