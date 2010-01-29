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
        private String fileFilter;

        private List<Bitmap> currentDirectoryBitmaps = new List<Bitmap>();
        private Dictionary<String, KryptonBreadCrumbItem> breadCrumbItems = new Dictionary<string,KryptonBreadCrumbItem>();
        private List<String> files = new List<string>();

        public FileBrowserPickerPanel(String fileFilter, StatePickerPanelController panelController)
            :base(panelController)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.fileFilter = fileFilter;
            fileListBox.ListBox.MouseDoubleClick += new MouseEventHandler(fileListBox_MouseDoubleClick);
            fileListBox.ListBox.MouseClick += new MouseEventHandler(ListBox_MouseClick);
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

        protected virtual void onFileChosen(String filename)
        {

        }

        private void showDirectory(String targetDirectory)
        {
            bool buildBreadCrumbs = false;
            KryptonBreadCrumbItem directoryCrumbs = breadCrumbItems[targetDirectory.Replace('\\', '/')];
            buildBreadCrumbs = directoryCrumbs.Items.Count == 0;
            foreach (Bitmap bitmap in currentDirectoryBitmaps)
            {
                bitmap.Dispose();
            }
            currentDirectoryBitmaps.Clear();
            fileListBox.Items.Clear();
            files.Clear();
            using (Archive archive = FileSystem.OpenArchive(targetDirectory))
            {
                if (archive.isDirectory(targetDirectory))
                {
                    foreach (String directory in archive.listDirectories(targetDirectory, false))
                    {
                        String fullPath = archive.getFullPath(directory).Replace('\\', '/');
                        ArchiveFileInfo fileInfo = archive.getFileInfo(fullPath);
                        KryptonListItem dirItem = new KryptonListItem(fileInfo.Name);
                        dirItem.Tag = fullPath;
                        String folderThumbnailFile = fullPath + "/folder.png";
                        if (archive.exists(folderThumbnailFile))
                        {
                            using (Stream imageStream = archive.openStream(folderThumbnailFile, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                            {
                                Bitmap bmp = new Bitmap(imageStream);
                                currentDirectoryBitmaps.Add(bmp);
                                dirItem.Image = bmp;
                            }
                        }
                        fileListBox.Items.Add(dirItem);
                        if (buildBreadCrumbs)
                        {
                            KryptonBreadCrumbItem crumb = new KryptonBreadCrumbItem(dirItem.ShortText);
                            directoryCrumbs.Items.Add(crumb);
                            crumb.Tag = fullPath;
                            System.Console.WriteLine(fullPath);
                            breadCrumbItems.Add(fullPath, crumb);
                        }
                    }
                    foreach (String file in archive.listFiles(targetDirectory, fileFilter, false))
                    {
                        String fixedFile = archive.getFullPath(file).Replace('\\', '/');
                        ArchiveFileInfo fileInfo = archive.getFileInfo(fixedFile);
                        String fileNameOnly = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.'));
                        KryptonListItem fileItem = new KryptonListItem(fileNameOnly);
                        fileItem.Tag = fixedFile;
                        String fileThumbnail = String.Format("{0}/{1}.png", fileInfo.DirectoryName, fileNameOnly);
                        if (archive.exists(fileThumbnail))
                        {
                            using (Stream imageStream = archive.openStream(fileThumbnail, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                            {
                                Bitmap bmp = new Bitmap(imageStream);
                                currentDirectoryBitmaps.Add(bmp);
                                fileItem.Image = bmp;
                            }
                        }
                        files.Add(fixedFile);
                        fileListBox.Items.Add(fileItem);
                    }
                }
            }
            currentDirectory = targetDirectory;
        }

        void fileListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (fileListBox.SelectedItem != null)
            {
                String newPath = ((KryptonListItem)fileListBox.SelectedItem).Tag.ToString().Replace('\\', '/');
                if (!files.Contains(newPath))
                {
                    breadCrumbs.SelectedItem = breadCrumbItems[newPath];
                }
            }
        }

        void ListBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (fileListBox.SelectedItem != null)
            {
                String newPath = ((KryptonListItem)fileListBox.SelectedItem).Tag.ToString().Replace('\\', '/');
                if (files.Contains(newPath))
                {
                    onFileChosen(newPath);
                }
            }
        }

        void breadCrumbs_SelectedItemChanged(object sender, EventArgs e)
        {
            if (breadCrumbs.SelectedItem != null)
            {
                showDirectory(breadCrumbs.SelectedItem.Tag.ToString());
            }
        }
    }
}
