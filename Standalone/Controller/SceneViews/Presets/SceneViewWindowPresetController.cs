using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Logging;

namespace Medical.Controller
{
    public class SceneViewWindowPresetController
    {
        private List<SceneViewWindowPresetSet> presetSets = new List<SceneViewWindowPresetSet>();
        private SceneViewWindowPresetSet defaultPreset;

        public SceneViewWindowPresetController()
        {
            defaultPreset = new SceneViewWindowPresetSet("__Default");
            SceneViewWindowPreset preset = new SceneViewWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            defaultPreset.addPreset(preset);
            addPresetSet(defaultPreset);
        }

        public void addPresetSet(SceneViewWindowPresetSet preset)
        {
            presetSets.Add(preset);
        }

        public void removePresetSet(SceneViewWindowPresetSet preset)
        {
            presetSets.Remove(preset);
        }

        public SceneViewWindowPresetSet getPresetSet(String name)
        {
            SceneViewWindowPresetSet preset = null;
            foreach (SceneViewWindowPresetSet current in presetSets)
            {
                if (current.Name == name)
                {
                    preset = current;
                    break;
                }
            }
            if (preset != null)
            {
                return preset;
            }
            else
            {
                Log.Warning("Cannot find window preset set {0}. Loading default.", name);
                return defaultPreset;
            }
        }

        public void clearPresetSets()
        {
            presetSets.Clear();
        }

        public IEnumerable<SceneViewWindowPresetSet> PresetSets
        {
            get
            {
                return presetSets;
            }
        }
    }
}
