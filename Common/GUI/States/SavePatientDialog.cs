using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ComponentFactory.Krypton.Toolkit;

namespace Medical.GUI
{
    public partial class SavePatientDialog : KryptonForm
    {
        private PatientDataFile patientData;
        private bool saveFile;
        private bool allowFileNameUpdate = true;
        private bool autoUpdate = false;

        public SavePatientDialog()
        {
            InitializeComponent();
            this.AllowFormChrome = !WindowsInfo.CompositionEnabled;
            lastText.TextChanged += new EventHandler(lastText_TextChanged);
            firstText.TextChanged += new EventHandler(firstText_TextChanged);
            fileNameTextBox.TextChanged += new EventHandler(fileNameTextBox_TextChanged);
            locationTextBox.TextChanged += new EventHandler(locationTextBox_TextChanged);
            warningLabel.Visible = false;
        }

        public bool save(Control parent)
        {
            if (patientData == null)
            {
                this.ShowDialog(parent);
                return saveFile;
            }
            else
            {
                return true;
            }
        }

        public bool saveAs(Control parent)
        {
            this.ShowDialog(parent);
            return saveFile;
        }

        void locationTextBox_TextChanged(object sender, EventArgs e)
        {
            checkForFile();
        }

        void fileNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!autoUpdate)
            {
                allowFileNameUpdate = false;
            }
            checkForFile();
        }

        void firstText_TextChanged(object sender, EventArgs e)
        {
            updateFileName();
        }

        void lastText_TextChanged(object sender, EventArgs e)
        {
            updateFileName();
        }

        void updateFileName()
        {
            if (allowFileNameUpdate)
            {
                autoUpdate = true;
                fileNameTextBox.Text = String.Format("{0}, {1}", lastText.Text, firstText.Text);
                autoUpdate = false;
            }
        }

        void checkForFile()
        {
            warningLabel.Visible = File.Exists(SavePath);
        }

        protected override void OnShown(EventArgs e)
        {
            if (patientData == null)
            {
                lastText.Text = "";
                firstText.Text = "";
                fileNameTextBox.Text = "";
                locationTextBox.Text = MedicalConfig.SaveDirectory;
            }
            else
            {
                lastText.Text = patientData.LastName;
                firstText.Text = patientData.FirstName;
                fileNameTextBox.Text = patientData.BackingFileName;
                locationTextBox.Text = patientData.BackingFileDirectory;
            }
            allowFileNameUpdate = true;
            firstText.Focus();
            base.OnShown(e);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            String filename = SavePath;
            saveFile = true;
            if (File.Exists(filename))
            {
                if (MessageBox.Show(this, String.Format("The file {0} already exists. Would you like to overwrite it?", fileNameTextBox.Text), "Overwrite?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    saveFile = false;
                }
            }
            if (saveFile)
            {
                patientData = new PatientDataFile(filename);
                patientData.FirstName = firstText.Text;
                patientData.LastName = lastText.Text;
                this.Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            saveFile = false;
            this.Close();
        }

        public PatientDataFile PatientData
        {
            get
            {
                return patientData;
            }
            set
            {
                patientData = value;
            }
        }

        public String SavePath
        {
            get
            {
                return String.Format("{0}/{1}.pdt", locationTextBox.Text, fileNameTextBox.Text);
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                locationTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }
}
