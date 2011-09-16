using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;

namespace Medical.GUI
{
    partial class NotesGUIData : TimelineWizardPanelData
    {
        [DoNotSave]
        private List<NotesThumbnail> thumbnails = new List<NotesThumbnail>();

        public NotesGUIData()
        {
            
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

        public override string Name
        {
            get { return "NotesGUIData"; }
        }

        protected NotesGUIData(LoadInfo info)
            :base(info)
        {
            info.RebuildList<NotesThumbnail>("NotesThumbnail", thumbnails);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractList<NotesThumbnail>("NotesThumbnail", thumbnails);
        }
    }

    partial class NotesGUIData
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
