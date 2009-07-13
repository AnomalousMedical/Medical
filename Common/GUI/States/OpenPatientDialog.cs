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
        }

        public void listFiles(String directory)
        {
            fileList.BeginUpdate();
            fileList.Items.Clear();
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

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            currentFile = null;
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
    }
}
