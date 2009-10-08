using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class DrawingWindowPresetSet
    {
        private LinkedList<DrawingWindowPreset> presets = new LinkedList<DrawingWindowPreset>();
        private String name;
        private bool hidden = false;

        public DrawingWindowPresetSet()
        {
            this.name = "";
        }

        public DrawingWindowPresetSet(String name)
        {
            this.name = name;
        }

        public void addPreset(DrawingWindowPreset preset)
        {
            presets.AddLast(preset);
        }

        public void removePreset(DrawingWindowPreset preset)
        {
            presets.Remove(preset);
        }

        internal IEnumerable<DrawingWindowPreset> getPresetEnum()
        {
            return presets;
        }

        public String Name
        {
            get
            {
                return name;
            }
            internal set
            {
                name = value;
            }
        }

        public bool Hidden
        {
            get
            {
                return hidden;
            }
            set
            {
                hidden = value;
            }
        }
    }
}
