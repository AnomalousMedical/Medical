using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.Xml;
using Medical.Muscles;
using MyGUIPlugin;

namespace Medical
{
    class MovementSequenceTypeController : EditorTypeController
    {
        private MovementSequenceEditor editor;
        private EditorController editorController;
        private ExtensionActionCollection extensionActions = new ExtensionActionCollection();
        private String currentSequenceFile = null;

        public MovementSequenceTypeController(MovementSequenceEditor editor, EditorController editorController)
            :base(".seq")
        {
            this.editor = editor;
            editor.GotFocus += new EventHandler(editor_GotFocus);
            this.editorController = editorController;

            extensionActions.Add(new ExtensionAction("Save Movement Sequence", "File", saveSequence));
            extensionActions.Add(new ExtensionAction("Save Movement Sequence As", "File", saveSequenceAs));
            extensionActions.Add(new ExtensionAction("Cut", "Edit", editor.cut));
            extensionActions.Add(new ExtensionAction("Copy", "Edit", editor.copy));
            extensionActions.Add(new ExtensionAction("Paste", "Edit", editor.paste));
            extensionActions.Add(new ExtensionAction("Select All", "Edit", editor.selectAll));
            extensionActions.Add(new ExtensionAction("Reverse Sides", "Sequence", editor.reverseSides));
        }

        public override void openFile(string file)
        {
            try
            {
                MovementSequence movementSequence = (MovementSequence)editorController.loadObject(file);
                editor.CurrentSequence = movementSequence;
                editor.updateTitle(file);
                if (!editor.Visible)
                {
                    editor.open(false);
                }
                editorController.ExtensionActions = extensionActions;
                editor.bringToFront();
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("Error loading movement sequence {0}.\nReason: {1}", file, ex.Message), "Load Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
        }

        void editor_GotFocus(object sender, EventArgs e)
        {
            editorController.ExtensionActions = extensionActions;
        }

        //void createNewSequence()
        //{
        //    MovementSequence movementSequence = new MovementSequence();
        //    movementSequence.Duration = 5.0f;
        //    movementSequenceController.CurrentSequence = movementSequence;
        //}

        //public void openSequence(String filename)
        //{
        //    try
        //    {
        //        using (XmlReader xmlReader = new XmlTextReader(filename))
        //        {
        //            loadingSequenceFromFile = true;
        //            CurrentSequenceFile = filename;
        //            MovementSequence movementSequence = EditorController.XmlSaver.restoreObject(xmlReader) as MovementSequence;
        //            movementSequenceController.CurrentSequence = movementSequence;
        //            loadingSequenceFromFile = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.show(String.Format("Error opening movement sequence {0}.\nReason: {1}", filename, ex.Message), "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
        //    }
        //}

        void saveSequenceAs()
        {
            using (FileSaveDialog saveDialog = new FileSaveDialog(MainWindow.Instance, "Save a sequence."))
            {
                saveDialog.Wildcard = "Sequence files (*.seq)|*.seq";
                if (saveDialog.showModal() == NativeDialogResult.OK)
                {
                    currentSequenceFile = saveDialog.Path;
                    editorController.saveObject(currentSequenceFile, editor.CurrentSequence);
                    editor.updateTitle(currentSequenceFile);
                }
            }
        }

        void saveSequence()
        {
            if (currentSequenceFile != null)
            {
                editorController.saveObject(currentSequenceFile, editor.CurrentSequence);
            }
            else
            {
                saveSequenceAs();
            }
        }
    }
}
