using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Logging;

namespace Medical.Controller
{
    public class DrawingWindowPresetController
    {
        private List<DrawingWindowPresetSet> presetSets = new List<DrawingWindowPresetSet>();
        private DrawingWindowPresetSet defaultPreset;
        private DrawingWindowController windowController;

        public DrawingWindowPresetController(DrawingWindowController windowController)
        {
            defaultPreset = new DrawingWindowPresetSet("__Default");
            DrawingWindowPreset preset = new DrawingWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            defaultPreset.addPreset(preset);
            addPresetSet(defaultPreset);
            this.windowController = windowController;
        }

        public void addPresetSet(DrawingWindowPresetSet preset)
        {
            presetSets.Add(preset);
        }

        public void removePresetSet(DrawingWindowPresetSet preset)
        {
            presetSets.Remove(preset);
        }

        public void setPresetSet(String name)
        {
            DrawingWindowPresetSet preset = null;
            foreach (DrawingWindowPresetSet current in presetSets)
            {
                if (current.Name == name)
                {
                    preset = current;
                    break;
                }
            }
            if (preset != null)
            {
                windowController.createFromPresets(preset);
            }
            else
            {
                Log.Warning("Cannot find window preset set {0}. Loading default.", name);
                windowController.createFromPresets(defaultPreset);
            }
        }

        public void clearPresetSets()
        {
            presetSets.Clear();
        }

        public IEnumerable<DrawingWindowPresetSet> PresetSets
        {
            get
            {
                return presetSets;
            }
        }
    }
}
