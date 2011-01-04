using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Engine;

namespace Medical
{
    public class PropFactory
    {
        public const String NodeName = "Node";
        public const String EntityName = "Entity";
        public const String FadeBehaviorName = "FadeBehavior";

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

        public SimObjectBase createProp(String propName, Vector3 translation, Quaternion rotation)
        {
            if (subScene != null)
            {
                SimObjectDefinition definition;
                prototypes.TryGetValue(propName, out definition);
                if (definition != null)
                {
                    definition.Name = UniqueKeyGenerator.generateStringKey();
                    definition.Translation = translation;
                    definition.Rotation = rotation;
                    SimObjectBase instance = definition.register(subScene);
                    medicalController.addSimObject(instance);
                    scene.buildScene();
                    return instance;
                }
            }
            return null;
        }

        public IEnumerable<String> PropNames
        {
            get
            {
                return prototypes.Keys;
            }
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
