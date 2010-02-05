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

        private String cameraFileDirectory = "";

        private String presetDirectory;

        private String layersFileDirectory;

        private String sequenceDirectory;

        public SimulationScene(String name)
        {
            this.name = name;
        }

        #region Functions

        public SimElementManagerDefinition createDefinition()
        {
            SimulationSceneDefinition definition = new SimulationSceneDefinition(name);
            definition.CameraFileDirectory = cameraFileDirectory;
            definition.PresetDirectory = presetDirectory;
            definition.LayersFileDirectory = layersFileDirectory;
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

        public String CameraFileDirectory
        {
            get
            {
                return cameraFileDirectory;
            }
            set
            {
                cameraFileDirectory = value;
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

        public String LayersFileDirectory
        {
            get
            {
                return layersFileDirectory;
            }
            set
            {
                layersFileDirectory = value;
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
