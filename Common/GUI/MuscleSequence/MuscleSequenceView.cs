using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medical.Properties;

namespace Medical.GUI
{
    public partial class MuscleSequenceView : UserControl
    {
        private Dictionary<String, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();

        public MuscleSequenceView()
        {
            InitializeComponent();
            muscleStateList.ItemActivate += new EventHandler(muscleStateList_ItemActivate);
        }

        public void initializeSequences()
        {
            groups.Clear();
            muscleStateList.Groups.Clear();
            foreach (MuscleSequence sequence in MuscleController.getMuscleSequences())
            {
                ListViewItem listViewItem = new ListViewItem(sequence.SequenceName, sequence.IconName);
                listViewItem.Tag = sequence;
                ListViewGroup group;
                groups.TryGetValue(sequence.GroupName, out group);
                if (group == null)
                {
                    group = new ListViewGroup(sequence.GroupName, sequence.GroupName);
                    groups.Add(sequence.GroupName, group);
                    muscleStateList.Groups.Add(group);
                }
                listViewItem.Group = group;
                muscleStateList.Items.Add(listViewItem);
            }
        }

        public void clearSequences()
        {
            muscleStateList.Items.Clear();
        }

        void muscleStateList_ItemActivate(object sender, EventArgs e)
        {
            if (muscleStateList.SelectedItems.Count > 0)
            {
                ((MuscleSequence)muscleStateList.SelectedItems[0].Tag).activate();
            }
        }

        public ImageList LargeImageList
        {
            get
            {
                return muscleStateList.LargeImageList;
            }
            set
            {
                muscleStateList.LargeImageList = value;
            }
        }
    }
}
