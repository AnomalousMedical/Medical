using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    interface SceneControlWidget : IDisposable
    {
        IntVector2 Position { get; set; }

        SceneAnatomyControl SceneAnatomyControl { get; }

        bool Visible { get; set; }
    }
}
