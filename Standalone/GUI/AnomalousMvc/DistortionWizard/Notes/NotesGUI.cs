using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.AnomalousMvc;
using Medical.Model;

namespace Medical.GUI.AnomalousMvc
{
    class NotesGUI : WizardPanel<NotesView>
    {
        EditBox stateNameTextBox;
        EditBox datePicker;
        EditBox distortionWizard;
        EditBox notes;

        private ThumbnailPickerGUI thumbnailPicker;

        public NotesGUI(NotesView wizardView, AnomalousMvcContext context)
            : base("Medical.GUI.AnomalousMvc.DistortionWizard.Notes.NotesGUI.layout", wizardView, context)
        {
            stateNameTextBox = widget.findWidget("Notes/DistortionName") as EditBox;
            datePicker = widget.findWidget("Notes/DateCreated") as EditBox;
            distortionWizard = widget.findWidget("Notes/DistortionWizard") as EditBox;
            notes = widget.findWidget("Notes/NotesText") as EditBox;

            thumbnailPicker = new ThumbnailPickerGUI(context.ImageRenderer, widget.findWidget("Notes/Thumbnails") as ScrollView);
        }

        public override void Dispose()
        {
            thumbnailPicker.Dispose();
            base.Dispose();
        }

        public override void opening()
        {
            WizardStateInfo wizardStateInfo = context.getModel<WizardStateInfo>(wizardView.WizardStateInfoName);
            if (wizardStateInfo == null)
            {
                wizardStateInfo = new WizardStateInfo();
            }

            distortionWizard.OnlyText = wizardStateInfo.DataSource;
            stateNameTextBox.OnlyText = wizardStateInfo.Name;
            notes.OnlyText = wizardStateInfo.Notes;
            datePicker.Caption = wizardStateInfo.ProcedureDate.ToString();

            foreach (NotesThumbnail thumb in wizardView.Thumbnails)
            {
                thumbnailPicker.addThumbnail(thumb);
            }
            thumbnailPicker.updateThumbnails();
        }

        //protected override void commitOutstandingData()
        //{
        //    wizard.StateName = stateNameTextBox.OnlyText;
        //    wizard.Notes = notes.OnlyText;
        //    wizard.Thumbnail = thumbnailPicker.SelectedThumbnail;
        //}
    }
}
