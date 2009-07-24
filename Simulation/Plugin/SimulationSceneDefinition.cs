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
        private Dictionary<String, PredefinedCamera> cameras = new Dictionary<String, PredefinedCamera>();

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

        public void addCamera(String name, Vector3 translation, Vector3 lookAt)
        {
            PredefinedCamera camera = new PredefinedCamera(name, translation, lookAt);
            cameras.Add(name, camera);
            if (editInterface != null)
            {
                createCameraEditInterface(camera);
            }
        }

        public SimElementManager createSimElementManager()
        {
            SimulationScene scene = new SimulationScene(name);
            foreach (PredefinedCamera camera in cameras.Values)
            {
                scene.addCamera(camera.Name, camera.Translation, camera.LookAt);
            }
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

        #region EditInterface

        private EditInterfaceCommand removeCameraCommand;
        private EditInterfaceManager<PredefinedCamera> cameraEdits;
        private EditInterface editInterface;

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, name + "Simulation Scene", null);
                removeCameraCommand = new EditInterfaceCommand("Remove", removeCamera);
                editInterface.addCommand(new EditInterfaceCommand("Add Predefined Camera", addCamera));
                cameraEdits = new EditInterfaceManager<PredefinedCamera>(editInterface);
                foreach (PredefinedCamera state in cameras.Values)
                {
                    createCameraEditInterface(state);
                }
            }
            return editInterface;
        }

        private void addCamera(EditUICallback callback, EditInterfaceCommand command)
        {
            String name;
            if (callback.getInputString("Enter the name of the camera.", out name, validateCameraName))
            {
                PredefinedCamera camera = new PredefinedCamera(name, Vector3.UnitX, Vector3.Zero);
                cameras.Add(name, camera);
                createCameraEditInterface(camera);
            }
        }

        private bool validateCameraName(string input, out string newPrompt)
        {
            if (cameras.ContainsKey(name))
            {
                newPrompt = "Camera already exists please enter another name.";
                return false;
            }
            newPrompt = "";
            return true;
        }

        private void removeCamera(EditUICallback callback, EditInterfaceCommand command)
        {
            PredefinedCamera camera = cameraEdits.resolveSourceObject(callback.getSelectedEditInterface());
            cameras.Remove(camera.Name);
            cameraEdits.removeSubInterface(camera);
        }

        private void createCameraEditInterface(PredefinedCamera camera)
        {
            EditInterface edit = ReflectedEditInterface.createEditInterface(camera, ReflectedEditInterface.DefaultScanner, camera.Name + " - Predefined Camera", null);
            edit.addCommand(removeCameraCommand);
            cameraEdits.addSubInterface(camera, edit);
        }

        #endregion

        #region Saveable Members

        private String NAME = "Name";
        private String CAMERA_BASE = "Camera";

        protected SimulationSceneDefinition(LoadInfo info)
        {
            name = info.GetString(NAME);
            info.RebuildDictionary<String, PredefinedCamera>(CAMERA_BASE, cameras);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(NAME, name);
            info.ExtractDictionary<String, PredefinedCamera>(CAMERA_BASE, cameras);
        }

        #endregion
    }
}
