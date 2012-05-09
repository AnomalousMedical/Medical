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
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class DopplerGUI : WizardPanel<DopplerView>
    {
        private DopplerControl dopplerControl;

        private String presetSubDirectory;
        private static XmlSaver xmlSaver = new XmlSaver();
        private PresetState presetState;

        //UI
        private Button lateralJointCameraButton;
        private Button superiorJointCameraButton;
        private Button bothJointsCameraButton;

        public DopplerGUI(DopplerView wizardView, AnomalousMvcContext context)
            : base("Medical.GUI.AnomalousMvc.DistortionWizard.Doppler.DopplerGUI.layout", wizardView, context)
        {
            dopplerControl = new DopplerControl(widget);

            dopplerControl.CurrentStageChanged += new EventHandler(dopplerControl_CurrentStageChanged);
            this.presetSubDirectory = wizardView.PresetDirectory;

            lateralJointCameraButton = widget.findWidget("DopplerPanel/LateralJointCamera") as Button;
            superiorJointCameraButton = widget.findWidget("DopplerPanel/SuperiorJointCamera") as Button;
            bothJointsCameraButton = widget.findWidget("DopplerPanel/BothJointsCamera") as Button;

            lateralJointCameraButton.MouseButtonClick += new MyGUIEvent(lateralJointCameraButton_MouseButtonClick);
            superiorJointCameraButton.MouseButtonClick += new MyGUIEvent(superiorJointCameraButton_MouseButtonClick);
            bothJointsCameraButton.MouseButtonClick += new MyGUIEvent(bothJointsCameraButton_MouseButtonClick);
        }

        public override void opening()
        {
            dopplerControl.setToDefault();
            String filename = String.Format("{0}/{1}", presetSubDirectory, "I.dst");
            if (context.ResourceProvider.exists(filename))
            {
                using (Stream stream = context.ResourceProvider.openFile(filename))
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
            filename = String.Format("{0}/{1}", presetSubDirectory, filename);
            if (context.ResourceProvider.exists(filename))
            {
                using (Stream stream = context.ResourceProvider.openFile(filename))
                {
                    using (XmlTextReader textReader = new XmlTextReader(stream))
                    {
                        presetState = xmlSaver.restoreObject(textReader) as PresetState;
                        context.applyPresetState(presetState, 1.0f);
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
            context.runAction(wizardView.BothJointsAction);
        }

        void superiorJointCameraButton_MouseButtonClick(Widget source, EventArgs e)
        {
            context.runAction(wizardView.SuperiorJointAction);
        }

        void lateralJointCameraButton_MouseButtonClick(Widget source, EventArgs e)
        {
            context.runAction(wizardView.LateralJointAction);
        }
    }
}
