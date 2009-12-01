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
    public partial class OpenPatientDialog : Form
    {
        private static char[] SEPS = { ',' };

        private PatientDataFile currentFile = null;

        public OpenPatientDialog()
        {
            InitializeComponent();
            fileList.ItemActivate += new EventHandler(fileList_ItemActivate);
            fileList.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(fileList_ItemSelectionChanged);
        }

        public void listFiles(String directory)
        {
            try
            {
                fileList.BeginUpdate();
                fileList.Items.Clear();
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                String[] files = Directory.GetFiles(directory, "*.pat");
                fileList.Columns[0].Width = (int)(fileList.Width * 0.35f);
                fileList.Columns[1].Width = (int)(fileList.Width * 0.35f);
                fileList.Columns[2].Width = (int)(fileList.Width * 0.3f);
                foreach (String file in files)
                {
                    String name = Path.GetFileNameWithoutExtension(file);
                    DateTime dateModified = File.GetLastWriteTime(file);
                    String[] split = name.Split(SEPS);
                    ListViewItem item = new ListViewItem(split[0].Trim());
                    if (split.Length > 1)
                    {
                        item.SubItems.Add(split[1].Trim());
                    }
                    else
                    {
                        item.SubItems.Add("");
                    }
                    item.SubItems.Add(dateModified.ToShortDateString() + " " + dateModified.ToShortTimeString());
                    item.Tag = file;
                    fileList.Items.Add(item);
                }
                fileList.EndUpdate();
            }
            catch
            {

            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            currentFile = null;
            openButton.Enabled = false;
        }

        void fileList_ItemActivate(object sender, EventArgs e)
        {
            openButton_Click(null, null);
        }

        public bool FileChosen
        {
            get
            {
                return currentFile != null;
            }
        }

        public PatientDataFile CurrentFile
        {
            get
            {
                return currentFile;
            }
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            currentFile = new PatientDataFile(fileList.SelectedItems[0].Tag.ToString());
            currentFile.loadHeader();
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void fileList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            openButton.Enabled = fileList.SelectedItems.Count > 0;
        }

        protected override void OnResize(EventArgs e)
        {
            fileList.Columns[0].Width = -1;
            fileList.Columns[1].Width = -1;
            fileList.Columns[2].Width = -1;
            base.OnResize(e);
        }
    }
}
