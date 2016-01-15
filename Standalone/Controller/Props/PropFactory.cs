﻿using System;
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
        public const String ManualObjectName = "ManualObject";
        public const String FadeBehaviorName = "FadeBehavior";
        public const String DetachableFollowerName = "DetachableFollower";
        public const String RigidBodyName = "RigidBody";

        private Dictionary<String, PropDefinition> prototypes = new Dictionary<String, PropDefinition>();
        private SimSubScene subScene;
        private SimScene scene;
        private MedicalController medicalController;

        public PropFactory(StandaloneController standaloneController)
        {
            this.medicalController = standaloneController.MedicalController;

            standaloneController.SceneLoaded += new SceneEvent(standaloneController_SceneLoaded);
            standaloneController.SceneUnloading += new SceneEvent(standaloneController_SceneUnloading);
        }

        public void addDefinition(PropDefinition definition)
        {
            prototypes.Add(definition.Name, definition);
        }

        public void removePropsForPlugin(long pluginId)
        {
            List<PropDefinition> props = new List<PropDefinition>(prototypes.Values.Where(i => i.DependencyPluginId == pluginId));
            foreach(var prop in props)
            {
                prototypes.Remove(prop.Name);
            }
        }

        public bool tryGetTrackInfo(String propTypeName, out ShowPropTrackInfo propTrackInfo)
        {
            PropDefinition propDef;
            bool found = false;
            propTrackInfo = null;
            if(prototypes.TryGetValue(propTypeName, out propDef))
            {
                found = true;
                propTrackInfo = propDef.TrackInfo;
            }
            return found;
        }

        public bool hasProp(String propName)
        {
            return prototypes.ContainsKey(propName);
        }

        public SimObjectBase createProp(String propName, Vector3 translation, Quaternion rotation)
        {
            if (subScene != null)
            {
                PropDefinition propDef;
                if (prototypes.TryGetValue(propName, out propDef))
                {
                    SimObjectDefinition definition = propDef.SimObject;
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

        public long? getDependencyIdForProp(string propName)
        {
            PropDefinition propDef;
            if (prototypes.TryGetValue(propName, out propDef))
            {
                return propDef.DependencyPluginId;
            }
            return null;
        }

        public void getInitialPosition(String propName, ref Vector3 translation, ref Quaternion rotation)
        {
            PropDefinition propDef;
            if (prototypes.TryGetValue(propName, out propDef))
            {
                SimObjectDefinition definition = propDef.SimObject;
                translation = definition.Translation;
                rotation = definition.Rotation;
            }
        }

        public IEnumerable<PropDefinition> PropDefinitions
        {
            get
            {
                return prototypes.Values;
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
