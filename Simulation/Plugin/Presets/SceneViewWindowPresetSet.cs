using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Attributes;
using Engine.Editing;
using Engine;

namespace Medical
{
    public partial class SceneViewWindowPresetSet : Saveable
    {
        [DoNotSave]
        private LinkedList<SceneViewWindowPreset> presets = new LinkedList<SceneViewWindowPreset>();
        private String name;

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
            itemAdded(preset);
        }

        public void removePreset(SceneViewWindowPreset preset)
        {
            presets.Remove(preset);
            itemRemoved(preset);
        }

        public IEnumerable<SceneViewWindowPreset> getPresetEnum()
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

        protected SceneViewWindowPresetSet(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info);
            info.RebuildLinkedList<SceneViewWindowPreset>("Preset", presets);
        }

        public void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info);
            info.ExtractLinkedList<SceneViewWindowPreset>("Preset", presets);
        }
    }

    public partial class SceneViewWindowPresetSet
    {
        [DoNotSave]
        private EditInterface editInterface;

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, String.Format("{0} - Preset Set", Name));
                editInterface.addCommand(new EditInterfaceCommand("Add", createNewItem));

                var itemEdits = editInterface.createEditInterfaceManager<SceneViewWindowPreset>();
                itemEdits.addCommand(new EditInterfaceCommand("Remove", removeItem));

                foreach (SceneViewWindowPreset set in presets)
                {
                    itemAdded(set);
                }
            }
            return editInterface;
        }

        private void itemAdded(SceneViewWindowPreset preset)
        {
            if (editInterface != null)
            {
                editInterface.addSubInterface(preset, preset.getEditInterface());
            }
        }

        private void itemRemoved(SceneViewWindowPreset preset)
        {
            if (editInterface != null)
            {
                editInterface.removeSubInterface(preset);
            }
        }

        private void createNewItem(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a name.", delegate(String input, ref String errorPrompt)
            {
                if (!hasItem(input))
                {
                    SceneViewWindowPreset item = new SceneViewWindowPreset(input, Vector3.Zero, Vector3.Zero);
                    addPreset(item);
                    return true;
                }
                errorPrompt = String.Format("A preset named {0} already exists. Please input another name.", input);
                return false;
            });
        }

        private void removeItem(EditUICallback callback, EditInterfaceCommand command)
        {
            SceneViewWindowPreset item = editInterface.resolveSourceObject<SceneViewWindowPreset>(callback.getSelectedEditInterface());
            removePreset(item);
        }

        private bool hasItem(String name)
        {
            foreach (SceneViewWindowPreset set in presets)
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
