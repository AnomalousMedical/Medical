using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Engine;

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
        }

        /// <summary>
        /// Load the scene in the given ScenePackage.
        /// </summary>
        /// <param name="scenePackage">The ScenePackage to load.</param>
        public void loadScene(ScenePackage scenePackage)
        {
            currentScenePackage = scenePackage;
            pluginManager.PrimaryResourceManager.changeResourcesToMatch(scenePackage.ResourceManager);
            pluginManager.PrimaryResourceManager.forceResourceRefresh();
            currentScene = scenePackage.SceneDefinition.createScene();
            if (OnSceneLoading != null)
            {
                OnSceneLoading.Invoke(this, currentScene);
            }
            currentSimObjects = scenePackage.SimObjectManagerDefinition.createSimObjectManager(currentScene.getDefaultSubScene());
            currentScene.buildScene();
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

        public ScenePackage saveSceneToPackage()
        {
            ScenePackage package = new ScenePackage();
            package.ResourceManager = pluginManager.createSecondaryResourceManager();
            package.SceneDefinition = currentScene.createDefinition();
            package.SimObjectManagerDefinition = currentSimObjects.saveToDefinition();
            return package;
        }
    }
}
