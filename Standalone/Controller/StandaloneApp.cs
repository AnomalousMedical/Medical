using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    public abstract class StandaloneApp : App
    {
        public abstract void createWindowPresets(SceneViewWindowPresetController windowPresetController);

        public abstract String WindowTitle { get; }
    }
}
