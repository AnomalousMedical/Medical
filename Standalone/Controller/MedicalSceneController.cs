using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Engine;
using Engine.Resources;

namespace Medical
{
    /// <summary>
    /// This delegate is called when the MedicalSceneController fires an event.
    /// </summary>
    /// <param name="controller">The controller that fired the event.</param>
    /// <param name="scene">The scene for the event.</param>
    delegate void MedicalSceneControllerEvent(MedicalSceneController controller, SimScene scene);

    class MedicalSceneController
    {
        private SimScene currentScene;
        private SimObjectManager currentSimObjects;
        private ScenePackage currentScenePackage;
        private PluginManager pluginManager;
        private ResourceManager sceneResourceManager;

        #region Events

        /// <summary>
        /// This event is fired before a scene loads.
        /// </summary>
        public event MedicalSceneControllerEvent OnSceneLoading;

        /// <summary>
        /// This event is fired when a scene is loaded.
        /// </summary>
        public event MedicalSceneControllerEvent OnSceneLoaded;

        /// <summary>
        /// This event is fired when a scene starts unloading.
        /// </summary>
        public event MedicalSceneControllerEvent OnSceneUnloading;

        /// <summary>
        /// This event is fired when a scene has finished unloading.
        /// </summary>
        public event MedicalSceneControllerEvent OnSceneUnloaded;

        #endregion Events

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pluginManager"></param>
        public MedicalSceneController(PluginManager pluginManager)
        {
            this.pluginManager = pluginManager;
            sceneResourceManager = pluginManager.createLiveResourceManager("Scene");
        }

        /// <summary>
        /// Load the scene in the given ScenePackage.
        /// </summary>
        /// <param name="scenePackage">The ScenePackage to load.</param>
        public IEnumerable<SceneBuildStatus> loadScene(ScenePackage scenePackage, SceneBuildOptions options)
        {
            currentScenePackage = scenePackage;
            yield return new SceneBuildStatus()
            {
                Message = "Setting up Resources"
            };
            sceneResourceManager.changeResourcesToMatch(scenePackage.ResourceManager);
            sceneResourceManager.initializeResources();

            currentScene = scenePackage.SceneDefinition.createScene();
            if (OnSceneLoading != null)
            {
                OnSceneLoading.Invoke(this, currentScene);
            }
            currentSimObjects = scenePackage.SimObjectManagerDefinition.createSimObjectManager(currentScene.getDefaultSubScene());
            foreach(var status in currentScene.buildSceneStatus(options))
            {
                yield return status;
            }
            if (OnSceneLoaded != null)
            {
                OnSceneLoaded.Invoke(this, currentScene);
            }
        }

        /// <summary>
        /// Destroy the currently loaded scene. Does nothing if no scene is loaded.
        /// </summary>
        public void destroyScene()
        {
            if (currentScene != null)
            {
                if (OnSceneUnloading != null)
                {
                    OnSceneUnloading.Invoke(this, currentScene);
                }
                currentSimObjects.Dispose();
                currentSimObjects = null;
                currentScene.Dispose();
                currentScene = null;
                if (OnSceneUnloaded != null)
                {
                    OnSceneUnloaded.Invoke(this, null);
                }
            }
        }

        public void addSimObject(SimObjectBase simObject)
        {
            currentSimObjects.addSimObject(simObject);
        }

        public SimObject getSimObject(String name)
        {
            SimObjectBase simObject;
            currentSimObjects.tryGetSimObject(name, out simObject);
            return simObject;
        }

        public SimScene CurrentScene
        {
            get
            {
                return currentScene;
            }
        }

        public IEnumerable<SimObjectBase> SimObjects
        {
            get
            {
                return currentSimObjects.SimObjects;
            }
        }
    }
}
