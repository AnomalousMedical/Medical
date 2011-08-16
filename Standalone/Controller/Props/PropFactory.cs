using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Engine;
using Logging;
using OgrePlugin;

namespace Medical
{
    public class PropFactory
    {
        public const String NodeName = "Node";
        public const String EntityName = "Entity";
        public const String FadeBehaviorName = "FadeBehavior";
        public const String DetachableFollowerName = "DetachableFollower";
        public const String RigidBodyName = "RigidBody";

        private Dictionary<String, SimObjectDefinition> prototypes = new Dictionary<String, SimObjectDefinition>();
        private SimSubScene subScene;
        private SimScene scene;
        private MedicalController medicalController;

        /// <summary>
        /// Create a generic prop with an mesh and a propFadeBehavior.
        /// </summary>
        /// <param name="definitionName">The name of the prop definition.</param>
        /// <param name="meshName">The name of the mesh for the prop.</param>
        /// <returns>A SimObjectDefinition with the entity specified.</returns>
        public static SimObjectDefinition createGenericProp(String definitionName, String meshName)
        {
            GenericSimObjectDefinition simObject = new GenericSimObjectDefinition(definitionName);
            simObject.Enabled = true;
            
            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = meshName;
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            simObject.addElement(nodeDefinition);
            
            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            simObject.addElement(propFadeBehaviorDef);

            return simObject;
        }

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
                if (prototypes.TryGetValue(propName, out definition))
                {
                    Vector3 originalTranslation = definition.Translation;
                    Quaternion originalRotation = definition.Rotation;

                    definition.Name = UniqueKeyGenerator.generateStringKey();
                    definition.Translation = translation;
                    definition.Rotation = rotation;
                    SimObjectBase instance = definition.register(subScene);
                    medicalController.addSimObject(instance);
                    scene.buildScene();

                    definition.Translation = originalTranslation;
                    definition.Rotation = originalRotation;
                    return instance;
                }
                else
                {
                    Log.Error("Could not create prop {0}. The definition cannot be found.", propName);
                }
            }
            else
            {
                Log.Error("Could not create prop {0}. The subscene is null.", propName);
            }
            return null;
        }

        public void getInitialPosition(String propName, ref Vector3 translation, ref Quaternion rotation)
        {
            SimObjectDefinition definition;
            if (prototypes.TryGetValue(propName, out definition))
            {
                translation = definition.Translation;
                rotation = definition.Rotation;
            }
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
