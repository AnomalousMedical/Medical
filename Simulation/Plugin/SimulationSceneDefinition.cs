using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Engine.Editing;
using Engine.Saving;
using Engine;

namespace Medical
{
    class SimulationSceneDefinition : SimElementManagerDefinition
    {
        /// <summary>
        /// Create function for SimulationSceneDefinitions
        /// </summary>
        /// <param name="name">The name of the definition to create.</param>
        /// <returns>A new definition.</returns>
        internal static SimulationSceneDefinition Create(String name, EditUICallback callback)
        {
            return new SimulationSceneDefinition(name);
        }

        private String name;
        private String presetDirectory;
        private String sequenceDirectory;
        private SceneViewWindowPresetController windowPresets;

        public SimulationSceneDefinition(String name)
        {
            this.name = name;
            windowPresets = new SceneViewWindowPresetController();
        }

        #region Functions

        public string Name
        {
            get 
            {
                return name;
            }
        }

        public SimElementManager createSimElementManager()
        {
            SimulationScene scene = new SimulationScene(name);
            scene.PresetDirectory = presetDirectory;
            scene.SequenceDirectory = sequenceDirectory;
            scene.WindowPresets = CopySaver.Default.copy(windowPresets);
            return scene;
        }

        public Type getSimElementManagerType()
        {
            return typeof(SimulationSceneDefinition);
        }

        public void Dispose()
        {
            
        }

        #endregion

        #region Properties

        [Editable]
        public String PresetDirectory
        {
            get
            {
                return presetDirectory;
            }
            set
            {
                presetDirectory = value;
            }
        }

        [Editable]
        public String SequenceDirectory
        {
            get
            {
                return sequenceDirectory;
            }
            set
            {
                sequenceDirectory = value;
            }
        }

        public SceneViewWindowPresetController WindowPresets
        {
            get
            {
                return windowPresets;
            }
            internal set
            {
                windowPresets = value;
            }
        }

        #endregion

        #region EditInterface

        private EditInterface editInterface;

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, name + "Simulation Scene", null);
                editInterface.addSubInterface(windowPresets.getEditInterface());
            }
            return editInterface;
        }

        #endregion

        #region Saveable Members

        private const String NAME = "Name";
        private const String PRESET_DIRECTORY = "PresetDirectory";
        private const String SEQUENCE_DIRECTORY = "SequenceDirectory";
        private const String WINDOW_PRESETS = "WindowPresets";

        protected SimulationSceneDefinition(LoadInfo info)
        {
            name = info.GetString(NAME);
            presetDirectory = info.GetString(PRESET_DIRECTORY);
            sequenceDirectory = info.GetString(SEQUENCE_DIRECTORY);
            windowPresets = info.GetValueCb<SceneViewWindowPresetController>(WINDOW_PRESETS, () =>
            {
                return new SceneViewWindowPresetController();
            });
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(NAME, name);
            info.AddValue(PRESET_DIRECTORY, presetDirectory);
            info.AddValue(SEQUENCE_DIRECTORY, sequenceDirectory);
            info.AddValue(WINDOW_PRESETS, windowPresets);
        }

        #endregion
    }
}
