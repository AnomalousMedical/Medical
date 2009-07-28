using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
        }

        public void initialize(PresetStateSet presetStateSet)
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
                ListViewItem item = new ListViewItem(state.Name, state.ImageName);
                item.Tag = state;
                item.Group = group;
                presetListView.Items.Add(item);
            }
            defaultItem = presetListView.Items[0];
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
