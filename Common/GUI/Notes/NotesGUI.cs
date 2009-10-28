using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI
{
    public partial class NotesGUI : GUIElement
    {
        MedicalStateController stateController;

        public NotesGUI(MedicalStateController stateController)
        {
            InitializeComponent();
            this.stateController = stateController;
            stateController.StateChanged += stateController_StateChanged;
        }

        void stateController_StateChanged(MedicalState state)
        {
            notes.Rtf = state.Notes.Notes;
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
