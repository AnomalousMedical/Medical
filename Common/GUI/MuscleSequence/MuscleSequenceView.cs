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
        public MuscleSequenceView()
        {
            InitializeComponent();
            muscleStateList.ItemActivate += new EventHandler(muscleStateList_ItemActivate);
        }

        public void initializeSequences()
        {
            foreach (MuscleSequence sequence in MuscleController.getMuscleSequences())
            {
                ListViewItem listViewItem = new ListViewItem(sequence.SequenceName, sequence.IconName);
                listViewItem.Tag = sequence;
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
