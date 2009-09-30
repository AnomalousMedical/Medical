using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using Engine.Saving.XMLSaver;
using Engine.Resources;
using Logging;

namespace Medical.GUI
{
    public partial class DiscPresetEditor : GUIElement
    {
        private MedicalStateController stateController;
        private XmlSaver saver = new XmlSaver();

        public DiscPresetEditor()
        {
            InitializeComponent();
            outputDirectoryText.Text = MedicalConfig.DocRoot + "/SavedPresets";
        }

        public void initialize(ImageRenderer imageRenderer, MedicalStateController stateController)
        {
            picturePreviewPanel.initialize(imageRenderer);
            this.stateController = stateController;
        }

        private void copySideButton_Click(object sender, EventArgs e)
        {
            copySideToOther();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            save();
        }

        private void save()
        {
            String sideName = "Left";
            if (rightSideSource.Checked)
            {
                sideName = "Right";
            }
            String presetDirectory = outputDirectoryText.Text + "/" + sideName + "Disc";
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
                String discName = sideName + "TMJDisc";
                MedicalState sourceState = stateController.createState(nameText.Text);
                DiscStateProperties discSource = sourceState.Disc.getPosition(discName);
                                
                DiscPresetState discPreset = new DiscPresetState(discName, nameText.Text, categoryText.Text, nameText.Text.Replace(" ", "") + ".png");
                discPreset.DiscOffset = discSource.DiscOffset;
                discPreset.HorizontalOffset = discSource.HorizontalOffset;
                discPreset.Locked = discSource.Locked;
                discPreset.PopLocation = discSource.PopLocation;
                discPreset.RdaOffset = discSource.RDAOffset;

                try
                {
                    using (XmlTextWriter textWriter = new XmlTextWriter(presetFile, Encoding.Default))
                    {
                        textWriter.Formatting = Formatting.Indented;
                        saver.saveObject(discPreset, textWriter);
                    }
                    picturePreviewPanel.saveBitmap(presetDirectory + "/" + discPreset.ImageName);
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
            String oldName = "Left";
            String newName = "Right";
            if (rightSideSource.Checked)
            {
                sourceDirectory = "Right";
                destDirectory = "Left";
                oldName = "Right";
                newName = "Left";
            }
            String prompt = String.Format("This will copy the contents of the {0} side to the {1} side. Are you sure you want to do this", oldName, newName);
            if (MessageBox.Show(this, prompt, "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (Archive archive = FileSystem.OpenArchive(sourceDirectory))
                {
                    doCopy(outputDirectoryText.Text + "/" + sourceDirectory + "Disc", outputDirectoryText.Text + "/" + destDirectory + "Disc", oldName, newName, archive);
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
                    DiscPresetState preset = saver.restoreObject(reader) as DiscPresetState;
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
    }
}
