using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class RenderPresetCollection : Saveable, IEnumerable<RenderPreset>
    {
        private LinkedList<RenderPreset> renderPresets = new LinkedList<RenderPreset>();

        public RenderPresetCollection()
        {

        }

        public void AddLast(RenderPreset preset)
        {
            renderPresets.AddLast(preset);
        }

        protected RenderPresetCollection(LoadInfo info)
        {
            info.RebuildLinkedList<RenderPreset>("Preset", renderPresets);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractLinkedList<RenderPreset>("Preset", renderPresets);
        }

        public IEnumerator<RenderPreset> GetEnumerator()
        {
            return renderPresets.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return renderPresets.GetEnumerator();
        }
    }
}
