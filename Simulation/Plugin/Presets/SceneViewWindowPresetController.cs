using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Logging;
using Engine.Attributes;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    public partial class SceneViewWindowPresetController : Saveable
    {
        [DoNotSave]
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
            itemAdded(preset);
        }

        public void removePresetSet(SceneViewWindowPresetSet preset)
        {
            presetSets.Remove(preset);
            itemRemoved(preset);
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
            itemsCleared();
        }

        public IEnumerable<SceneViewWindowPresetSet> PresetSets
        {
            get
            {
                return presetSets;
            }
        }

        protected SceneViewWindowPresetController(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info);
            info.RebuildList<SceneViewWindowPresetSet>("Preset", presetSets);
        }

        public void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info);
            info.ExtractList<SceneViewWindowPresetSet>("Preset", presetSets);
        }
    }

    public partial class SceneViewWindowPresetController
    {
        [DoNotSave]
        private EditInterface editInterface;

        [DoNotSave]
        private EditInterfaceManager<SceneViewWindowPresetSet> itemEdits;

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, "Window Presets");
                editInterface.addSubInterface(defaultPreset.getEditInterface());
                editInterface.addCommand(new EditInterfaceCommand("Add", createNewItem));

                itemEdits = new EditInterfaceManager<SceneViewWindowPresetSet>(editInterface);
                itemEdits.addCommand(new EditInterfaceCommand("Remove", removeItem));

                foreach (SceneViewWindowPresetSet set in presetSets)
                {
                    if (set != defaultPreset)
                    {
                        itemAdded(set);
                    }
                }
            }
            return editInterface;
        }

        private void itemAdded(SceneViewWindowPresetSet preset)
        {
            if (itemEdits != null)
            {
                itemEdits.addSubInterface(preset, preset.getEditInterface());
            }
        }

        private void itemRemoved(SceneViewWindowPresetSet preset)
        {
            if (itemEdits != null)
            {
                itemEdits.removeSubInterface(preset);
            }
        }

        private void itemsCleared()
        {
            if (itemEdits != null)
            {
                itemEdits.clearSubInterfaces();
            }
        }

        private void createNewItem(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a name.", delegate(String input, ref String errorPrompt)
            {
                if (!hasItem(input))
                {
                    SceneViewWindowPresetSet item = new SceneViewWindowPresetSet(input);
                    addPresetSet(item);
                    return true;
                }
                errorPrompt = String.Format("An item named {0} already exists. Please input another name.", input);
                return false;
            });
        }

        private void removeItem(EditUICallback callback, EditInterfaceCommand command)
        {
            SceneViewWindowPresetSet item = itemEdits.resolveSourceObject(callback.getSelectedEditInterface());
            removePresetSet(item);
        }

        private bool hasItem(String name)
        {
            foreach (SceneViewWindowPresetSet set in presetSets)
            {
                if (set.Name == name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
