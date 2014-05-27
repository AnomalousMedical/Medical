using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class EngineConfig
    {
        public event Action<EngineConfig> ShowStatsToggled;
        public const int MinimumAllowedFramerate = 60;
        private bool showStats = false;

        private ConfigSection section;

        public EngineConfig(ConfigFile configFile)
        {
            section = configFile.createOrRetrieveConfigSection("Engine");
            showStats = section.getValue("ShowStats", false);
        }

        public int FPSCap
        {
            get
            {
                int fpsCap = section.getValue("FPSCap", 60);
                if (fpsCap < MinimumAllowedFramerate && fpsCap != 0)
                {
                    fpsCap = MinimumAllowedFramerate;
                    section.setValue("FPSCap", MinimumAllowedFramerate);
                }
                return fpsCap;
            }
            set
            {
                section.setValue("FPSCap", value);
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
                return PlatformConfig.AllowFullscreen && section.getValue("Fullscreen", false);
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
                return showStats;
            }
            set
            {
                if (showStats != value)
                {
                    showStats = value;
                    section.setValue("ShowStats", showStats);
                    if (ShowStatsToggled != null)
                    {
                        ShowStatsToggled.Invoke(this);
                    }
                }
            }
        }
    }
}
