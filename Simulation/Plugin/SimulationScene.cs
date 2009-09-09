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
        private String cameraFile = "";

        public SimulationScene(String name)
        {
            this.name = name;
        }

        #region Functions

        public SimElementManagerDefinition createDefinition()
        {
            SimulationSceneDefinition definition = new SimulationSceneDefinition(name);
            definition.CameraFile = cameraFile;
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

        #endregion 
    }
}
