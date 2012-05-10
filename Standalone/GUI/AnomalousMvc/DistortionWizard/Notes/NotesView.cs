using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;
using Engine.Attributes;
using Medical.Model;

namespace Medical.GUI.AnomalousMvc
{
    partial class NotesView : WizardView
    {
        [DoNotSave]
        private List<NotesThumbnail> thumbnails = new List<NotesThumbnail>();

        public NotesView(String name)
            : base(name)
        {
            AttachToScrollView = false;
            WizardStateInfoName = MedicalStateInfoModel.DefaultName;
        }

        public void addThumbnail(NotesThumbnail thumbnail)
        {
            thumbnails.Add(thumbnail);
            onThumbnailAdded(thumbnail);
        }

        public void removeThumbnail(NotesThumbnail thumbnail)
        {
            thumbnails.Remove(thumbnail);
            onThumbnailRemoved(thumbnail);
        }

        public IEnumerable<NotesThumbnail> Thumbnails
        {
            get
            {
                return thumbnails;
            }
        }

        [Editable]
        public String WizardStateInfoName { get; set; }

        public override ViewHost createViewHost(AnomalousMvcContext context)
        {
            return new NotesGUI(this, context);
        }

        protected NotesView(LoadInfo info)
            :base(info)
        {
            info.RebuildList<NotesThumbnail>("Thumb", thumbnails);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractList<NotesThumbnail>("Thumb", thumbnails);
        }
    }

    partial class NotesView
    {
        private EditInterfaceManager<NotesThumbnail> dataFieldEdits;

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addCommand(new EditInterfaceCommand("Add Thumbnail", addThumbnail));
            dataFieldEdits = new EditInterfaceManager<NotesThumbnail>(editInterface);
            dataFieldEdits.addCommand(new EditInterfaceCommand("Remove", removeThumbnail));
            foreach (NotesThumbnail thumb in thumbnails)
            {
                onThumbnailAdded(thumb);
            }
        }

        private void addThumbnail(EditUICallback callback, EditInterfaceCommand command)
        {
            addThumbnail(new NotesThumbnail());
        }

        private void removeThumbnail(EditUICallback callback, EditInterfaceCommand command)
        {
            NotesThumbnail thumb = dataFieldEdits.resolveSourceObject(callback.getSelectedEditInterface());
            removeThumbnail(thumb);
        }

        private void onThumbnailAdded(NotesThumbnail notesThumbnail)
        {
            if (dataFieldEdits != null)
            {
                dataFieldEdits.addSubInterface(notesThumbnail, notesThumbnail.EditInterface);
            }
        }

        private void onThumbnailRemoved(NotesThumbnail notesThumbnail)
        {
            if (dataFieldEdits != null)
            {
                dataFieldEdits.removeSubInterface(notesThumbnail);
            }
        }
    }
}
