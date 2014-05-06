using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class AnatomyContextWindowLiveThumbHost : LiveThumbnailHost
    {
        AnatomyContextWindow window;

        public AnatomyContextWindowLiveThumbHost(AnatomyContextWindow window)
        {
            this.window = window;
        }

        public override void setTextureInfo(string name, IntCoord coord)
        {
            window.setTextureInfo(name, coord);
        }
    }
}
