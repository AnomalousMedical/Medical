using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.Resources;
using System.IO;
using System.Xml;
using Engine.Saving.XMLSaver;
using Logging;
using Medical.Controller;
using Medical.Muscles;

namespace Medical.GUI
{
    /// <summary>
    /// A panel for the doppler in a state wizard.
    /// </summary>
    public partial class DopplerPanel : StatePickerPanel
    {
        private String currentPresetDirectory;
        private String presetSubDirectory;
        private static XmlSaver xmlSaver = new XmlSaver();
        private PresetState presetState;
        private MovementSequenceController movementSequenceController;
        private String currentSequenceDirectory;
        private MovementSequence movementSequence;
        private MovementSequence previousSequence; //The sequence loaded when the panel was opened.
        private String lateralJointCameraName;
        private String superiorJointCameraName;

        public DopplerPanel(String presetSubDirectory, String jointCameraName, String superiorJointCameraName, MovementSequenceController movementSequenceController, StatePickerPanelController panelController)
            :base(panelController)
        {
            InitializeComponent();
            this.lateralJointCameraName = jointCameraName;
            this.superiorJointCameraName = superiorJointCameraName;
            dopplerControl1.CurrentStageChanged += new EventHandler(dopplerControl1_CurrentStageChanged);
            this.presetSubDirectory = presetSubDirectory;
            this.movementSequenceController = movementSequenceController;
        }

        public override void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            String newSequenceDir = medicalController.CurrentSceneDirectory + '/' + simScene.SequenceDirectory;
            if (currentSequenceDirectory != newSequenceDir)
            {
                currentSequenceDirectory = newSequenceDir;
                movementSequence = movementSequenceController.loadSequence(currentSequenceDirectory + "/Doppler.seq");
            }
            String presetDirectory = medicalController.CurrentSceneDirectory + '/' + simScene.PresetDirectory;
            if (currentPresetDirectory != presetDirectory)
            {
                currentPresetDirectory = presetDirectory;
                dopplerControl1.setToDefault();
                String filename = String.Format("{0}/{1}/{2}", currentPresetDirectory, presetSubDirectory, "I.dst");
                using (Archive archive = FileSystem.OpenArchive(filename))
                {
                    if (archive.exists(filename))
                    {
                        using (Stream stream = archive.openStream(filename, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                        {
                            using (XmlTextReader textReader = new XmlTextReader(stream))
                            {
                                presetState = xmlSaver.restoreObject(textReader) as PresetState;
                            }
                        }
                    }
                    else
                    {
                        presetState = null;
                        Log.Error("Cannot load doppler distortion file {0}.", filename);
                    }
                }
            }
        }

        void dopplerControl1_CurrentStageChanged(object sender, EventArgs e)
        {
            String filename = "I.dst";
            switch (dopplerControl1.CurrentStage)
            {
                case DopplerStage.I:
                    filename = "I.dst";
                    break;
                case DopplerStage.II:
                    filename = "II.dst";
                    break;
                case DopplerStage.IIIa:
                    filename = "IIIa.dst";
                    break;
                case DopplerStage.IIIb:
                    filename = "IIIb.dst";
                    break;
                case DopplerStage.IVa:
                    switch(dopplerControl1.CurrentReduction)
                    {
                        case RdaReduction.Mild:
                            filename = "IVaMildRDAReduction.dst";
                            break;
                        case RdaReduction.Moderate:
                            filename = "IVaModerateRDAReduction.dst";
                            break;
                        case RdaReduction.Severe:
                            filename = "IVaSevereRDAReduction.dst";
                            break;
                    }
                    break;
                case DopplerStage.IVb:
                    switch (dopplerControl1.CurrentReduction)
                    {
                        case RdaReduction.Mild:
                            filename = "IVbMildRDAReduction.dst";
                            break;
                        case RdaReduction.Moderate:
                            filename = "IVbModerateRDAReduction.dst";
                            break;
                        case RdaReduction.Severe:
                            filename = "IVbSevereRDAReduction.dst";
                            break;
                    }
                    break;
                case DopplerStage.Va:
                    filename = "Va.dst";
                    break;
                case DopplerStage.Vb:
                    filename = "Vb.dst";
                    break;
            }
            filename = String.Format("{0}/{1}/{2}", currentPresetDirectory, presetSubDirectory, filename);
            using (Archive archive = FileSystem.OpenArchive(filename))
            {
                if (archive.exists(filename))
                {
                    using (Stream stream = archive.openStream(filename, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                    {
                        using (XmlTextReader textReader = new XmlTextReader(stream))
                        {
                            presetState = xmlSaver.restoreObject(textReader) as PresetState;
                            showChanges(false);
                        }
                    }
                }
                else
                {
                    presetState = null;
                    Log.Error("Cannot load doppler distortion file {0}.", filename);
                }
            }
            
        }

        public override void setToDefault()
        {
            dopplerControl1.setToDefault();
        }

        public override void recordOpeningState()
        {
            dopplerControl1.setToDefault();
        }

        public override void applyToState(MedicalState state)
        {
            if (presetState != null)
            {
                presetState.applyToState(state);
            }
        }

        public override void modifyScene()
        {
            previousSequence = movementSequenceController.CurrentSequence;
            movementSequenceController.CurrentSequence = movementSequence;
            movementSequenceController.playCurrentSequence();
        }

        protected override void onPanelClosing()
        {
            movementSequenceController.stopPlayback();
            movementSequenceController.CurrentSequence = previousSequence;
            previousSequence = null;
        }

        private void lateralJointCamerButton_Click(object sender, EventArgs e)
        {
            this.setNavigationState(lateralJointCameraName);
            this.setLayerState(LayerState);
        }

        private void superiorJointCameraButton_Click(object sender, EventArgs e)
        {
            this.setNavigationState(superiorJointCameraName);
            this.setLayerState("JointMenuLayers");
        }

        private void bothJointsCameraButton_Click(object sender, EventArgs e)
        {
            this.setNavigationState("WizardBothTMJSuperior");
            this.setLayerState("JointMenuLayers");
        }
    }
}
