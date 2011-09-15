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
using Medical.Muscles;
using Medical.Controller;

namespace Medical.GUI
{
    public class DopplerGUI : TimelineWizardPanel
    {
        private DopplerControl dopplerControl;

        private String currentPresetDirectory;
        private String presetSubDirectory;
        private static XmlSaver xmlSaver = new XmlSaver();
        private PresetState presetState;
        //private String currentSequenceDirectory;
        //private MovementSequence movementSequence;
        //private MovementSequence previousSequence; //The sequence loaded when the panel was opened.
        //private bool panelOpen = false;

        //UI
        private Button lateralJointCameraButton;
        private Button superiorJointCameraButton;
        private Button bothJointsCameraButton;

        public DopplerGUI(String presetSubDirectory, TimelineWizard wizard)
            : base("Medical.TimelineGUI.Panels.Doppler.DopplerGUI.layout", wizard)
        {
            dopplerControl = new DopplerControl(widget);

            dopplerControl.CurrentStageChanged += new EventHandler(dopplerControl_CurrentStageChanged);
            this.presetSubDirectory = presetSubDirectory;

            lateralJointCameraButton = widget.findWidget("DopplerPanel/LateralJointCamera") as Button;
            superiorJointCameraButton = widget.findWidget("DopplerPanel/SuperiorJointCamera") as Button;
            bothJointsCameraButton = widget.findWidget("DopplerPanel/BothJointsCamera") as Button;

            lateralJointCameraButton.MouseButtonClick += new MyGUIEvent(lateralJointCameraButton_MouseButtonClick);
            superiorJointCameraButton.MouseButtonClick += new MyGUIEvent(superiorJointCameraButton_MouseButtonClick);
            bothJointsCameraButton.MouseButtonClick += new MyGUIEvent(bothJointsCameraButton_MouseButtonClick);
        }

        public override void opening(MedicalController medicalController, SimulationScene simScene)
        {
            //String newSequenceDir = medicalController.CurrentSceneDirectory + '/' + simScene.SequenceDirectory;
            //if (currentSequenceDirectory != newSequenceDir)
            //{
            //    currentSequenceDirectory = newSequenceDir;
            //    movementSequence = controller.loadSequence(currentSequenceDirectory + "/Doppler.seq");
            //}
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

        void dopplerControl_CurrentStageChanged(object sender, EventArgs e)
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
                        timelineWizard.applyPresetState(presetState);
                    }
                }
            }
            else
            {
                presetState = null;
                Log.Error("Cannot load doppler distortion file {0}.", filename);
            }
        }

        void bothJointsCameraButton_MouseButtonClick(Widget source, EventArgs e)
        {
            DopplerGUIData dopplerData = (DopplerGUIData)PanelData;
            SceneViewWindow window = timelineWizard.SceneViewController.ActiveWindow;
            if (window != null)
            {
                window.setPosition(dopplerData.BothJointsCamera);
            }
            applyLayers(dopplerData.NormalLayers);
        }

        void superiorJointCameraButton_MouseButtonClick(Widget source, EventArgs e)
        {
            DopplerGUIData dopplerData = (DopplerGUIData)PanelData;
            SceneViewWindow window = timelineWizard.SceneViewController.ActiveWindow;
            if (window != null)
            {
                window.setPosition(dopplerData.SuperiorJointCamera);
            }
            applyLayers(dopplerData.NormalLayers);
        }

        void lateralJointCameraButton_MouseButtonClick(Widget source, EventArgs e)
        {
            DopplerGUIData dopplerData = (DopplerGUIData)PanelData;
            SceneViewWindow window = timelineWizard.SceneViewController.ActiveWindow;
            if (window != null)
            {
                window.setPosition(dopplerData.LateralJointCamera);
            }
            applyLayers(dopplerData.LateralJointLayers);
        }

        void applyLayers(LayerState layers)
        {
            if (layers != null)
            {
                layers.apply();
            }
        }
    }
}
