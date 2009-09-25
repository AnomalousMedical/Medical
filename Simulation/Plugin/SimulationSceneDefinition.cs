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
        private String cameraFile;
        private String presetDirectory;
        private String layersFile;

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
            scene.CameraFile = cameraFile;
            scene.PresetDirectory = presetDirectory;
            scene.LayersFile = layersFile;
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
        public String CameraFile
        {
            get
            {
                return cameraFile;
            }
            set
            {
                cameraFile = value;
            }
        }

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
        public String LayersFile
        {
            get
            {
                return layersFile;
            }
            set
            {
                layersFile = value;
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
        private const String CAMERA_FILE = "CameraFile";
        private const String PRESET_DIRECTORY = "PresetDirectory";
        private const String LAYERS_FILE = "LayersFile";

        protected SimulationSceneDefinition(LoadInfo info)
        {
            name = info.GetString(NAME);
            cameraFile = info.GetString(CAMERA_FILE);
            presetDirectory = info.GetString(PRESET_DIRECTORY);
            layersFile = info.GetString(LAYERS_FILE);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(NAME, name);
            info.AddValue(CAMERA_FILE, cameraFile);
            info.AddValue(PRESET_DIRECTORY, presetDirectory);
            info.AddValue(LAYERS_FILE, layersFile);
        }

        #endregion
    }
}
