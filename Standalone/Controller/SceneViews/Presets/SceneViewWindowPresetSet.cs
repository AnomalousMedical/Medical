using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Medical.Controller
{
    public class SceneViewWindowPresetSet
    {
        private LinkedList<SceneViewWindowPreset> presets = new LinkedList<SceneViewWindowPreset>();
        private String name;
        private bool hidden = false;

        public SceneViewWindowPresetSet()
        {
            this.name = "";
        }

        public SceneViewWindowPresetSet(String name)
        {
            this.name = name;
        }

        public void addPreset(SceneViewWindowPreset preset)
        {
            presets.AddLast(preset);
        }

        public void removePreset(SceneViewWindowPreset preset)
        {
            presets.Remove(preset);
        }

        internal IEnumerable<SceneViewWindowPreset> getPresetEnum()
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

        public Bitmap Image { get; set; }
    }
}
