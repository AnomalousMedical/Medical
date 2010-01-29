using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using Engine.Resources;
using System.IO;
using Logging;
using System.Xml;

namespace Medical.GUI
{
    public partial class PresetStatePanel : StatePickerPanel
    {
        private Dictionary<String, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();
        private ListViewItem defaultItem = null;
        private ListViewItem openingItem = null; //the item that was selected when this ui was opened.
        private bool allowUpdates = true;
        private LinkedList<Image> images = new LinkedList<Image>();
        private String lastRootDirectory;
        private String subDirectory;

        public PresetStatePanel(String subDirectory, StatePickerPanelController panelController)
            : base(panelController)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.subDirectory = subDirectory;
            presetListView.SelectedIndexChanged += new EventHandler(presetListView_SelectedIndexChanged);
        }

        void presetListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                showChanges(false);
            }
        }

        public override void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            String rootDirectory = medicalController.CurrentSceneDirectory + '/' + simScene.PresetDirectory;
            if (rootDirectory != lastRootDirectory)
            {
                lastRootDirectory = rootDirectory;

                PresetStateSet presets = new PresetStateSet(this.Text, rootDirectory + '/' + subDirectory);
                loadPresetSet(presets);
                this.clear();
                this.initialize(presets);
            }
        }

        public void initialize(PresetStateSet presetStateSet)
        {
            if (presetListView.LargeImageList == null)
            {
                presetListView.LargeImageList = panelController.ImageList;
            }
            using (Archive archive = FileSystem.OpenArchive(presetStateSet.SourceDirectory))
            {
                foreach (PresetState state in presetStateSet.Presets)
                {
                    ListViewGroup group;
                    groups.TryGetValue(state.Category, out group);
                    if (group == null)
                    {
                        group = new ListViewGroup(state.Category);
                        groups.Add(state.Category, group);
                        presetListView.Groups.Add(group);
                    }
                    String fullImageName = presetStateSet.SourceDirectory + "/" + state.ImageName;
                    if (!presetListView.LargeImageList.Images.ContainsKey(fullImageName))
                    {
                        try
                        {
                            using (Stream imageStream = archive.openStream(fullImageName, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                            {
                                Image image = Image.FromStream(imageStream);
                                if (image != null)
                                {
                                    presetListView.LargeImageList.Images.Add(fullImageName, image);
                                    images.AddLast(image);
                                    image.Tag = fullImageName;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error("Could not open image preview file {0}.", fullImageName);
                        }
                    }
                    ListViewItem item = new ListViewItem(state.Name, fullImageName);
                    item.Tag = state;
                    item.Group = group;
                    presetListView.Items.Add(item);
                }
                if (presetListView.Items.Count > 0)
                {
                    defaultItem = presetListView.Items[0];
                }
            }
        }

        public void clear()
        {
            clearImages();
            groups.Clear();
            presetListView.Groups.Clear();
            presetListView.Items.Clear();
        }

        public override void applyToState(MedicalState state)
        {
            if (presetListView.SelectedItems.Count > 0)
            {
                PresetState preset = presetListView.SelectedItems[0].Tag as PresetState;
                preset.applyToState(state);
            }
            else
            {
                PresetState preset = defaultItem.Tag as PresetState;
                preset.applyToState(state);
            }
        }

        public override void setToDefault()
        {
            allowUpdates = false;
            presetListView.SelectedItems.Clear();
            if (defaultItem != null)
            {
                defaultItem.Selected = true;
            }
            allowUpdates = true;
        }

        public override void recordOpeningState()
        {
            if (presetListView.SelectedItems.Count > 0)
            {
                openingItem = presetListView.SelectedItems[0];
            }
        }

        public override void resetToOpeningState()
        {
            allowUpdates = false;
            presetListView.SelectedItems.Clear();
            if (openingItem != null)
            {
                openingItem.Selected = true;
            }
            allowUpdates = true;
        }

        private void clearImages()
        {
            foreach (Image image in images)
            {
                panelController.ImageList.Images.RemoveByKey(image.Tag.ToString());
                image.Dispose();
            }
            images.Clear();
        }

        private void loadPresetSet(PresetStateSet presetStateSet)
        {
            using (Archive archive = FileSystem.OpenArchive(presetStateSet.SourceDirectory))
            {
                String[] files = archive.listFiles(presetStateSet.SourceDirectory, "*.pre", false);
                foreach (String file in files)
                {
                    XmlTextReader reader = new XmlTextReader(archive.openStream(file, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read));
                    PresetState preset = panelController.XmlSaver.restoreObject(reader) as PresetState;
                    if (preset != null)
                    {
                        presetStateSet.addPresetState(preset);
                    }
                    else
                    {
                        Log.Error("Could not load preset from file {0}. Object was not a BoneManipulatorPresetState.", file);
                    }
                    reader.Close();
                }
            }
        }
    }
}
