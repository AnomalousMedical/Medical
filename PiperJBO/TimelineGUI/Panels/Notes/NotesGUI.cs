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

        public NotesGUI(TimelineWizard wizard, ImageRenderer imageRenderer)
            : base("Medical.TimelineGUI.Panels.Notes.NotesGUI.layout", wizard)
        {
            //this.LayerState = "MandibleSizeLayers";
            //this.NavigationState = "WizardMidlineAnterior";
            //this.TextLine1 = "Notes";

            stateNameTextBox = widget.findWidget("Notes/DistortionName") as Edit;
            datePicker = widget.findWidget("Notes/DateCreated") as Edit;
            distortionWizard = widget.findWidget("Notes/DistortionWizard") as Edit;
            notes = widget.findWidget("Notes/NotesText") as Edit;

            thumbnailPicker = new ThumbnailPickerGUI(imageRenderer, widget.findWidget("Notes/Thumbnails") as ScrollView);
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
            setToDefault();

            distortionWizard.Caption = "Piper's JBO";// controller.CurrentWizardName;
            thumbnailPicker.updateThumbnails();
        }

        public void setToDefault()
        {
            stateNameTextBox.Caption = "Custom Distortion";
            notes.Caption = "";
            datePicker.Caption = DateTime.Now.ToString();
        }

        public void applyToState(MedicalState state)
        {
            state.Notes.DataSource = distortionWizard.Caption;
            state.Notes.Notes = notes.Caption;
            state.Notes.ProcedureDate = DateTime.Now;
            state.Name = stateNameTextBox.Caption;
            state.Thumbnail = thumbnailPicker.SelectedThumbnail;
        }

        public bool Visible
        {
            get
            {
                return widget.Visible;
            }
            set
            {
                widget.Visible = value;
            }
        }
    }
}
