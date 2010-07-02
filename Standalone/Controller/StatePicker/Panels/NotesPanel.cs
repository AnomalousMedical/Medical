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

        public NotesPanel(String notesPanelFile, StateWizardPanelController controller)
            : base(notesPanelFile, controller)
        {
            this.LayerState = "MandibleSizeLayers";
            this.NavigationState = "WizardMidlineAnterior";
            this.TextLine1 = "Notes";

            stateNameTextBox = mainWidget.findWidget("Notes/DistortionName") as Edit;
            datePicker = mainWidget.findWidget("Notes/DateCreated") as Edit;
            distortionWizard = mainWidget.findWidget("Notes/DistortionWizard") as Edit;
            notes = mainWidget.findWidget("Notes/NotesText") as Edit;

            //thumbnailPicker.ImageRenderer = imageRenderer;
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
        }

        public override void setToDefault()
        {
            stateNameTextBox.Caption = "Custom Distortion";
            notes.Caption = "";
            //datePicker.Value = DateTime.Now;
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
            //state.Thumbnail = thumbnailPicker.SelectedThumbnail;
        }

        public String DistortionWizardText
        {
            get
            {
                return distortionWizard.Caption;
            }
            set
            {
                distortionWizard.Caption = value;
            }
        }

        protected override void onPanelOpening()
        {
            //thumbnailPicker.updateThumbnails();
        }
    }
}
