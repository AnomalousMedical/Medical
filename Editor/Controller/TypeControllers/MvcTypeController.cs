using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;

namespace Medical
{
    class MvcTypeController : SaveableTypeController
    {
        public const String WILDCARD = "MVC Contexts (*.mvc)|*.mvc";

        private GenericEditor editor;
        private EditorController editorController;
        private ExtensionActionCollection extensionActions = new ExtensionActionCollection();
        private String currentFile;
        private AnomalousMvcContext context;

        public MvcTypeController(GenericEditor editor, EditorController editorController)
            :base(".mvc", editorController)
        {
            this.editor = editor;
            editor.GotFocus += new EventHandler(editor_GotFocus);
            this.editorController = editorController;

            extensionActions.Add(new ExtensionAction("Save MVC Context", "File", save));
            extensionActions.Add(new ExtensionAction("Save MVC Context As", "File", saveAs));
        }

        public override void openFile(string file)
        {
            context = (AnomalousMvcContext)loadObject(file);
            currentFile = file;
            editor.CurrentEditInterface = context.getEditInterface();
            editor.changeCaption(currentFile);
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

        public void save()
        {
            if (currentFile != null)
            {
                saveObject(currentFile, context);
            }
            else
            {
                saveAs();
            }
        }

        public void saveAs()
        {
            using (FileSaveDialog fileDialog = new FileSaveDialog(MainWindow.Instance, "Save a MVC Context", "", "", WILDCARD))
            {
                if (fileDialog.showModal() == NativeDialogResult.OK)
                {
                    try
                    {
                        currentFile = fileDialog.Path;
                        saveObject(currentFile, context);
                        editor.changeCaption(currentFile);
                    }
                    catch (Exception e)
                    {
                        MessageBox.show("Save error", String.Format("Exception saving MVC Context to {0}:\n{1}.", currentFile, e.Message), MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                    }
                }
            }
        }
    }
}
