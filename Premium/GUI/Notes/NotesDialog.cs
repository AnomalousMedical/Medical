using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class NotesDialog : MDIDialog
    {
        private MedicalStateController stateController;

        private EditBox stateNameTextBox;
        private EditBox datePicker;
        private EditBox distortionWizard;
        private EditBox notes;

        public NotesDialog(MedicalStateController stateController)
            : base("Medical.GUI.Notes.NotesDialog.layout")
        {
            window.Visible = false;

            stateNameTextBox = window.findWidget("Notes/DistortionName") as EditBox;
            datePicker = window.findWidget("Notes/DateCreated") as EditBox;
            distortionWizard = window.findWidget("Notes/DistortionWizard") as EditBox;
            notes = window.findWidget("Notes/NotesText") as EditBox;

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
