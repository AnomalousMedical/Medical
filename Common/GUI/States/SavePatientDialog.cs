using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Medical.GUI
{
    public partial class SavePatientDialog : Form
    {
        private String filename;
        private bool saveFile;

        public SavePatientDialog()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            lastText.Text = "";
            firstText.Text = "";
            base.OnShown(e);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            filename = MedicalConfig.SaveDirectory + "/" + String.Format("{0}, {1}", lastText.Text, firstText.Text) + ".pat";
            saveFile = true;
            if (File.Exists(filename))
            {
                if (MessageBox.Show(this, String.Format("The patient {0} {1} already exists. Would you like to overwrite it?", firstText.Text, lastText.Text), "Overwrite?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    saveFile = false;
                }
            }
            if (saveFile)
            {
                this.Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            saveFile = false;
            this.Close();
        }

        public String Filename
        {
            get
            {
                return filename;
            }
        }

        public bool SaveFile
        {
            get
            {
                return saveFile;
            }
        }
    }
}
