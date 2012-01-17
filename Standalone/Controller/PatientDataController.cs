using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical
{
    public class PatientDataController
    {
        public event Action<PatientDataFile> PatientDataChanged;

        private MedicalStateController medicalStateController;
        private ExamController examController;
        private MedicalController medicalController;
        private StandaloneController standaloneController;

        private PatientDataFile currentDataFile;

        public PatientDataController(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            this.medicalStateController = standaloneController.MedicalStateController;
            this.examController = standaloneController.ExamController;
            this.medicalController = standaloneController.MedicalController;
        }

        public void clearData()
        {
            CurrentData = null;
        }

        public void saveMedicalState(PatientDataFile dataFile)
        {
            dataFile.PatientData.MedicalStates = medicalStateController.getSavedState(medicalController.CurrentSceneFile);
            examController.addExamsToData(dataFile.PatientData);
            dataFile.save();
            CurrentData = dataFile;
        }

        public void openPatientFile(PatientDataFile dataFile)
        {
            if (dataFile.loadData())
            {
                SavedMedicalStates states = dataFile.PatientData.MedicalStates;
                if (states != null)
                {
                    standaloneController.changeScene(MedicalConfig.SceneDirectory + "/" + states.SceneName);
                    medicalStateController.setStates(states);
                    medicalStateController.blend(0.0f);
                }
                else
                {
                    MessageBox.show(String.Format("Error loading file {0}.\nCould not read state information.", dataFile.BackingFile), "Load Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                }
                examController.setExamsFromData(dataFile.PatientData);
                CurrentData = dataFile;
            }
            else
            {
                MessageBox.show(String.Format("Error loading file {0}.\nCould not load patient data.", dataFile.BackingFile), "Load Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
        }

        public PatientDataFile CurrentData
        {
            get
            {
                return currentDataFile;
            }
            private set
            {
                if (currentDataFile != value)
                {
                    currentDataFile = value;
                    if (PatientDataChanged != null)
                    {
                        PatientDataChanged.Invoke(currentDataFile);
                    }
                }
            }
        }
    }
}
