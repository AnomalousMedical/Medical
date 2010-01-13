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

            stateListBox.SelectedIndexChanged += new EventHandler(stateListBox_SelectedIndexChanged);
        }

        void stateListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (stateListBox.SelectedIndices.Count > 0)
            {
                //stateController.blendTo(stateListBox.SelectedIndices[0], 1.0f);
                stateController.directBlend(stateListBox.SelectedIndices[0], 1.0f);
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
