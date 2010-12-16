using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Standalone;
using Engine.ObjectManagement;

namespace Medical.Controller
{
    class PropFactory
    {
        private Dictionary<String, SimObjectDefinition> prototypes = new Dictionary<String, SimObjectDefinition>();
        private SimSubScene subScene;
        private SimScene scene;
        private MedicalController medicalController;

        public PropFactory(StandaloneController standaloneController)
        {
            this.medicalController = standaloneController.MedicalController;

            standaloneController.SceneLoaded += new SceneEvent(standaloneController_SceneLoaded);
            standaloneController.SceneUnloading += new SceneEvent(standaloneController_SceneUnloading);
        }

        public void addDefinition(String name, SimObjectDefinition definition)
        {
            prototypes.Add(name, definition);
        }

        public SimObject createSimObject(String name)
        {
            if (subScene != null)
            {
                SimObjectDefinition definition;
                prototypes.TryGetValue(name, out definition);
                if (definition != null)
                {
                    SimObjectBase instance = definition.register(subScene);
                    medicalController.addSimObject(instance);
                    scene.buildScene();
                    return instance;
                }
            }
            return null;
        }

        void standaloneController_SceneUnloading(SimScene scene)
        {
            scene = null;
            subScene = null;
        }

        void standaloneController_SceneLoaded(SimScene scene)
        {
            this.scene = scene;
            this.subScene = scene.getDefaultSubScene();
        }
    }
}
