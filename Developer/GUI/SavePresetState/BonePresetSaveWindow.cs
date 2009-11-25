using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.Resources;
using System.IO;
using Engine.Saving.XMLSaver;
using System.Xml;
using Logging;

namespace Medical.GUI
{
    public partial class BonePresetSaveWindow : GUIElement
    {
        MedicalStateController stateController;
        private XmlSaver saver = new XmlSaver();

        public BonePresetSaveWindow()
        {
            InitializeComponent();
            typeCombo.SelectedIndex = 0;
            leftSideSource.Checked = true;
            outputDirectoryText.Text = MedicalConfig.DocRoot + "/SavedPresets";
        }

        public void initialize(ImageRenderer renderer, MedicalStateController stateController)
        {
            picturePreviewPanel.initialize(renderer);
            this.stateController = stateController;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (nameText.Text != null && nameText.Text != "" && categoryText.Text != null && categoryText.Text != "")
            {
                ensureDirectory();
                if (typeCombo.SelectedItem.ToString() == "Growth Defect")
                {
                    saveGrowthDefect();
                }
                else
                {
                    saveDegeneration();
                }
            }
            else
            {
                MessageBox.Show(this, String.Format("Please fill in the name and category text."), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveGrowthDefect()
        {
            String presetDirectory = "Left";
            if (rightSideSource.Checked)
            {
                presetDirectory = "Right";
            }
            presetDirectory = outputDirectoryText.Text + "/" + presetDirectory + "Growth";
            if (!Directory.Exists(presetDirectory))
            {
                Directory.CreateDirectory(presetDirectory);
            }
            String presetFile = presetDirectory + "/" + nameText.Text.Replace(" ", "") + ".pre";

            bool allowSave = true;
            if (File.Exists(presetFile))
            {
                allowSave = MessageBox.Show(this, String.Format("The file {0} already exists. Overwrite?", presetFile), "Overwrite?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            }

            if (allowSave)
            {
                MedicalState sourceState = stateController.createState(nameText.Text);
                BoneManipulatorPresetState presetBones = new BoneManipulatorPresetState(nameText.Text, categoryText.Text, nameText.Text.Replace(" ", "") + ".png");
                String baseName = "left";
                if (rightSideSource.Checked)
                {
                    baseName = "right";
                }
                String ramusHeight = baseName + "RamusHeightMandible";
                String condyleHeight = baseName + "CondyleHeightMandible";
                String condyleRotation = baseName + "CondyleRotationMandible";
                String mandibluarNotch = baseName + "MandibularNotchMandible";
                String antegonialNotch = baseName + "AntegonialNotchMandible";

                presetBones.addPosition(sourceState.BoneManipulator.getEntry(ramusHeight).clone());
                presetBones.addPosition(sourceState.BoneManipulator.getEntry(condyleHeight).clone());
                presetBones.addPosition(sourceState.BoneManipulator.getEntry(condyleRotation).clone());
                presetBones.addPosition(sourceState.BoneManipulator.getEntry(mandibluarNotch).clone());
                presetBones.addPosition(sourceState.BoneManipulator.getEntry(antegonialNotch).clone());

                try
                {
                    using (XmlTextWriter textWriter = new XmlTextWriter(presetFile, Encoding.Default))
                    {
                        textWriter.Formatting = Formatting.Indented;
                        saver.saveObject(presetBones, textWriter);
                    }
                    picturePreviewPanel.saveBitmap(presetDirectory + "/" + presetBones.ImageName);
                    MessageBox.Show(this, String.Format("Saved preset to {0}.", presetFile), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (IOException e)
                {
                    MessageBox.Show(this, String.Format("Error saving preset file.\nReason: {0}", e.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void saveDegeneration()
        {
            String presetDirectory = "Left";
            if (rightSideSource.Checked)
            {
                presetDirectory = "Right";
            }
            presetDirectory = outputDirectoryText.Text + "/" + presetDirectory + "Degeneration";
            if (!Directory.Exists(presetDirectory))
            {
                Directory.CreateDirectory(presetDirectory);
            }
            String presetFile = presetDirectory + "/" + nameText.Text.Replace(" ", "") + ".pre";

            bool allowSave = true;
            if (File.Exists(presetFile))
            {
                allowSave = MessageBox.Show(this, String.Format("The file {0} already exists. Overwrite?", presetFile), "Overwrite?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            }

            if (allowSave)
            {
                MedicalState sourceState = stateController.createState(nameText.Text);
                BoneManipulatorPresetState presetBones = new BoneManipulatorPresetState(nameText.Text, categoryText.Text, nameText.Text.Replace(" ", "") + ".png");
                String baseName = "left";
                if (rightSideSource.Checked)
                {
                    baseName = "right";
                }
                String condyleDegenerationMandible = baseName + "CondyleDegenerationMandible";
                String lateralPoleMandible = baseName + "LateralPoleMandible";
                String medialPoleScale = baseName + "MedialPoleScaleMandible";

                presetBones.addPosition(sourceState.BoneManipulator.getEntry(condyleDegenerationMandible).clone());
                presetBones.addPosition(sourceState.BoneManipulator.getEntry(lateralPoleMandible).clone());
                presetBones.addPosition(sourceState.BoneManipulator.getEntry(medialPoleScale).clone());

                try
                {
                    using (XmlTextWriter textWriter = new XmlTextWriter(presetFile, Encoding.Default))
                    {
                        textWriter.Formatting = Formatting.Indented;
                        saver.saveObject(presetBones, textWriter);
                    }
                    picturePreviewPanel.saveBitmap(presetDirectory + "/" + presetBones.ImageName);
                    MessageBox.Show(this, String.Format("Saved preset to {0}.", presetFile), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (IOException e)
                {
                    MessageBox.Show(this, String.Format("Error saving preset file.\nReason: {0}", e.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void copySideToOther()
        {
            String sourceDirectory = "Left";
            String destDirectory = "Right";
            String oldName = "left";
            String newName = "right";
            if (rightSideSource.Checked)
            {
                sourceDirectory = "Right";
                destDirectory = "Left";
                oldName = "right";
                newName = "left";
            }
            String prompt = String.Format("This will copy the contents of the {0} side to the {1} side. Are you sure you want to do this", oldName, newName);
            if (MessageBox.Show(this, prompt, "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (Archive archive = FileSystem.OpenArchive(sourceDirectory))
                {
                    doCopy(outputDirectoryText.Text + "/" + sourceDirectory + "Growth", outputDirectoryText.Text + "/" + destDirectory + "Growth", oldName, newName, archive);
                    doCopy(outputDirectoryText.Text + "/" + sourceDirectory + "Degeneration", outputDirectoryText.Text + "/" + destDirectory + "Degeneration", oldName, newName, archive);
                }
                MessageBox.Show(this, "Finished copying sides.", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void doCopy(String sourceDirectory, String destDirectory, String oldName, String newName, Archive archive)
        {
            if (!Directory.Exists(sourceDirectory))
            {
                Directory.CreateDirectory(sourceDirectory);
            }
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }
            String[] files = archive.listFiles(sourceDirectory, "*.pre", false);
            foreach (String file in files)
            {
                using (XmlTextReader reader = new XmlTextReader(archive.openStream(file, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read)))
                {
                    BoneManipulatorPresetState preset = saver.restoreObject(reader) as BoneManipulatorPresetState;
                    if (preset != null)
                    {
                        preset.changeSide(oldName, newName);
                        using (Stream stream = archive.openStream(sourceDirectory + "/" + preset.ImageName, Engine.Resources.FileMode.Open))
                        {
                            using (Bitmap bitmap = new Bitmap(stream))
                            {
                                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                bitmap.Save(destDirectory + "/" + preset.ImageName);
                            }
                        }
                        using (XmlTextWriter textWriter = new XmlTextWriter(destDirectory + "/" + file.Substring(sourceDirectory.Length), Encoding.ASCII))
                        {
                            textWriter.Formatting = Formatting.Indented;
                            saver.saveObject(preset, textWriter);
                        }
                    }
                    else
                    {
                        Log.Error("Could not load preset from file {0}. Object was not a BoneManipulatorPresetState.", file);
                    }
                }
            }
        }

        private void ensureDirectory()
        {
            if (!Directory.Exists(outputDirectoryText.Text))
            {
                Directory.CreateDirectory(outputDirectoryText.Text);
            }
        }

        private void outDirBrowseButton_Click(object sender, EventArgs e)
        {
            ensureDirectory();
            folderBrowserDialog.SelectedPath = outputDirectoryText.Text;
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                outputDirectoryText.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void copySideButton_Click(object sender, EventArgs e)
        {
            copySideToOther();
        }
    }
}
