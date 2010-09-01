using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class NotesPanel : StateWizardPanel
    {
        Edit stateNameTextBox;
        Edit datePicker;
        Edit distortionWizard;
        Edit notes;

        private ThumbnailPicker thumbnailPicker;

        public NotesPanel(StateWizardPanelController controller)
            : base("Medical.Controller.StatePicker.Panels.Notes.NotesPanel.layout", controller)
        {
            this.LayerState = "MandibleSizeLayers";
            this.NavigationState = "WizardMidlineAnterior";
            this.TextLine1 = "Notes";

            stateNameTextBox = mainWidget.findWidget("Notes/DistortionName") as Edit;
            datePicker = mainWidget.findWidget("Notes/DateCreated") as Edit;
            distortionWizard = mainWidget.findWidget("Notes/DistortionWizard") as Edit;
            notes = mainWidget.findWidget("Notes/NotesText") as Edit;

            thumbnailPicker = new ThumbnailPicker(controller.ImageRenderer, mainWidget.findWidget("Notes/Thumbnails") as ScrollView);
            thumbnailPicker.addThumbnail("ThumbnailMidlineAnterior", "MandibleSizeLayers");
            thumbnailPicker.addThumbnail("ThumbnailRightLateral", "MandibleSizeLayers");
            thumbnailPicker.addThumbnail("ThumbnailLeftLateral", "MandibleSizeLayers");
            thumbnailPicker.addThumbnail("ThumbnailRightTMJ", "DiscLayers");
            thumbnailPicker.addThumbnail("ThumbnailLeftTMJ", "DiscLayers");
            thumbnailPicker.addThumbnail("ThumbnailLeftTMJSuperior", "ThumbnailTMJSuperiorLayers");
            thumbnailPicker.addThumbnail("ThumbnailRightTMJSuperior", "ThumbnailTMJSuperiorLayers");
            thumbnailPicker.addThumbnail("ThumbnailTeethMidlineAnterior", "TeethLayers");
            thumbnailPicker.addThumbnail("ThumbnailTeethRightLateral", "TeethLayers");
            thumbnailPicker.addThumbnail("ThumbnailTeethLeftLateral", "TeethLayers");
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void setToDefault()
        {
            stateNameTextBox.Caption = "Custom Distortion";
            notes.Caption = "";
            datePicker.Caption = DateTime.Now.ToString();
        }

        public override void recordOpeningState()
        {
            setToDefault();
        }

        public override void applyToState(MedicalState state)
        {
            state.Notes.DataSource = distortionWizard.Caption;
            state.Notes.Notes = notes.Caption;
            state.Notes.ProcedureDate = DateTime.Now;// datePicker.Value;
            state.Name = stateNameTextBox.Caption;
            state.Thumbnail = thumbnailPicker.SelectedThumbnail;
        }

        protected override void onPanelOpening()
        {
            distortionWizard.Caption = controller.CurrentWizardName;
            thumbnailPicker.updateThumbnails();
        }
    }
}
