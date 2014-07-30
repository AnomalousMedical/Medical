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

        private Dictionary<String, SimObjectDefinition> prototypes = new Dictionary<String, SimObjectDefinition>();
        Dictionary<String, ShowPropTrackInfo> prototypeTrackInfo = new Dictionary<string, ShowPropTrackInfo>();
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

        public void addTrackInfo(String name, ShowPropTrackInfo trackInfo)
        {
            prototypeTrackInfo.Add(name, trackInfo);
        }

        public bool tryGetTrackInfo(String propTypeName, out ShowPropTrackInfo propTrackInfo)
        {
            return prototypeTrackInfo.TryGetValue(propTypeName, out propTrackInfo);
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
