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
                    if (exportGrowthDefect.Checked)
                    {
                        compoundState.addSubState(getGrowth("left", state));
                    }
                    if (exportMandibularDegenration.Checked)
                    {
                        compoundState.addSubState(getDegeneration("left", state));
                    }

                    if (exportDisc.Checked)
                    {
                        DiscPresetState leftDisc = new DiscPresetState("LeftTMJDisc", "LeftTMJDisc", "", "");
                        leftDisc.captureFromState(state.Disc.getPosition("LeftTMJDisc"));
                        compoundState.addSubState(leftDisc);
                    }

                    if (exportFossa.Checked)
                    {
                        FossaPresetState leftFossaState = new FossaPresetState("LeftFossa", "", "");
                        leftFossaState.captureFromState("LeftFossa", state.Fossa);
                        compoundState.addSubState(leftFossaState);
                    }
                }

                if (exportRightCheck.Checked)
                {
                    if (exportGrowthDefect.Checked)
                    {
                        compoundState.addSubState(getGrowth("right", state));
                    }
                    if (exportMandibularDegenration.Checked)
                    {
                        compoundState.addSubState(getDegeneration("right", state));
                    }

                    if (exportDisc.Checked)
                    {
                        DiscPresetState rightDisc = new DiscPresetState("RightTMJDisc", "RightTMJDisc", "", "");
                        rightDisc.captureFromState(state.Disc.getPosition("RightTMJDisc"));
                        compoundState.addSubState(rightDisc);
                    }

                    if (exportFossa.Checked)
                    {
                        FossaPresetState rightFossaState = new FossaPresetState("RightFossa", "", "");
                        rightFossaState.captureFromState("RightFossa", state.Fossa);
                        compoundState.addSubState(rightFossaState);
                    }
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

            if (exportRamusHeight.Checked)
            {
                presetBones.captureFromState(sourceState.BoneManipulator.getEntry(ramusHeight));
            }
            if (exportCondyleHeight.Checked)
            {
                presetBones.captureFromState(sourceState.BoneManipulator.getEntry(condyleHeight));
            }
            if (exportCondyleRotation.Checked)
            {
                presetBones.captureFromState(sourceState.BoneManipulator.getEntry(condyleRotation));
            }
            if (exportMandibularNotch.Checked)
            {
                presetBones.captureFromState(sourceState.BoneManipulator.getEntry(mandibluarNotch));
            }
            if (exportAntegonialNotch.Checked)
            {
                presetBones.captureFromState(sourceState.BoneManipulator.getEntry(antegonialNotch));
            }

            return presetBones;
        }

        private AnimationManipulatorPresetState getDegeneration(String baseName, MedicalState sourceState)
        {
            AnimationManipulatorPresetState presetBones = new AnimationManipulatorPresetState(baseName + "Degeneration", "", "");
            String condyleDegenerationMandible = baseName + "CondyleDegenerationMandible";
            String lateralPoleMandible = baseName + "LateralPoleMandible";
            String medialPoleScale = baseName + "MedialPoleScaleMandible";
            String roughnessPose = baseName + "CondyleRoughnessMandible";

            if (exportCondyleDegeneration.Checked)
            {
                presetBones.captureFromState(sourceState.BoneManipulator.getEntry(condyleDegenerationMandible));
            }
            if (exportLateralPoleDegeneration.Checked)
            {
                presetBones.captureFromState(sourceState.BoneManipulator.getEntry(lateralPoleMandible));
            }
            if (exportMedialPoleDegeneration.Checked)
            {
                presetBones.captureFromState(sourceState.BoneManipulator.getEntry(medialPoleScale));
            }
            if (exportCondyleRoughness.Checked)
            {
                presetBones.captureFromState(sourceState.BoneManipulator.getEntry(roughnessPose));
            }

            return presetBones;
        }
    }
}
