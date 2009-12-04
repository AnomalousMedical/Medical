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
        private Dictionary<String, KryptonBreadCrumbItem> breadCrumbItems = new Dictionary<string,KryptonBreadCrumbItem>();

        public FileBrowserPickerPanel()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            fileListBox.ListBox.MouseDoubleClick += new MouseEventHandler(fileListBox_MouseDoubleClick);
            breadCrumbs.SelectedItemChanged += new EventHandler(breadCrumbs_SelectedItemChanged);
        }

        public void initialize(String rootDirectory)
        {
            this.rootDirectory = rootDirectory;
            breadCrumbItems.Clear();
            breadCrumbs.RootItem.Items.Clear();
            breadCrumbs.RootItem.Tag = rootDirectory;
            breadCrumbItems.Add(rootDirectory.Replace('\\', '/'), breadCrumbs.RootItem);
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
            bool buildBreadCrumbs = false;
            KryptonBreadCrumbItem directoryCrumbs = breadCrumbItems[targetDirectory.Replace('\\', '/')];
            buildBreadCrumbs = directoryCrumbs.Items.Count == 0;
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
                        ArchiveFileInfo fileInfo = archive.getFileInfo(directory);
                        KryptonListItem dirItem = new KryptonListItem(fileInfo.Name);
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
                        if (buildBreadCrumbs)
                        {
                            KryptonBreadCrumbItem crumb = new KryptonBreadCrumbItem(dirItem.ShortText);
                            directoryCrumbs.Items.Add(crumb);
                            crumb.Tag = directory;
                            breadCrumbItems.Add(directory.Replace('\\', '/'), crumb);
                        }
                    }
                }
            }
            currentDirectory = targetDirectory;
        }

        void fileListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (fileListBox.SelectedItem != null)
            {
                String newPath = String.Format("{0}/{1}", currentDirectory, ((KryptonListItem)fileListBox.SelectedItem).ShortText).Replace('\\', '/');
                breadCrumbs.SelectedItem = breadCrumbItems[newPath];
            }
        }

        void breadCrumbs_SelectedItemChanged(object sender, EventArgs e)
        {
            showDirectory(breadCrumbs.SelectedItem.Tag.ToString());
        }
    }
}
