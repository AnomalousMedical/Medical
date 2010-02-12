using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.ObjectManagement;
using Engine;
using Engine.Saving.XMLSaver;
using Engine.Resources;
using System.IO;
using System.Xml;
using Logging;

namespace Medical.GUI
{
    public partial class DiscSpacePanel : StatePickerPanel
    {
        private Vector3 openingStateOffset = Vector3.Zero;

        private String currentPresetDirectory;
        private static XmlSaver xmlSaver = new XmlSaver();
        private PresetState presetState;
        private String defaultLayerState;
        private String subDirectory;
        private bool showing = false;

        public DiscSpacePanel(String defaultLayerState, String subDirectory, StatePickerPanelController panelController)
            :base(panelController)
        {
            InitializeComponent();
            this.defaultLayerState = defaultLayerState;
            this.subDirectory = subDirectory;
            discSpaceControl1.CurrentStageChanged += new EventHandler(discSpaceControl1_CurrentStageChanged);
        }

        void discSpaceControl1_CurrentStageChanged(object sender, EventArgs e)
        {
            String filename = "I.dst";
            switch (discSpaceControl1.CurrentStage)
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
                    switch(discSpaceControl1.CurrentClockFace)
                    {
                        case ClockFace.Clock12:
                            filename = "IVa12.dst";
                            break;
                        case ClockFace.Clock11:
                            filename = "IVa11.dst";
                            break;
                        case ClockFace.Clock10:
                            switch (discSpaceControl1.CurrentReduction)
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

        public override void resetToOpeningState()
        {
            discSpaceControl1.setToDefault();
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
                discSpaceControl1.setToDefault();
                String filename = String.Format("{0}/{1}/{2}", currentPresetDirectory, subDirectory, "I.dst");
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
                        Log.Error("Cannot load disc space distortion file {0}.", filename);
                    }
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
            discSpaceControl1.setToDefault();
        }

        private void showDiscCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (showing)
            {
                if (showDiscCheckBox.Checked)
                {
                    panelController.LayerController.applyLayerState("DiscLayers");
                }
                else
                {
                    panelController.LayerController.applyLayerState(LayerState);
                }
            }
        }
    }
}
