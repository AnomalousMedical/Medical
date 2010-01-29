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
        private String discName;
        private Disc disc;
        private Vector3 openingStateOffset = Vector3.Zero;

        private String currentPresetDirectory;
        private static XmlSaver xmlSaver = new XmlSaver();
        private PresetState presetState;
        private String defaultLayerState;
        private String subDirectory;

        public DiscSpacePanel(String discName, String defaultLayerState, String subDirectory, StatePickerPanelController panelController)
            :base(panelController)
        {
            InitializeComponent();
            this.discName = discName;
            this.defaultLayerState = defaultLayerState;
            this.subDirectory = subDirectory;
            discOffsetSlider.ValueChanged += new EventHandler(discOffsetSlider_ValueChanged);
            dopplerControl1.Enabled = false;
            dopplerDataCheck.CheckedChanged += new EventHandler(dopplerDataCheck_CheckedChanged);
            dopplerControl1.CurrentStageChanged += new EventHandler(dopplerControl1_CurrentStageChanged);
        }

        public override void recordOpeningState()
        {
            discOffsetSlider.Value = (int)(disc.DiscOffset.y * -discOffsetSlider.Maximum);

            openingStateOffset = disc.DiscOffset;
            dopplerDataCheck.Checked = false;
            dopplerControl1.setToDefault();
        }

        public override void resetToOpeningState()
        {
            disc.DiscOffset = openingStateOffset;
        }

        public Image BoneOnBoneImage
        {
            get
            {
                return normalPanel.BackgroundImage;
            }
            set
            {
                normalPanel.BackgroundImage = value;
            }
        }

        public Image OpenImage
        {
            get
            {
                return distortedPanel.BackgroundImage;
            }
            set
            {
                distortedPanel.BackgroundImage = value;
            }
        }

        void dopplerDataCheck_CheckedChanged(object sender, EventArgs e)
        {
            dopplerControl1.Enabled = dopplerDataCheck.Checked;
            if (dopplerDataCheck.Checked)
            {
                this.setLayerState("DiscLayers");
                LayerState = "DiscLayers";
            }
            else
            {
                this.setLayerState(defaultLayerState);
                LayerState = defaultLayerState;
            }
        }

        void discOffsetSlider_ValueChanged(object sender, EventArgs e)
        {
            //disc.PopLocation = disc.NormalPopLocation;
            //disc.PopLocation = disc.NormalPopLocation;
            //disc.Locked = false;
            Vector3 newOffset = disc.NormalDiscOffset;
            newOffset.y = discOffsetSlider.Value / -(float)discOffsetSlider.Maximum;
            disc.DiscOffset = newOffset;
            disc.RDAOffset = newOffset;
        }

        public override void applyToState(MedicalState state)
        {
            state.Disc.addPosition(new DiscStateProperties(disc));
            presetState.applyToState(state);

            Vector3 newOffset = disc.NormalDiscOffset;
            newOffset.y = discOffsetSlider.Value / -(float)discOffsetSlider.Maximum;
            DiscStateProperties discState = state.Disc.getPosition(discName);
            discState.DiscOffset = newOffset;
            discState.RDAOffset = newOffset;
        }

        public override void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            disc = DiscController.getDisc(discName);
            String presetDirectory = medicalController.CurrentSceneDirectory + '/' + simScene.PresetDirectory + '/' + subDirectory;
            if (currentPresetDirectory != presetDirectory)
            {
                currentPresetDirectory = presetDirectory;
                dopplerControl1.setToDefault();
                String filename = String.Format("{0}/{1}", currentPresetDirectory, "I.dst");
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

        protected override void onPanelOpening()
        {
            discOffsetSlider.Value = (int)(disc.DiscOffset.y * -discOffsetSlider.Maximum);
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            resetToOpeningState();
            onPanelOpening();
        }

        private void makeNormalButton_Click(object sender, EventArgs e)
        {
            disc.DiscOffset = disc.NormalDiscOffset;
            onPanelOpening();
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
                    switch (dopplerControl1.CurrentReduction)
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
            filename = String.Format("{0}/{1}", currentPresetDirectory, filename);
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
    }
}
