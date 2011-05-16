using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class NotesDialog : Dialog
    {
        private MedicalStateController stateController;

        private Edit stateNameTextBox;
        private Edit datePicker;
        private Edit distortionWizard;
        private Edit notes;

        public NotesDialog(MedicalStateController stateController)
            : base("Medical.GUI.Notes.NotesDialog.layout")
        {
            window.Visible = false;

            stateNameTextBox = window.findWidget("Notes/DistortionName") as Edit;
            datePicker = window.findWidget("Notes/DateCreated") as Edit;
            distortionWizard = window.findWidget("Notes/DistortionWizard") as Edit;
            notes = window.findWidget("Notes/NotesText") as Edit;

            this.stateController = stateController;
            stateController.StateChanged += new MedicalStateStatusUpdate(stateController_StateChanged);
        }

        void stateController_StateChanged(MedicalState state)
        {
            stateNameTextBox.Caption = state.Name;
            datePicker.Caption = state.Notes.ProcedureDate.ToString();
            distortionWizard.Caption = state.Notes.DataSource;
            notes.OnlyText = state.Notes.Notes;
        }
    }
}
