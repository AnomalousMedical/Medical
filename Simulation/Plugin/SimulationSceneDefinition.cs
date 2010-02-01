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
        private String graphicsCameraFile;
        private String standardCameraFile;
        private String liteCameraFile;
        private String presetDirectory;
        private String graphicsLayersFile;
        private String standardLayersFile;
        private String liteLayersFile;
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
            scene.GraphicsCameraFile = graphicsCameraFile;
            scene.StandardCameraFile = standardCameraFile;
            scene.LiteCameraFile = liteCameraFile;
            scene.PresetDirectory = presetDirectory;
            scene.GraphicsLayersFile = graphicsLayersFile;
            scene.StandardLayersFile = standardLayersFile;
            scene.LiteLayersFile = liteLayersFile;
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
        public String GraphicsCameraFile
        {
            get
            {
                return graphicsCameraFile;
            }
            set
            {
                graphicsCameraFile = value;
            }
        }

        [Editable]
        public String StandardCameraFile
        {
            get
            {
                return standardCameraFile;
            }
            set
            {
                standardCameraFile = value;
            }
        }

        [Editable]
        public String LiteCameraFile
        {
            get
            {
                return liteCameraFile;
            }
            set
            {
                liteCameraFile = value;
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
        public String GraphicsLayersFile
        {
            get
            {
                return graphicsLayersFile;
            }
            set
            {
                graphicsLayersFile = value;
            }
        }

        [Editable]
        public String StandardLayersFile
        {
            get
            {
                return standardLayersFile;
            }
            set
            {
                standardLayersFile = value;
            }
        }

        [Editable]
        public String LiteLayersFile
        {
            get
            {
                return liteLayersFile;
            }
            set
            {
                liteLayersFile = value;
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
        private const String GRAPHICS_CAMERA_FILE = "GraphicsCameraFile";
        private const String STANDARD_CAMERA_FILE = "StandardCameraFile";
        private const String LITE_CAMERA_FILE = "LiteCameraFile";
        private const String PRESET_DIRECTORY = "PresetDirectory";
        private const String GRAPHICS_LAYERS_FILE = "GraphicsLayersFile";
        private const String STANDARD_LAYERS_FILE = "StandardLayersFile";
        private const String LITE_LAYERS_FILE = "LiteLayersFile";
        private const String SEQUENCE_DIRECTORY = "SequenceDirectory";

        protected SimulationSceneDefinition(LoadInfo info)
        {
            name = info.GetString(NAME);
            graphicsCameraFile = info.GetString(GRAPHICS_CAMERA_FILE);
            standardCameraFile = info.GetString(STANDARD_CAMERA_FILE);
            liteCameraFile = info.GetString(LITE_CAMERA_FILE);
            presetDirectory = info.GetString(PRESET_DIRECTORY);
            graphicsLayersFile = info.GetString(GRAPHICS_LAYERS_FILE);
            standardLayersFile = info.GetString(STANDARD_LAYERS_FILE);
            liteLayersFile = info.GetString(LITE_LAYERS_FILE);
            sequenceDirectory = info.GetString(SEQUENCE_DIRECTORY);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(NAME, name);
            info.AddValue(GRAPHICS_CAMERA_FILE, graphicsCameraFile);
            info.AddValue(STANDARD_CAMERA_FILE, standardCameraFile);
            info.AddValue(LITE_CAMERA_FILE, liteCameraFile);
            info.AddValue(PRESET_DIRECTORY, presetDirectory);
            info.AddValue(GRAPHICS_LAYERS_FILE, graphicsLayersFile);
            info.AddValue(STANDARD_LAYERS_FILE, standardLayersFile);
            info.AddValue(LITE_LAYERS_FILE, liteLayersFile);
            info.AddValue(SEQUENCE_DIRECTORY, sequenceDirectory);
        }

        #endregion
    }
}
