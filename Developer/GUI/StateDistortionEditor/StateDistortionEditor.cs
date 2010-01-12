using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.Saving.XMLSaver;
using System.Xml;
using System.IO;

namespace Medical.GUI
{
    public partial class StateDistortionEditor : GUIElement
    {
        private MedicalStateController stateController;
        private XmlSaver xmlSaver = new XmlSaver();

        public StateDistortionEditor(MedicalStateController stateController, ImageRenderer imageRenderer)
        {
            InitializeComponent();
            this.stateController = stateController;
            picturePreviewPanel.initialize(imageRenderer, 100, 100);
            picturePreviewPanel.ImageProperties.AntiAliasingMode = 16;
            picturePreviewPanel.ImageProperties.TransparentBackground = true;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog(this.FindForm()) == DialogResult.OK)
            {
                MedicalState state = stateController.createState("Predefined");
                String distortionFile = saveFileDialog.FileName;
                String fileName = Path.GetFileNameWithoutExtension(distortionFile);

                CompoundPresetState compoundState = new CompoundPresetState(fileName, "", fileName + ".png");

                if (exportLeftCheck.Checked)
                {
                    compoundState.addSubState(getGrowth("left", state));
                    compoundState.addSubState(getDegeneration("left", state));

                    DiscPresetState leftDisc = new DiscPresetState("LeftTMJDisc", "LeftTMJDisc", "", "");
                    leftDisc.captureFromState(state.Disc.getPosition("LeftTMJDisc"));
                    compoundState.addSubState(leftDisc);

                    FossaPresetState leftFossaState = new FossaPresetState("LeftFossa", "", "");
                    leftFossaState.captureFromState("LeftFossa", state.Fossa);
                    compoundState.addSubState(leftFossaState);
                }

                if (exportRightCheck.Checked)
                {
                    compoundState.addSubState(getGrowth("right", state));
                    compoundState.addSubState(getDegeneration("right", state));

                    DiscPresetState rightDisc = new DiscPresetState("RightTMJDisc", "RightTMJDisc", "", "");
                    rightDisc.captureFromState(state.Disc.getPosition("RightTMJDisc"));
                    compoundState.addSubState(rightDisc);

                    FossaPresetState rightFossaState = new FossaPresetState("RightFossa", "", "");
                    rightFossaState.captureFromState("RightFossa", state.Fossa);
                    compoundState.addSubState(rightFossaState);
                }

                if (exportTeethCheck.Checked)
                {
                    TeethPresetState teethPreset = new TeethPresetState("Teeth", "", "");
                    teethPreset.captureFromState(state.Teeth);
                    compoundState.addSubState(teethPreset);
                }

                using (XmlTextWriter textWriter = new XmlTextWriter(distortionFile, Encoding.Default))
                {
                    textWriter.Formatting = Formatting.Indented;
                    xmlSaver.saveObject(compoundState, textWriter);
                }

                if (saveThumbnailCheck.Checked)
                {
                    String imageFile = distortionFile.Replace(".dst", ".png");
                    picturePreviewPanel.saveBitmap(imageFile);
                }
            }
        }

        private AnimationManipulatorPresetState getGrowth(String baseName, MedicalState sourceState)
        {
            AnimationManipulatorPresetState presetBones = new AnimationManipulatorPresetState(baseName + "Growth", "", "");
            String ramusHeight = baseName + "RamusHeightMandible";
            String condyleHeight = baseName + "CondyleHeightMandible";
            String condyleRotation = baseName + "CondyleRotationMandible";
            String mandibluarNotch = baseName + "MandibularNotchMandible";
            String antegonialNotch = baseName + "AntegonialNotchMandible";

            presetBones.captureFromState(sourceState.BoneManipulator.getEntry(ramusHeight));
            presetBones.captureFromState(sourceState.BoneManipulator.getEntry(condyleHeight));
            presetBones.captureFromState(sourceState.BoneManipulator.getEntry(condyleRotation));
            presetBones.captureFromState(sourceState.BoneManipulator.getEntry(mandibluarNotch));
            presetBones.captureFromState(sourceState.BoneManipulator.getEntry(antegonialNotch));

            return presetBones;
        }

        private AnimationManipulatorPresetState getDegeneration(String baseName, MedicalState sourceState)
        {
            AnimationManipulatorPresetState presetBones = new AnimationManipulatorPresetState(baseName + "Degeneration", "", "");
            String condyleDegenerationMandible = baseName + "CondyleDegenerationMandible";
            String lateralPoleMandible = baseName + "LateralPoleMandible";
            String medialPoleScale = baseName + "MedialPoleScaleMandible";
            String roughnessPose = baseName + "CondyleRoughnessMandible";

            presetBones.captureFromState(sourceState.BoneManipulator.getEntry(condyleDegenerationMandible));
            presetBones.captureFromState(sourceState.BoneManipulator.getEntry(lateralPoleMandible));
            presetBones.captureFromState(sourceState.BoneManipulator.getEntry(medialPoleScale));
            presetBones.captureFromState(sourceState.BoneManipulator.getEntry(roughnessPose));

            return presetBones;
        }
    }
}
