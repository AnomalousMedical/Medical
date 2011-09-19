using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    public abstract class StandaloneApp : App
    {
        public StandaloneApp()
        {
            
        }

        public abstract void createWindowPresets(SceneViewWindowPresetController windowPresetController);

        public abstract String WindowTitle { get; }

        public abstract WindowIcons Icon { get; }

        public abstract String PrimaryArchive { get; }

        public abstract String getPatchArchiveName(int index);

        public abstract String DefaultScene { get; }

        public abstract int ProductID { get; }

        public LicenseManager LicenseManager { get; protected set; }
    }
}
