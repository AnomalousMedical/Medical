using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class EngineConfig
    {
        public event EventHandler ShowStatsToggled;

        private ConfigSection section;

        public EngineConfig(ConfigFile configFile)
        {
            section = configFile.createOrRetrieveConfigSection("Engine");
        }

        public int MaxFPS
        {
            get
            {
                return section.getValue("MaxFPS", 0);
            }
            set
            {
                section.setValue("MaxFPS", value);
            }
        }

        public int HorizontalRes
        {
            get
            {
                return section.getValue("HorizontalRes", 800);
            }
            set
            {
                section.setValue("HorizontalRes", value);
            }
        }

        public int VerticalRes
        {
            get
            {
                return section.getValue("VerticalRes", 600);
            }
            set
            {
                section.setValue("VerticalRes", value);
            }
        }

        public bool Fullscreen
        {
            get
            {
#if MAC_OSX
                //Fullscreen does not work on mac, so block it.
                return false;
#else
                return section.getValue("Fullscreen", false);
#endif
            }
            set
            {
                section.setValue("Fullscreen", value);
            }
        }

        public bool ShowStatistics
        {
            get
            {
                return section.getValue("ShowStats", false);
            }
            set
            {
                section.setValue("ShowStats", value);
                if (ShowStatsToggled != null)
                {
                    ShowStatsToggled.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
