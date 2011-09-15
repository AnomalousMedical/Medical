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
            //thumbnailPicker.addThumbnail("ThumbnailMidlineAnterior", "MandibleSizeLayers");
            //thumbnailPicker.addThumbnail("ThumbnailRightLateral", "MandibleSizeLayers");
            //thumbnailPicker.addThumbnail("ThumbnailLeftLateral", "MandibleSizeLayers");
            //thumbnailPicker.addThumbnail("ThumbnailRightTMJ", "DiscLayers");
            //thumbnailPicker.addThumbnail("ThumbnailLeftTMJ", "DiscLayers");
            //thumbnailPicker.addThumbnail("ThumbnailLeftTMJSuperior", "ThumbnailTMJSuperiorLayers");
            //thumbnailPicker.addThumbnail("ThumbnailRightTMJSuperior", "ThumbnailTMJSuperiorLayers");
            //thumbnailPicker.addThumbnail("ThumbnailTeethMidlineAnterior", "TeethLayers");
            //thumbnailPicker.addThumbnail("ThumbnailTeethRightLateral", "TeethLayers");
            //thumbnailPicker.addThumbnail("ThumbnailTeethLeftLateral", "TeethLayers");
            thumbnailPicker.addThumbnail();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void opening(MedicalController medicalController, SimulationScene simScene)
        {
            distortionWizard.OnlyText = wizard.DataSource;
            stateNameTextBox.OnlyText = wizard.StateName;
            notes.OnlyText = wizard.Notes;
            datePicker.Caption = wizard.ProcedureDate.ToString();
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
