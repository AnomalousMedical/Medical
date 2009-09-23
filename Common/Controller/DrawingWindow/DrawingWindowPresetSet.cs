using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class DrawingWindowPresetSet
    {
        private LinkedList<DrawingWindowPreset> presets = new LinkedList<DrawingWindowPreset>();

        public DrawingWindowPresetSet()
        {

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
    }
}
