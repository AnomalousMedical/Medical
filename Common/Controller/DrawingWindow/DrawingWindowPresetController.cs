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
        private Dictionary<String, DrawingWindowPresetSet> presetSets = new Dictionary<string, DrawingWindowPresetSet>();
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
            presetSets.Add(preset.Name, preset);
        }

        public void removePresetSet(DrawingWindowPresetSet preset)
        {
            presetSets.Remove(preset.Name);
        }

        public void setPresetSet(String name)
        {
            DrawingWindowPresetSet preset;
            presetSets.TryGetValue(name, out preset);
            if (preset != null)
            {
                windowController.createFromPresets(preset);
            }
            else
            {
                Log.Debug("Cannot find window preset set {0}. Loading default.", name);
                windowController.createFromPresets(defaultPreset);
            }
        }

        public void loadPresetSet()
        {
            presetSets.Clear();
            tempLoadPresets();
        }

        private void tempLoadPresets()
        {
            DrawingWindowPresetSet primary = new DrawingWindowPresetSet("Primary");
            DrawingWindowPreset preset = new DrawingWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            primary.addPreset(preset);
            primary.Hidden = true;
            addPresetSet(primary);

            DrawingWindowPresetSet oneWindow = new DrawingWindowPresetSet("One Window");
            preset = new DrawingWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            oneWindow.addPreset(preset);
            addPresetSet(oneWindow);

            DrawingWindowPresetSet twoWindows = new DrawingWindowPresetSet("Two Windows");
            preset = new DrawingWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            twoWindows.addPreset(preset);

            preset = new DrawingWindowPreset("Camera 2", new Vector3(0.0f, -5.0f, -170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            preset.ParentWindow = "Camera 1";
            preset.WindowPosition = DrawingWindowPosition.Right;
            twoWindows.addPreset(preset);
            addPresetSet(twoWindows);

            DrawingWindowPresetSet threeWindows = new DrawingWindowPresetSet("Three Windows");
            preset = new DrawingWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            threeWindows.addPreset(preset);

            preset = new DrawingWindowPreset("Camera 2", new Vector3(-170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
            preset.ParentWindow = "Camera 1";
            preset.WindowPosition = DrawingWindowPosition.Bottom;
            threeWindows.addPreset(preset);

            preset = new DrawingWindowPreset("Camera 3", new Vector3(170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
            preset.ParentWindow = "Camera 2";
            preset.WindowPosition = DrawingWindowPosition.Right;
            threeWindows.addPreset(preset);
            addPresetSet(threeWindows);

            DrawingWindowPresetSet fourWindows = new DrawingWindowPresetSet("Four Windows");
            preset = new DrawingWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            fourWindows.addPreset(preset);

            preset = new DrawingWindowPreset("Camera 2", new Vector3(0.0f, -5.0f, -170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            preset.ParentWindow = "Camera 1";
            preset.WindowPosition = DrawingWindowPosition.Right;
            fourWindows.addPreset(preset);

            preset = new DrawingWindowPreset("Camera 3", new Vector3(-170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
            preset.ParentWindow = "Camera 1";
            preset.WindowPosition = DrawingWindowPosition.Bottom;
            fourWindows.addPreset(preset);

            preset = new DrawingWindowPreset("Camera 4", new Vector3(170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
            preset.ParentWindow = "Camera 3";
            preset.WindowPosition = DrawingWindowPosition.Right;
            fourWindows.addPreset(preset);
            addPresetSet(fourWindows);
        }
    }
}
