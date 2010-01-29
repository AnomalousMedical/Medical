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
            this.procedureType.Text = procedureType;
            this.Text = "Notes";

            thumbnailPicker.ImageRenderer = imageRenderer;
            thumbnailPicker.addThumbnail("Midline Anterior", "MandibleSizeLayers");
            thumbnailPicker.addThumbnail("Right Lateral", "MandibleSizeLayers");
            thumbnailPicker.addThumbnail("Left Lateral", "MandibleSizeLayers");
            thumbnailPicker.addThumbnail("Right TMJ Thumbnail", "DiscLayers");
            thumbnailPicker.addThumbnail("Left TMJ Thumbnail", "DiscLayers");
            thumbnailPicker.addThumbnail("Teeth Midline Anterior", "TeethLayers");
            thumbnailPicker.addThumbnail("Teeth Right Lateral", "TeethLayers");
            thumbnailPicker.addThumbnail("Teeth Left Lateral", "TeethLayers");
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
            state.Thumbnail = thumbnailPicker.SelectedThumbnail;
        }

        protected override void onPanelOpening()
        {
            thumbnailPicker.updateThumbnails();
        }
    }
}
