﻿using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Movement.GUI
{
    interface SceneControlWidget : IDisposable
    {
        IntVector2 Position { get; set; }

        SceneAnatomyControl SceneAnatomyControl { get; }

        void setVisibleTypes(SceneAnatomyControlType visibleTypes);

        void setCameraVisible(bool visible);
    }
}
