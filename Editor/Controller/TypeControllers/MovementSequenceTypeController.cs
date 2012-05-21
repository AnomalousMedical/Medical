using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.Xml;
using Medical.Muscles;
using MyGUIPlugin;
using System.IO;

namespace Medical
{
    class MovementSequenceTypeController : SaveableTypeController
    {
        private MovementSequenceEditor editor;
        private EditorController editorController;
        private ExtensionActionCollection extensionActions = new ExtensionActionCollection();
        private String currentSequenceFile = null;

        public MovementSequenceTypeController(MovementSequenceEditor editor, EditorController editorController)
            :base(".seq", editorController)
        {
            this.editor = editor;
            editor.GotFocus += new EventHandler(editor_GotFocus);
            this.editorController = editorController;
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);

            extensionActions.Add(new ExtensionAction("Close Movement Sequence", "File", close));
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
                MovementSequence movementSequence = (MovementSequence)loadObject(file);
                editor.CurrentSequence = movementSequence;
                currentSequenceFile = file;
                editor.updateTitle(currentSequenceFile);
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

        public override void addCreationMethod(ContextMenu contextMenu, string path, bool isDirectory, bool isTopLevel)
        {
            contextMenu.add(new ContextMenuItem("Create Movement Sequence", path, delegate(ContextMenuItem item)
            {
                InputBox.GetInput("Movement Sequence Name", "Enter a name for the movement sequence.", true, delegate(String result, ref String errorMessage)
                {
                    String filePath = Path.Combine(path, result);
                    filePath = Path.ChangeExtension(filePath, ".seq");
                    if (editorController.ResourceProvider.exists(filePath))
                    {
                        MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle overrideResult)
                        {
                            if (overrideResult == MessageBoxStyle.Yes)
                            {
                                createNewMovementSequence(filePath);
                            }
                        });
                    }
                    else
                    {
                        createNewMovementSequence(filePath);
                    }
                    return true;
                });
            }));
        }

        void createNewMovementSequence(String filePath)
        {
            MovementSequence movementSequence = new MovementSequence();
            movementSequence.Duration = 5.0f;
            creatingNewFile(filePath);
            saveObject(filePath, movementSequence);
            openFile(filePath);
        }

        void editor_GotFocus(object sender, EventArgs e)
        {
            editorController.ExtensionActions = extensionActions;
        }

        void saveSequenceAs()
        {
            using (FileSaveDialog saveDialog = new FileSaveDialog(MainWindow.Instance, "Save a sequence."))
            {
                saveDialog.Wildcard = "Sequence files (*.seq)|*.seq";
                if (saveDialog.showModal() == NativeDialogResult.OK)
                {
                    currentSequenceFile = saveDialog.Path;
                    saveObject(currentSequenceFile, editor.CurrentSequence);
                    editor.updateTitle(currentSequenceFile);
                }
            }
        }

        void saveSequence()
        {
            if (currentSequenceFile != null)
            {
                saveObject(currentSequenceFile, editor.CurrentSequence);
            }
            else
            {
                saveSequenceAs();
            }
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            close();
        }

        private void close()
        {
            editor.CurrentSequence = null;
            editor.updateTitle(null);
            closeCurrentCachedResource();
        }
    }
}
