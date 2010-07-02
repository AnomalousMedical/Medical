using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving.XMLSaver;
using System.IO;
using System.Xml;
using Logging;
using MyGUIPlugin;

namespace Medical.GUI
{
    class DiscSpacePanel : StateWizardPanel
    {
        private Vector3 openingStateOffset = Vector3.Zero;

        private String currentPresetDirectory;
        private static XmlSaver xmlSaver = new XmlSaver();
        private PresetState presetState;
        private String defaultLayerState;
        private String subDirectory;
        private bool showing = false;

        private DiscSpaceControl discSpaceControl;
        private CheckButton showDiscCheckBox;

        public DiscSpacePanel(String defaultLayerState, String subDirectory, String panelFile, StateWizardPanelController controller)
            : base(panelFile, controller)
        {
            this.defaultLayerState = defaultLayerState;
            this.subDirectory = subDirectory;

            discSpaceControl = new DiscSpaceControl(mainWidget);
            discSpaceControl.CurrentStageChanged += new EventHandler(discSpaceControl1_CurrentStageChanged);

            showDiscCheckBox = new CheckButton(mainWidget.findWidget("DiscSpace/ShowDisc") as Button);
            showDiscCheckBox.CheckedChanged += new MyGUIEvent(showDiscCheckBox_CheckedChanged);
        }

        void discSpaceControl1_CurrentStageChanged(object sender, EventArgs e)
        {
            String filename = "I.dst";
            switch (discSpaceControl.CurrentStage)
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
                    switch (discSpaceControl.CurrentClockFace)
                    {
                        case ClockFace.Clock12:
                            filename = "IVa12.dst";
                            break;
                        case ClockFace.Clock11:
                            filename = "IVa11.dst";
                            break;
                        case ClockFace.Clock10:
                            switch (discSpaceControl.CurrentReduction)
                            {
                                case RdaReduction.Mild:
                                    filename = "IVa10MildRDAReduction.dst";
                                    break;
                                case RdaReduction.Moderate:
                                    filename = "IVa10ModerateRDAReduction.dst";
                                    break;
                                case RdaReduction.Severe:
                                    filename = "IVa10SevereRDAReduction.dst";
                                    break;
                            }
                            break;
                    }
                    break;
                case PiperStage.IVb:
                    filename = "IVb.dst";
                    break;
                case PiperStage.Va:
                    filename = "Va.dst";
                    break;
                case PiperStage.Vb:
                    filename = "Vb.dst";
                    break;
            }
            filename = String.Format("{0}/{1}/{2}", currentPresetDirectory, subDirectory, filename);
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

        public override void resetToOpeningState()
        {
            discSpaceControl.setToDefault();
        }

        public override void applyToState(MedicalState state)
        {
            if (presetState != null)
            {
                presetState.applyToState(state);
            }
        }

        public override void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            String presetDirectory = medicalController.CurrentSceneDirectory + '/' + simScene.PresetDirectory;
            if (currentPresetDirectory != presetDirectory)
            {
                currentPresetDirectory = presetDirectory;
                discSpaceControl.setToDefault();
                String filename = String.Format("{0}/{1}/{2}", currentPresetDirectory, subDirectory, "I.dst");
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
                    Log.Error("Cannot load disc space distortion file {0}.", filename);
                }
            }
        }

        protected override void onPanelOpening()
        {
            showDiscCheckBox.Checked = false;
            showing = true;
        }

        protected override void onPanelClosing()
        {
            showing = false;
        }

        public override void setToDefault()
        {
            discSpaceControl.setToDefault();
        }

        void showDiscCheckBox_CheckedChanged(Widget source, EventArgs e)
        {
            if (showing)
            {
                if (showDiscCheckBox.Checked)
                {
                    controller.setLayerState("DiscLayers");
                }
                else
                {
                    controller.setLayerState(LayerState);
                }
            }
        }
    }
}
