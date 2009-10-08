using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.Resources;
using System.IO;

namespace Medical.GUI
{
    public partial class ScenePicker : Form
    {
        private LinkedList<Image> images = new LinkedList<Image>();

        public ScenePicker()
        {
            InitializeComponent();
            sceneSelectionView.LargeImageList = new ImageList();
            sceneSelectionView.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
            sceneSelectionView.LargeImageList.ImageSize = new Size(214, 256);
            sceneSelectionView.ItemActivate += new EventHandler(sceneSelectionView_ItemActivate);
        }

        public void initialize()
        {
            using (Archive archive = FileSystem.OpenArchive(MedicalConfig.SceneDirectory))
            {
                String sceneDirectory = MedicalConfig.SceneDirectory;
                String[] files = archive.listFiles(sceneDirectory, "*.sim.xml", false);
                foreach (String file in files)
                {
                    String fileName = FileSystem.GetFileName(file);
                    String baseName = fileName.Substring(0, fileName.IndexOf('.'));
                    ListViewItem listViewItem = new ListViewItem(baseName);
                    String pictureFileName = sceneDirectory + "/" + baseName + ".png";
                    if (archive.exists(pictureFileName))
                    {
                        using (Stream imageStream = archive.openStream(pictureFileName, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                        {
                            Image image = Image.FromStream(imageStream);
                            if (image != null)
                            {
                                sceneSelectionView.LargeImageList.Images.Add(pictureFileName, image);
                                listViewItem.ImageKey = pictureFileName;
                                images.AddLast(image);
                            }
                        }
                    }
                    listViewItem.Tag = fileName;
                    sceneSelectionView.Items.Add(listViewItem);
                }
            }
        }

        public String SelectedFileName
        {
            get
            {
                if (sceneSelectionView.SelectedItems.Count > 0)
                {
                    return MedicalConfig.SceneDirectory + "/" + sceneSelectionView.SelectedItems[0].Tag.ToString();
                }
                else
                {
                    return MedicalConfig.DefaultScene;
                }
            }
        }

        private void sceneSelectionView_ItemActivate(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void clearImages()
        {
            foreach (Image image in images)
            {
                image.Dispose();
            }
        }
    }
}
