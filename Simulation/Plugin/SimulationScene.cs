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
        private Dictionary<String, PredefinedCamera> cameras = new Dictionary<String, PredefinedCamera>();
        private SimulationSceneFactory factory = new SimulationSceneFactory();

        public SimulationScene(String name)
        {
            this.name = name;
        }

        #region Functions

        internal void addCamera(String name, Vector3 translation, Vector3 lookAt)
        {
            cameras.Add(name, new PredefinedCamera(name, translation, lookAt));
        }

        public IEnumerable<PredefinedCamera> getPredefinedCameras()
        {
            return cameras.Values;
        }

        public bool contains(string cameraName)
        {
            return cameras.ContainsKey(cameraName);
        }

        public PredefinedCamera get(string cameraName)
        {
            PredefinedCamera camera;
            cameras.TryGetValue(cameraName, out camera);
            return camera;
        }

        public SimElementManagerDefinition createDefinition()
        {
            SimulationSceneDefinition definition = new SimulationSceneDefinition(name);
            foreach (PredefinedCamera camera in cameras.Values)
            {
                definition.addCamera(camera.Name, camera.Translation, camera.LookAt);
            }
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
    }
}
