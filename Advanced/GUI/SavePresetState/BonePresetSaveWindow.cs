using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Engine.Resources;
using System.IO;
using Engine.Saving.XMLSaver;
using System.Xml;

namespace Medical.GUI
{
    public partial class BonePresetSaveWindow : GUIElement
    {
        ImageRenderer renderer;
        Bitmap currentBitmap;
        MedicalStateController stateController;
        private XmlSaver saver = new XmlSaver();

        public BonePresetSaveWindow()
        {
            InitializeComponent();
            typeCombo.SelectedIndex = 0;
            leftSideSource.Checked = true;
        }

        public void initialize(ImageRenderer renderer, MedicalStateController stateController)
        {
            this.renderer = renderer;
            this.stateController = stateController;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (nameText.Text != null && nameText.Text != "" && categoryText.Text != null && categoryText.Text != "")
            {
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

        private void refreshImageButton_Click(object sender, EventArgs e)
        {
            refreshImage();
        }

        private void refreshImage()
        {
            previewPicture.Image = null;
            if (currentBitmap != null)
            {
                currentBitmap.Dispose();
            }
            currentBitmap = renderer.renderImage(400, 400);
            previewPicture.Image = currentBitmap;
        }

        private void saveGrowthDefect()
        {
            String presetDirectory = "Left";
            if (rightSideSource.Checked)
            {
                presetDirectory = "Right";
            }
            presetDirectory = Resource.ResourceRoot + "/Presets/" + presetDirectory + "Growth";
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
                    if (currentBitmap == null)
                    {
                        refreshImage();
                    }
                    currentBitmap.Save(presetDirectory + "/" + presetBones.ImageName);
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
            presetDirectory = Resource.ResourceRoot + "/Presets/" + presetDirectory + "Degeneration";
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
                    if (currentBitmap == null)
                    {
                        refreshImage();
                    }
                    currentBitmap.Save(presetDirectory + "/" + presetBones.ImageName);
                    MessageBox.Show(this, String.Format("Saved preset to {0}.", presetFile), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (IOException e)
                {
                    MessageBox.Show(this, String.Format("Error saving preset file.\nReason: {0}", e.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
