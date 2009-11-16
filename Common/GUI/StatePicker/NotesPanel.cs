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
    public partial class NotesPanel : StatePickerPanel
    {
        public NotesPanel(String procedureType)
        {
            InitializeComponent();
            this.procedureType.Text = procedureType;
            this.Text = "Notes";
        }

        public override void setToDefault()
        {
            notes.Text = "";
            datePicker.Value = DateTime.Now;
        }

        public override void applyToState(MedicalState state)
        {
            state.Notes.DataSource = procedureType.Text;
            state.Notes.Notes = notes.Rtf;
            state.Notes.ProcedureDate = datePicker.Value;
            state.Name = stateNameTextBox.Text;
        }

        protected override void statePickerSet(StatePickerWizard controller)
        {
            parentPicker.ImageList.Images.Add(NavigationImageKey, Resources.NotesIcon);
        }

        internal override String NavigationImageKey
        {
            get
            {
                return "__NotesPanelKey";
            }
        }
    }
}
