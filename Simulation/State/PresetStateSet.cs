using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class PresetStateSet
    {
        private String name;
        private String sourceDirectory;
        private List<PresetState> presets = new List<PresetState>();

        public PresetStateSet(String name, String sourceDirectory)
        {
            this.name = name;
            this.sourceDirectory = sourceDirectory;
        }

        public void addPresetState(PresetState state)
        {
            presets.Add(state);
        }

        public IEnumerable<PresetState> Presets
        {
            get
            {
                return presets;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
        }

        public String SourceDirectory
        {
            get
            {
                return sourceDirectory;
            }
        }
    }
}
