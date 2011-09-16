using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class NotesGUI : TimelineWizardPanel
    {
        Edit stateNameTextBox;
        Edit datePicker;
        Edit distortionWizard;
        Edit notes;

        private ThumbnailPickerGUI thumbnailPicker;
        private TimelineWizard wizard;

        public NotesGUI(TimelineWizard wizard)
            : base("Medical.TimelineGUI.Panels.Notes.NotesGUI.layout", wizard)
        {
            //this.LayerState = "MandibleSizeLayers";
            //this.NavigationState = "WizardMidlineAnterior";
            //this.TextLine1 = "Notes";

            this.wizard = wizard;

            stateNameTextBox = widget.findWidget("Notes/DistortionName") as Edit;
            datePicker = widget.findWidget("Notes/DateCreated") as Edit;
            distortionWizard = widget.findWidget("Notes/DistortionWizard") as Edit;
            notes = widget.findWidget("Notes/NotesText") as Edit;

            thumbnailPicker = new ThumbnailPickerGUI(wizard.ImageRenderer, widget.findWidget("Notes/Thumbnails") as ScrollView);
        }

        public override void Dispose()
        {
            thumbnailPicker.Dispose();
            base.Dispose();
        }

        public override void opening(MedicalController medicalController, SimulationScene simScene)
        {
            distortionWizard.OnlyText = wizard.DataSource;
            stateNameTextBox.OnlyText = wizard.StateName;
            notes.OnlyText = wizard.Notes;
            datePicker.Caption = wizard.ProcedureDate.ToString();

            NotesGUIData notesData = (NotesGUIData)PanelData;
            foreach (NotesThumbnail thumb in notesData.Thumbnails)
            {
                thumbnailPicker.addThumbnail(thumb);
            }
            thumbnailPicker.updateThumbnails();
        }

        protected override void commitOutstandingData()
        {
            wizard.StateName = stateNameTextBox.OnlyText;
            wizard.Notes = notes.OnlyText;
            wizard.Thumbnail = thumbnailPicker.SelectedThumbnail;
        }
    }
}
