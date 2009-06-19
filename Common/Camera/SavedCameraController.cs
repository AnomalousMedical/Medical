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

        private const String POSITION_ENTRY = "Position";
        private const String LOOK_AT_ENTRY = "LookAt";

        public SavedCameraController(String backingFile)
        {
            savedCamerasFile = new ConfigFile(backingFile);
            savedCamerasFile.loadConfigFile();
            foreach (ConfigSection section in savedCamerasFile.getSectionIterator())
            {
                addOrUpdateSavedCamera(new SavedCameraDefinition(section.Name, section.getValue(POSITION_ENTRY, Vector3.Backward), section.getValue(LOOK_AT_ENTRY, Vector3.Zero)));
            }

            //Add default cameras.
            CameraSection cameras = MedicalConfig.CameraSection;
            addOrUpdateSavedCamera(new SavedCameraDefinition("Front", cameras.FrontCameraPosition, cameras.FrontCameraLookAt, false));
            addOrUpdateSavedCamera(new SavedCameraDefinition("Back", cameras.BackCameraPosition, cameras.BackCameraLookAt, false));
            addOrUpdateSavedCamera(new SavedCameraDefinition("Right", cameras.RightCameraPosition, cameras.RightCameraLookAt, false));
            addOrUpdateSavedCamera(new SavedCameraDefinition("Left", cameras.LeftCameraPosition, cameras.LeftCameraLookAt, false));
        }

        public void saveCameras()
        {
            savedCamerasFile.clearSections();
            foreach (SavedCameraDefinition camera in savedCameras.Values)
            {
                if (camera.Save)
                {
                    ConfigSection cameraSection = savedCamerasFile.createOrRetrieveConfigSection(camera.Name);
                    cameraSection.setValue(POSITION_ENTRY, camera.Position);
                    cameraSection.setValue(LOOK_AT_ENTRY, camera.LookAt);
                }
            }
            savedCamerasFile.writeConfigFile();
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
        }

        public bool hasSavedCamera(String name)
        {
            return savedCameras.ContainsKey(name);
        }

        public bool removeSavedCamera(String name)
        {
            if (savedCameras.ContainsKey(name) && savedCameras[name].Save == true)
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
