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

        public SimulationSceneDefinition(String name)
        {
            this.name = name;
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

        #endregion

        #region EditInterface

        private EditInterface editInterface;

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, name + "Simulation Scene", null);
            }
            return editInterface;
        }

        #endregion

        #region Saveable Members

        private const String NAME = "Name";
        private const String CAMERA_FILE_DIRECTORY = "CameraFileDirectory";
        private const String PRESET_DIRECTORY = "PresetDirectory";
        private const String LAYERS_FILE_DIRECTORY = "LayersFileDirectory";
        private const String SEQUENCE_DIRECTORY = "SequenceDirectory";

        protected SimulationSceneDefinition(LoadInfo info)
        {
            name = info.GetString(NAME);
            presetDirectory = info.GetString(PRESET_DIRECTORY);
            sequenceDirectory = info.GetString(SEQUENCE_DIRECTORY);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(NAME, name);
            info.AddValue(PRESET_DIRECTORY, presetDirectory);
            info.AddValue(SEQUENCE_DIRECTORY, sequenceDirectory);
        }

        #endregion
    }
}
