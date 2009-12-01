using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medical.Controller;
using Logging;

namespace Medical.GUI
{
    public partial class StateNotesPanel : UserControl
    {
        private MedicalStateController stateController;
        private ShortcutController shortcutController;

        public StateNotesPanel(MedicalStateController stateController, ShortcutController shortcutController)
        {
            InitializeComponent();
            this.stateController = stateController;
            this.shortcutController = shortcutController;
            stateController.StateChanged += stateController_StateChanged;
            stateController.StatesCleared += stateController_StatesCleared;
            notes.LostFocus += notes_LostFocus;
            notes.GotFocus += notes_GotFocus;
        }

        private void notes_GotFocus(object sender, EventArgs e)
        {
            shortcutController.Enabled = false;
        }

        private void notes_LostFocus(object sender, EventArgs e)
        {
            shortcutController.Enabled = true;
        }

        private void stateController_StateChanged(MedicalState state)
        {
            if (notes.Enabled == false)
            {
                notes.Enabled = true;
                datePicker.Enabled = true;
            }
            try
            {
                notes.Rtf = state.Notes.Notes;
            }
            catch (ArgumentException e)
            {
                Log.Warning("Error setting notes text.\n{0}", e.Message);
                notes.Text = state.Notes.Notes;
            }
            procedureType.Text = state.Notes.DataSource;
            DateTime date = state.Notes.ProcedureDate;
            if (date > datePicker.MinDate && date < datePicker.MaxDate)
            {
                datePicker.Value = date;
            }
        }

        private void stateController_StatesCleared(MedicalStateController controller)
        {
            notes.Rtf = "";
            datePicker.Value = DateTime.Today;
            procedureType.Text = "";
            notes.Enabled = false;
            datePicker.Enabled = false;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            MedicalState currentState = stateController.CurrentState;
            if (currentState != null)
            {
                MedicalStateNotes notes = currentState.Notes;
                notes.ProcedureDate = datePicker.Value;
                notes.DataSource = procedureType.Text;
                notes.Notes = this.notes.Rtf;
            }
        }
    }
}
