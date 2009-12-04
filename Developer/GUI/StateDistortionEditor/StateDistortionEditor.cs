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
                using (XmlTextWriter textWriter = new XmlTextWriter(distortionFile, Encoding.Default))
                {
                    textWriter.Formatting = Formatting.Indented;
                    xmlSaver.saveObject(state, textWriter);
                }
                String imageFile = distortionFile.Replace(".dst", ".png");
                picturePreviewPanel.saveBitmap(imageFile);
            }
        }
    }
}
