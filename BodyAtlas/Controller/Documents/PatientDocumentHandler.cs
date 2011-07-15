using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;

namespace Medical
{
    class PatientDocumentHandler : DocumentHandler
    {
        private StandaloneController standaloneController;
        private BodyAtlasMainPlugin bodyAtlasPlugin;

        public PatientDocumentHandler(StandaloneController standaloneController, BodyAtlasMainPlugin bodyAtlasPlugin)
        {
            this.standaloneController = standaloneController;
            this.bodyAtlasPlugin = bodyAtlasPlugin;
        }

        public bool canReadFile(string filename)
        {
            return filename.EndsWith(".pdt");
        }

        public bool processFile(string filename)
        {
            PatientDataFile patientData = new PatientDataFile(filename);
            if (patientData.loadHeader())
            {
                bodyAtlasPlugin.changeActiveFile(patientData);
                standaloneController.openPatientFile(patientData);
            }
            else
            {
                MessageBox.show(String.Format("Error loading file {0}.", patientData.BackingFile), "Load Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
            return false; //bodyAtlasPlugin.changeActiveFile will have already added this to the recent docs list for us.
        }

        public string getPrettyName(string filename)
        {
            return "Patient";
        }

        public string getIcon(string filename)
        {
            return "ExamIcon";
        }
    }
}