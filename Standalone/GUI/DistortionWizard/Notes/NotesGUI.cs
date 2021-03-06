﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.AnomalousMvc;
using Medical.Model;

namespace Medical.GUI.AnomalousMvc
{
    class NotesGUI : WizardComponent<NotesView>
    {
        EditBox stateNameTextBox;
        EditBox datePicker;
        EditBox distortionWizard;
        EditBox notes;
        private MedicalStateInfoModel stateInfo;

        private ThumbnailPickerGUI thumbnailPicker;

        public NotesGUI(NotesView wizardView, AnomalousMvcContext context, MyGUIViewHost viewHost)
            : base("Medical.GUI.DistortionWizard.Notes.NotesGUI.layout", wizardView, context, viewHost)
        {
            stateNameTextBox = widget.findWidget("Notes/DistortionName") as EditBox;
            stateNameTextBox.EventEditTextChange += new MyGUIEvent(stateNameTextBox_EventEditTextChange);
            datePicker = widget.findWidget("Notes/DateCreated") as EditBox;
            distortionWizard = widget.findWidget("Notes/DistortionWizard") as EditBox;
            notes = widget.findWidget("Notes/NotesText") as EditBox;
            notes.EventEditTextChange += new MyGUIEvent(notes_EventEditTextChange);

            thumbnailPicker = new ThumbnailPickerGUI(context.ImageRenderer, widget.findWidget("Notes/Thumbnails") as ScrollView);
            thumbnailPicker.SelectedThumbnailChanged += thumbnailPicker_SelectedThumbnailChanged;
        }

        public override void Dispose()
        {
            thumbnailPicker.SelectedThumbnailChanged -= thumbnailPicker_SelectedThumbnailChanged;
            thumbnailPicker.Dispose();
            base.Dispose();
        }

        public override void opening()
        {
            stateInfo = context.getModel<MedicalStateInfoModel>(wizardView.WizardStateInfoName);
            if (stateInfo == null)
            {
                stateInfo = new MedicalStateInfoModel("MissingStateInfo");
                context.addModel(wizardView.WizardStateInfoName, stateInfo);
            }

            distortionWizard.OnlyText = stateInfo.DataSource;
            stateNameTextBox.OnlyText = stateInfo.StateName;
            notes.OnlyText = stateInfo.Notes;
            datePicker.Caption = stateInfo.ProcedureDate.ToString();

            foreach (NotesThumbnail thumb in wizardView.Thumbnails)
            {
                thumbnailPicker.addThumbnail(thumb);
            }
            thumbnailPicker.updateThumbnails();
        }

        void stateNameTextBox_EventEditTextChange(Widget source, EventArgs e)
        {
            stateInfo.StateName = stateNameTextBox.OnlyText;
        }

        void notes_EventEditTextChange(Widget source, EventArgs e)
        {
            stateInfo.Notes = notes.OnlyText;
        }

        void thumbnailPicker_SelectedThumbnailChanged(ThumbnailPickerGUI thumbPicker)
        {
            stateInfo.ThumbInfo = thumbPicker.SelectedThumbnailProperties;
        }
    }
}
