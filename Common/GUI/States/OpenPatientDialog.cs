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
        private String currentFile = null;

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
                String[] files = Directory.GetFiles(directory, "*.sim.xml");
                fileList.Columns[0].Width = (int)(fileList.Width * 0.7f);
                fileList.Columns[1].Width = fileList.Width - fileList.Columns[0].Width;
                foreach (String file in files)
                {
                    String name = Path.GetFileNameWithoutExtension(file);
                    DateTime dateModified = File.GetLastWriteTime(file);
                    ListViewItem item = new ListViewItem(name);
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
            currentFile = fileList.SelectedItems[0].Tag.ToString();
            this.Close();
        }

        public bool FileChosen
        {
            get
            {
                return currentFile != null;
            }
        }

        public String CurrentFile
        {
            get
            {
                return currentFile;
            }
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            currentFile = fileList.SelectedItems[0].Tag.ToString();
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
    }
}
