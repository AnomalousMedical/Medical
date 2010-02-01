using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Engine;

namespace Medical
{
    public class SimulationScene : SimElementManager
    {
        private String name;
        private SimulationSceneFactory factory = new SimulationSceneFactory();

        private String graphicsCameraFile = "";
        private String standardCameraFile;
        private String liteCameraFile;

        private String presetDirectory;

        private String graphicsLayersFile;
        private String standardLayersFile;
        private String liteLayersFile;

        private String sequenceDirectory;

        public SimulationScene(String name)
        {
            this.name = name;
        }

        #region Functions

        public SimElementManagerDefinition createDefinition()
        {
            SimulationSceneDefinition definition = new SimulationSceneDefinition(name);
            definition.GraphicsCameraFile = graphicsCameraFile;
            definition.StandardCameraFile = standardCameraFile;
            definition.LiteCameraFile = liteCameraFile;
            definition.PresetDirectory = presetDirectory;
            definition.GraphicsLayersFile = graphicsLayersFile;
            definition.StandardLayersFile = standardLayersFile;
            definition.LiteLayersFile = liteLayersFile;
            definition.SequenceDirectory = sequenceDirectory;
            return definition;
        }

        public SimElementFactory getFactory()
        {
            return factory;
        }

        public string getName()
        {
            return name;
        }

        public Type getSimElementManagerType()
        {
            return typeof(SimulationScene);
        }

        public void Dispose()
        {
            
        }

        #endregion

        #region Properties

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
    }
}
