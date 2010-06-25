using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving.XMLSaver;
using Engine;
using System.IO;
using System.Xml;
using Logging;
using MyGUIPlugin;

namespace Medical.GUI
{
    class DopplerPanel : StateWizardPanel
    {
        private DopplerControl dopplerControl;

        private String currentPresetDirectory;
        private String presetSubDirectory;
        private static XmlSaver xmlSaver = new XmlSaver();
        private PresetState presetState;
        //private MovementSequenceController movementSequenceController;
        private String currentSequenceDirectory;
        //private MovementSequence movementSequence;
        //private MovementSequence previousSequence; //The sequence loaded when the panel was opened.
        private String lateralJointCameraName;
        private String superiorJointCameraName;
        private bool panelOpen = false;

        //UI
        private Button lateralJointCameraButton;
        private Button superiorJointCameraButton;
        private Button bothJointsCameraButton;

        public DopplerPanel(String panelFile, StateWizardPanelController controller, String presetSubDirectory, String jointCameraName, String superiorJointCameraName/*, MovementSequenceController movementSequenceController*/)
            : base(panelFile, controller)
        {
            dopplerControl = new DopplerControl(mainWidget);

            this.lateralJointCameraName = jointCameraName;
            this.superiorJointCameraName = superiorJointCameraName;
            dopplerControl.CurrentStageChanged += new EventHandler(dopplerControl1_CurrentStageChanged);
            this.presetSubDirectory = presetSubDirectory;
            //this.movementSequenceController = movementSequenceController;

            lateralJointCameraButton = mainWidget.findWidget("DopplerPanel/LateralJointCamera") as Button;
            superiorJointCameraButton = mainWidget.findWidget("DopplerPanel/SuperiorJointCamera") as Button;
            bothJointsCameraButton = mainWidget.findWidget("DopplerPanel/BothJointsCamera") as Button;

            lateralJointCameraButton.MouseButtonClick += new MyGUIEvent(lateralJointCameraButton_MouseButtonClick);
            superiorJointCameraButton.MouseButtonClick += new MyGUIEvent(superiorJointCameraButton_MouseButtonClick);
            bothJointsCameraButton.MouseButtonClick += new MyGUIEvent(bothJointsCameraButton_MouseButtonClick);
        }

        public override void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            String newSequenceDir = medicalController.CurrentSceneDirectory + '/' + simScene.SequenceDirectory;
            if (currentSequenceDirectory != newSequenceDir)
            {
                currentSequenceDirectory = newSequenceDir;
                //movementSequence = movementSequenceController.loadSequence(currentSequenceDirectory + "/Doppler.seq");
            }
            String presetDirectory = medicalController.CurrentSceneDirectory + '/' + simScene.PresetDirectory;
            if (currentPresetDirectory != presetDirectory)
            {
                currentPresetDirectory = presetDirectory;
                dopplerControl.setToDefault();
                String filename = String.Format("{0}/{1}/{2}", currentPresetDirectory, presetSubDirectory, "I.dst");
                VirtualFileSystem archive = VirtualFileSystem.Instance;
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

        void dopplerControl1_CurrentStageChanged(object sender, EventArgs e)
        {
            if (panelOpen)
            {
                String filename = "I.dst";
                switch (dopplerControl.CurrentStage)
                {
                    case PiperStage.I:
                        filename = "I.dst";
                        break;
                    case PiperStage.II:
                        filename = "II.dst";
                        break;
                    case PiperStage.IIIa:
                        filename = "IIIa.dst";
                        break;
                    case PiperStage.IIIb:
                        filename = "IIIb.dst";
                        break;
                    case PiperStage.IVa:
                        switch (dopplerControl.CurrentReduction)
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
                    case PiperStage.IVb:
                        switch (dopplerControl.CurrentReduction)
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
                    case PiperStage.Va:
                        filename = "Va.dst";
                        break;
                    case PiperStage.Vb:
                        filename = "Vb.dst";
                        break;
                }
                filename = String.Format("{0}/{1}/{2}", currentPresetDirectory, presetSubDirectory, filename);
                VirtualFileSystem archive = VirtualFileSystem.Instance;
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
            dopplerControl.setToDefault();
        }

        public override void recordOpeningState()
        {
            dopplerControl.setToDefault();
        }

        public override void applyToState(MedicalState state)
        {
            if (presetState != null)
            {
                presetState.applyToState(state);
            }
        }

        //public override void modifyScene()
        //{
        //    previousSequence = movementSequenceController.CurrentSequence;
        //    movementSequenceController.CurrentSequence = movementSequence;
        //    movementSequenceController.playCurrentSequence();
        //}

        protected override void onPanelOpening()
        {
            panelOpen = true;
        }

        protected override void onPanelClosing()
        {
            panelOpen = false;
            //movementSequenceController.stopPlayback();
            //movementSequenceController.CurrentSequence = previousSequence;
            //previousSequence = null;
        }

        void bothJointsCameraButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.setNavigationState("WizardBothTMJSuperior");
            this.setLayerState("JointMenuLayers");
        }

        void superiorJointCameraButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.setNavigationState(superiorJointCameraName);
            this.setLayerState("JointMenuLayers");
        }

        void lateralJointCameraButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.setNavigationState(lateralJointCameraName);
            this.setLayerState("DiscLayers");
        }
    }
}
