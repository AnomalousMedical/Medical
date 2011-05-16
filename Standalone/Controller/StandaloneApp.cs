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
            WatermarkText = null;
        }

        private List<String> movementSequenceDirectories = new List<string>();

        public abstract void createWindowPresets(SceneViewWindowPresetController windowPresetController);

        public abstract void addHelpDocuments(HtmlHelpController helpController);

        public abstract String WindowTitle { get; }

        public abstract String ProgramFolder { get; }

        public abstract WindowIcons Icon { get; }

        public abstract String PrimaryArchive { get; }

        public abstract String getPatchArchiveName(int index);

        public abstract String DefaultScene { get; }

        public abstract int ProductID { get; }

        public virtual bool IsTrial
        {
            get
            {
                return false;
            }
        }

        public String CamerasFile { get; protected set; }

        public String LayersFile { get; protected set; }

        public String WatermarkText { get; protected set; }

        public LicenseManager LicenseManager { get; protected set; }

        public List<String> MovementSequenceDirectories
        {
            get
            {
                return movementSequenceDirectories;
            }
        }

        protected void addMovementSequenceDirectory(String directory)
        {
            movementSequenceDirectories.Add(directory);
        }
    }
}
