using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.Resources;
using ComponentFactory.Krypton.Toolkit;
using System.IO;

namespace Medical.GUI
{
    public partial class FileBrowserPickerPanel : StatePickerPanel
    {
        private String rootDirectory;
        private String currentDirectory;

        private List<Bitmap> currentDirectoryFiles = new List<Bitmap>();

        public FileBrowserPickerPanel()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            fileListBox.ListBox.MouseDoubleClick += new MouseEventHandler(fileListBox_MouseDoubleClick);
        }

        public void initialize(String rootDirectory)
        {
            this.rootDirectory = rootDirectory;
        }

        public override void setToDefault()
        {
            if (rootDirectory != null)
            {
                showDirectory(rootDirectory);
            }
        }

        private void showDirectory(String targetDirectory)
        {
            foreach (Bitmap bitmap in currentDirectoryFiles)
            {
                bitmap.Dispose();
            }
            currentDirectoryFiles.Clear();
            fileListBox.Items.Clear();
            using (Archive archive = FileSystem.OpenArchive(targetDirectory))
            {
                if (archive.isDirectory(targetDirectory))
                {
                    foreach (String directory in archive.listDirectories(targetDirectory, false))
                    {
                        KryptonListItem dirItem = new KryptonListItem(Path.GetFileNameWithoutExtension(directory));
                        String folderThumbnailFile = directory + "/folder.png";
                        if (archive.exists(folderThumbnailFile))
                        {
                            using (Stream imageStream = archive.openStream(folderThumbnailFile, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                            {
                                Bitmap bmp = new Bitmap(imageStream);
                                currentDirectoryFiles.Add(bmp);
                                dirItem.Image = bmp;
                            }
                        }
                        fileListBox.Items.Add(dirItem);
                    }
                }
            }
            currentDirectory = targetDirectory;
        }

        void fileListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (fileListBox.SelectedItem != null)
            {
                showDirectory(String.Format("{0}/{1}", currentDirectory, ((KryptonListItem)fileListBox.SelectedItem).ShortText));
            }
        }
    }
}
