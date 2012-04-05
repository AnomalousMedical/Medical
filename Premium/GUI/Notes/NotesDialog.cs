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

        private MedicalState currentState;

        public NotesDialog(MedicalStateController stateController)
            : base("Medical.GUI.Notes.NotesDialog.layout")
        {
            window.Visible = false;

            stateNameTextBox = window.findWidget("Notes/DistortionName") as EditBox;
            datePicker = window.findWidget("Notes/DateCreated") as EditBox;
            distortionWizard = window.findWidget("Notes/DistortionWizard") as EditBox;
            notes = window.findWidget("Notes/NotesText") as EditBox;

            Button save = window.findWidget("Save") as Button;
            save.MouseButtonClick += new MyGUIEvent(save_MouseButtonClick);

            this.stateController = stateController;
            stateController.StateChanged += new MedicalStateStatusUpdate(stateController_StateChanged);
            stateController.StatesCleared += new MedicalStateEvent(stateController_StatesCleared);
        }

        void stateController_StatesCleared(MedicalStateController controller)
        {
            currentState = null;
            stateNameTextBox.Caption = "";
            datePicker.Caption = "";
            distortionWizard.Caption = "";
            notes.OnlyText = "";
        }

        void stateController_StateChanged(MedicalState state)
        {
            this.currentState = state;
            stateNameTextBox.Caption = state.Name;
            datePicker.Caption = state.Notes.ProcedureDate.ToString();
            distortionWizard.Caption = state.Notes.DataSource;
            notes.OnlyText = state.Notes.Notes;
        }

        void save_MouseButtonClick(Widget source, EventArgs e)
        {
            if (currentState != null)
            {
                currentState.Name = stateNameTextBox.Caption;
                currentState.Notes.Notes = notes.OnlyText;

                stateController.alertStateUpdated(currentState);
            }
        }
    }
}
