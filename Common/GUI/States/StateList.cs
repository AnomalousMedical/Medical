using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using Medical.Properties;
using Engine;
using Medical.Controller;

namespace Medical.GUI
{
    public partial class StateList : UserControl
    {
        private MedicalStateController stateController;
        private Dictionary<MedicalState, KryptonListItem> entries = new Dictionary<MedicalState, KryptonListItem>();
        private bool ignoreIndexChanges = false;

        public StateList(MedicalStateController stateController)
        {
            InitializeComponent();
            
            this.stateController = stateController;
            stateController.StateAdded += new MedicalStateAdded(stateController_StateAdded);
            stateController.StateRemoved += new MedicalStateRemoved(stateController_StateRemoved);
            stateController.StatesCleared += new MedicalStateEvent(stateController_StatesCleared);
            stateController.StateChanged += new MedicalStateChanged(stateController_StateChanged);
            stateController.BlendingStarted += new MedicalStateEvent(stateController_BlendingStarted);
            stateController.BlendingStopped += new MedicalStateEvent(stateController_BlendingStopped);

            stateListBox.SelectedValueChanged += new EventHandler(stateListBox_SelectedValueChanged);
            stateListBox.ListBox.MouseUp += new MouseEventHandler(ListBox_MouseUp);
            stateListBox.ListBox.KeyUp += new KeyEventHandler(ListBox_KeyUp);

            deleteCommand.Execute += new EventHandler(deleteCommand_Execute);
        }

        void ListBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                stateListBox.SelectedIndex = stateListBox.ListBox.IndexFromPoint(e.Location);
                contextMenu.Show(stateListBox.PointToScreen(e.Location));
            }
        }

        void stateListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (stateListBox.SelectedIndices.Count > 0)
            {
                //stateController.blendTo(stateListBox.SelectedIndices[0], 1.0f);
                stateController.directBlend(stateListBox.SelectedIndices[0], 1.0f);
            }
        }

        void ListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteCommand_Execute(this, EventArgs.Empty);
            }
        }

        void deleteCommand_Execute(object sender, EventArgs e)
        {
            int selectedIndex = stateListBox.SelectedIndex;
            stateController.destroyState(selectedIndex);
            if (selectedIndex < stateListBox.Items.Count)
            {
                stateListBox.SelectedIndex = selectedIndex;
            }
            else if(stateListBox.Items.Count > 0)
            {
                if (selectedIndex > 0)
                {
                    stateListBox.SelectedIndex = selectedIndex - 1;
                }
            }
        }

        void stateController_StateAdded(MedicalStateController controller, MedicalState state, int index)
        {
            KryptonListItem entry = new KryptonListItem();
            entry.ShortText = state.Name;
            entry.Image = state.Thumbnail;
            entries.Add(state, entry);
            stateListBox.Items.Insert(index, entry);
        }

        void stateController_StateRemoved(MedicalStateController controller, MedicalState state, int index)
        {
            KryptonListItem entry = entries[state];
            stateListBox.Items.Remove(entry);
            entry.Dispose();
            entries.Remove(state);
        }

        void stateController_StatesCleared(MedicalStateController controller)
        {
            stateListBox.Items.Clear();
            entries.Clear();
        }

        void stateController_StateChanged(MedicalState state)
        {
            if (!ignoreIndexChanges)
            {
                stateListBox.SelectedItem = entries[state];
            }
        }

        void stateController_BlendingStopped(MedicalStateController controller)
        {
            ignoreIndexChanges = false;
            StatusController.TaskCompleted();
        }

        void stateController_BlendingStarted(MedicalStateController controller)
        {
            ignoreIndexChanges = true;
            StatusController.SetStatus("Blending...");
        }
    }
}
