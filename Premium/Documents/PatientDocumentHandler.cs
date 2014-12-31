﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;
using Anomalous.GuiFramework;

namespace Medical
{
    class PatientDocumentHandler : DocumentHandler
    {
        private StandaloneController standaloneController;

        public PatientDocumentHandler(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
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
                standaloneController.PatientDataController.openPatientFile(patientData);
            }
            else
            {
                MessageBox.show(String.Format("Error loading file {0}.", patientData.BackingFile), "Load Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
            return false;
        }

        public string getPrettyName(string filename)
        {
            return "Patient";
        }

        public string getIcon(string filename)
        {
            return "PremiumFeatures/PatientDataIcon";
        }
    }
}