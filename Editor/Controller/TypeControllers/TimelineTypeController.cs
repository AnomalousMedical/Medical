using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.IO;
using System.Xml;
using MyGUIPlugin;
using Logging;

namespace Medical
{
    class TimelineTypeController : SaveableTypeController
    {
        public const String TIMELINE_WILDCARD = "Timelines (*.tl)|*.tl";

        private TimelineEditor editor;
        private ExtensionActionCollection extensionActions = new ExtensionActionCollection();
        private EditorController editorController;
        private String currentFile;

        public TimelineTypeController(TimelineEditor editor, EditorController editorController)
            :base(".tl", editorController)
        {
            this.editor = editor;
            this.editorController = editorController;
            editor.GotFocus += new EventHandler(editor_GotFocus);

            extensionActions.Add(new ExtensionAction("Save Timeline", "File", saveTimeline));
            extensionActions.Add(new ExtensionAction("Save Timeline As", "File", saveTimelineAs));
            extensionActions.Add(new ExtensionAction("Cut", "Edit", editor.cut));
            extensionActions.Add(new ExtensionAction("Copy", "Edit", editor.copy));
            extensionActions.Add(new ExtensionAction("Paste", "Edit", editor.paste));
            extensionActions.Add(new ExtensionAction("Select All", "Edit", editor.selectAll));
        }

        public override void openFile(string path)
        {
            editor.CurrentTimeline = (Timeline)loadObject(path);
            currentFile = path;
            editor.updateFileName(currentFile);
            if (!editor.Visible)
            {
                editor.open(false);
            }
            editorController.ExtensionActions = extensionActions;
            editor.bringToFront();
        }

        void editor_GotFocus(object sender, EventArgs e)
        {
            editorController.ExtensionActions = extensionActions;
        }

        private void saveTimeline()
        {
            if (editor.CurrentTimeline != null)
            {
                saveTimeline(editor.CurrentTimeline, currentFile);
            }
            else
            {
                saveTimelineAs();
            }
        }

        private void saveTimelineAs()
        {
            using (FileSaveDialog saveDialog = new FileSaveDialog(MainWindow.Instance, "Save Timeline", "", "", TIMELINE_WILDCARD))
            {
                if (saveDialog.showModal() == NativeDialogResult.OK)
                {
                    saveTimeline(editor.CurrentTimeline, saveDialog.Path);
                }
            }
        }

        private void saveTimeline(Timeline timeline, String filename)
        {
            try
            {
                saveObject(filename, timeline);
                timeline.LEGACY_SourceFile = filename;
                currentFile = filename;
                editor.updateFileName(currentFile);
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("There was an error saving your timeline to\n'{0}'\nPlease make sure that destination is valid.", filename), "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                Log.Error("Could not save timeline. {0}", ex.Message);
            }
        }
    }
}
