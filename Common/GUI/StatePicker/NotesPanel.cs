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
        public NotesPanel(String procedureType, ImageRenderer imageRenderer, StatePickerPanelController panelController)
            :base(panelController)
        {
            InitializeComponent();
            this.distortionWizard.Text = procedureType;
            this.Text = "Notes";

            thumbnailPicker.ImageRenderer = imageRenderer;
            thumbnailPicker.addThumbnail("ThumbnailMidlineAnterior", "MandibleSizeLayers");
            thumbnailPicker.addThumbnail("ThumbnailRightLateral", "MandibleSizeLayers");
            thumbnailPicker.addThumbnail("ThumbnailLeftLateral", "MandibleSizeLayers");
            thumbnailPicker.addThumbnail("ThumbnailRightTMJ", "DiscLayers");
            thumbnailPicker.addThumbnail("ThumbnailLeftTMJ", "DiscLayers");
            thumbnailPicker.addThumbnail("ThumbnailTeethMidlineAnterior", "TeethLayers");
            thumbnailPicker.addThumbnail("ThumbnailTeethRightLateral", "TeethLayers");
            thumbnailPicker.addThumbnail("ThumbnailTeethLeftLateral", "TeethLayers");
        }

        public override void setToDefault()
        {
            notes.Text = "";
            datePicker.Value = DateTime.Now;
        }

        public override void applyToState(MedicalState state)
        {
            state.Notes.DataSource = distortionWizard.Text;
            state.Notes.Notes = notes.Rtf;
            state.Notes.ProcedureDate = datePicker.Value;
            state.Name = stateNameTextBox.Text;
            state.Thumbnail = thumbnailPicker.SelectedThumbnail;
        }

        public String DistortionWizardText
        {
            get
            {
                return distortionWizard.Text;
            }
            set
            {
                distortionWizard.Text = value;
            }
        }

        protected override void onPanelOpening()
        {
            thumbnailPicker.updateThumbnails();
        }
    }
}
