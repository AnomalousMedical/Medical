using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Engine.Editing;
using Engine.Saving;
using Engine;

namespace Medical
{
    class SimulationSceneDefinition : SimElementManagerDefinition
    {
        /// <summary>
        /// Create function for SimulationSceneDefinitions
        /// </summary>
        /// <param name="name">The name of the definition to create.</param>
        /// <returns>A new definition.</returns>
        internal static SimulationSceneDefinition Create(String name, EditUICallback callback)
        {
            return new SimulationSceneDefinition(name);
        }

        private String name;
        private String cameraFile;

        public SimulationSceneDefinition(String name)
        {
            this.name = name;
        }

        #region Functions

        public string Name
        {
            get 
            {
                return name;
            }
        }

        public SimElementManager createSimElementManager()
        {
            SimulationScene scene = new SimulationScene(name);
            scene.CameraFile = cameraFile;
            return scene;
        }

        public Type getSimElementManagerType()
        {
            return typeof(SimulationSceneDefinition);
        }

        public void Dispose()
        {
            
        }

        #endregion

        #region Properties

        [Editable]
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

        #region EditInterface

        private EditInterface editInterface;

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, name + "Simulation Scene", null);
            }
            return editInterface;
        }

        #endregion

        #region Saveable Members

        private String NAME = "Name";
        private String CAMERA_FILE = "CameraFile";

        protected SimulationSceneDefinition(LoadInfo info)
        {
            name = info.GetString(NAME);
            cameraFile = info.GetString(CAMERA_FILE);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(NAME, name);
            info.AddValue(CAMERA_FILE, cameraFile);
        }

        #endregion
    }
}
