using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class NotesTaskbarItem : TaskbarItem
    {
        private NotesDialog notesDialog;

        public NotesTaskbarItem(MedicalStateController stateController)
            :base("Notes", "Notes")
        {
            notesDialog = new NotesDialog(stateController);
            notesDialog.Shown += new EventHandler(notesDialog_Shown);
            notesDialog.Closed += new EventHandler(notesDialog_Closed);
        }

        public override void Dispose()
        {
            notesDialog.Dispose();
            base.Dispose();
        }

        public override void clicked(Widget source, EventArgs e)
        {
            if (!notesDialog.Visible)
            {
                notesDialog.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
            }
            notesDialog.Visible = !notesDialog.Visible;
        }

        void notesDialog_Closed(object sender, EventArgs e)
        {
            taskbarButton.StateCheck = false;
        }

        void notesDialog_Shown(object sender, EventArgs e)
        {
            taskbarButton.StateCheck = true;
        }
    }
}
