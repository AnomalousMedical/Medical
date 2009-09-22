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

namespace Medical.GUI
{
    public partial class PresetStatePanel : StatePickerPanel
    {
        private Dictionary<String, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();
        private ListViewItem defaultItem = null;

        public PresetStatePanel()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            presetListView.LargeImageList = new ImageList();
            presetListView.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
            presetListView.LargeImageList.ImageSize = new Size(100, 100);
            presetListView.SelectedIndexChanged += new EventHandler(presetListView_SelectedIndexChanged);
        }

        void presetListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            showChanges(false);
        }

        public void initialize(PresetStateSet presetStateSet)
        {
            using (Archive archive = FileSystem.OpenArchive(presetStateSet.SourceDirectory))
            {
                this.Text = presetStateSet.Name;
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
                    if (!presetListView.LargeImageList.Images.ContainsKey(state.ImageName))
                    {
                        try
                        {
                            using (Stream imageStream = archive.openStream(presetStateSet.SourceDirectory + "/" + state.ImageName, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                            {
                                Image image = Image.FromStream(imageStream);
                                if (image != null)
                                {
                                    presetListView.LargeImageList.Images.Add(state.ImageName, image);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error("Could not open image preview file {0}.", presetStateSet.SourceDirectory + "/" + state.ImageName);
                        }
                    }
                    ListViewItem item = new ListViewItem(state.Name, state.ImageName);
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
            
        }
    }
}
