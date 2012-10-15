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
        public const int MinimumAllowedFramerate = 60;

        private ConfigSection section;

        public EngineConfig(ConfigFile configFile)
        {
            section = configFile.createOrRetrieveConfigSection("Engine");
        }

        public int MaxFPS
        {
            get
            {
                int maxFps = section.getValue("MaxFPS", 120);
                if (maxFps < MinimumAllowedFramerate && maxFps != 0)
                {
                    maxFps = MinimumAllowedFramerate;
                    section.setValue("MaxFPS", MinimumAllowedFramerate);
                }
                return maxFps;
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

        public bool UseHardwareSkinning
        {
            get
            {
                return section.getValue("UseHardwareSkinning", () => { return PlatformConfig.PreferHardwareSkinning; });
            }
            set
            {
                section.setValue("UseHardwareSkinning", value);
            }
        }
    }
}
