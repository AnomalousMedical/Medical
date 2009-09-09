using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class SavedCameraController
    {
        private Dictionary<String, SavedCameraDefinition> savedCameras = new Dictionary<string,SavedCameraDefinition>();
        ConfigFile savedCamerasFile;
        private bool loading = false;

        private const String POSITION_ENTRY = "Position";
        private const String LOOK_AT_ENTRY = "LookAt";

        public SavedCameraController()
        {

        }

        public SavedCameraController(String backingFile)
        {
            changeBackingFile(backingFile);
        }

        public void changeBackingFile(String backingFile)
        {
            savedCameras.Clear();
            loading = true;
            savedCamerasFile = new ConfigFile(backingFile);
            savedCamerasFile.loadConfigFile();
            foreach (ConfigSection section in savedCamerasFile.getSectionIterator())
            {
                addOrUpdateSavedCamera(new SavedCameraDefinition(section.Name, section.getValue(POSITION_ENTRY, Vector3.Backward), section.getValue(LOOK_AT_ENTRY, Vector3.Zero)));
            }
            loading = false;
        }

        public void saveCameras()
        {
            if (!loading)
            {
                savedCamerasFile.clearSections();
                foreach (SavedCameraDefinition camera in savedCameras.Values)
                {
                    ConfigSection cameraSection = savedCamerasFile.createOrRetrieveConfigSection(camera.Name);
                    cameraSection.setValue(POSITION_ENTRY, camera.Position);
                    cameraSection.setValue(LOOK_AT_ENTRY, camera.LookAt);
                }
                savedCamerasFile.writeConfigFile();
            }
        }

        public void addOrUpdateSavedCamera(SavedCameraDefinition definition)
        {
            if (!savedCameras.ContainsKey(definition.Name))
            {
                savedCameras.Add(definition.Name, definition);
            }
            else
            {
                savedCameras[definition.Name] = definition;
            }
            saveCameras();
        }

        public bool hasSavedCamera(String name)
        {
            return savedCameras.ContainsKey(name);
        }

        public bool removeSavedCamera(String name)
        {
            if (savedCameras.ContainsKey(name))
            {
                savedCameras.Remove(name);
                return true;
            }
            return false;
        }

        public void removeSavedCamera(SavedCameraDefinition definition)
        {
            if (savedCameras.ContainsKey(definition.Name))
            {
                savedCameras.Remove(definition.Name);
            }
        }

        public SavedCameraDefinition getSavedCamera(string name)
        {
            if(savedCameras.ContainsKey(name))
            {
                return savedCameras[name];
            }
            return null;
        }

        public IEnumerable<String> getSavedCameraNames()
        {
            return savedCameras.Keys;
        }
    }
}
