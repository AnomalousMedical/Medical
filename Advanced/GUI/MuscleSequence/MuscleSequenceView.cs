using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medical.Properties;
using Engine.Resources;
using Engine.Saving.XMLSaver;
using Medical.Muscles;

namespace Medical.GUI
{
    public delegate void MuscleSequenceEvent(String sequenceText, String sequenceFile);

    public partial class MuscleSequenceView : UserControl
    {
        private Dictionary<String, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();
        public event MuscleSequenceEvent SequenceActivated;
        public event MuscleSequenceEvent SequenceSelected;

        public MuscleSequenceView()
        {
            InitializeComponent();
            muscleStateList.ItemActivate += new EventHandler(muscleStateList_ItemActivate);
            muscleStateList.SelectedIndexChanged += new EventHandler(muscleStateList_SelectedIndexChanged);
            muscleStateList.Columns.Add("Name", -2, HorizontalAlignment.Left);
        }

        public void initializeSequences(SimulationScene simScene, String sceneDirectory)
        {
            groups.Clear();
            muscleStateList.Groups.Clear();
            String sequenceDir = sceneDirectory + "/" + simScene.SequenceDirectory;
            using (Archive archive = FileSystem.OpenArchive(sequenceDir))
            {
                foreach (String directory in archive.listDirectories(sequenceDir, false))
                {
                    ListViewGroup group = new ListViewGroup(archive.getFileInfo(directory).Name);
                    muscleStateList.Groups.Add(group);
                    foreach (String file in archive.listFiles(directory, false))
                    {
                        String fileName = archive.getFileInfo(file).Name;
                        ListViewItem listViewItem = new ListViewItem(fileName.Substring(0, fileName.Length - 4), group);
                        listViewItem.Tag = archive.getFullPath(file);
                        muscleStateList.Items.Add(listViewItem);
                    }
                }
            }
        }

        public void clearSequences()
        {
            muscleStateList.Items.Clear();
        }

        void muscleStateList_ItemActivate(object sender, EventArgs e)
        {
            if (muscleStateList.SelectedItems.Count > 0 && SequenceActivated != null)
            {
                ListViewItem selected = muscleStateList.SelectedItems[0];
                SequenceActivated.Invoke(selected.Text, selected.Tag.ToString());
            }
        }

        void muscleStateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SequenceSelected != null)
            {
                if (muscleStateList.SelectedItems.Count > 0)
                {
                    ListViewItem selected = muscleStateList.SelectedItems[0];
                    SequenceSelected.Invoke(selected.Text, selected.Tag.ToString());
                }
                else
                {
                    SequenceSelected.Invoke(null, null);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (muscleStateList != null && muscleStateList.Columns.Count > 0)
            {
                muscleStateList.Columns[0].Width = -2;
            }
        }
    }
}
