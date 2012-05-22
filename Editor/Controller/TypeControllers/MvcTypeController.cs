using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;
using System.IO;
using Medical.Editor;

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
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);

            extensionActions.Add(new ExtensionAction("Close MVC Context", "File", close));
            extensionActions.Add(new ExtensionAction("Save MVC Context", "File", save));
            extensionActions.Add(new ExtensionAction("Save MVC Context As", "File", saveAs));
        }

        public override void openFile(string file)
        {
            context = (AnomalousMvcContext)loadObject(file);
            BrowserWindowController.setCurrentEditingMvcContext(context);
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

        public override void addCreationMethod(ContextMenu contextMenu, string path, bool isDirectory, bool isTopLevel)
        {
            contextMenu.add(new ContextMenuItem("Create MVC Context", path, delegate(ContextMenuItem item)
            {
                InputBox.GetInput("MVC Context Name", "Enter a name for the MVC Context.", true, delegate(String result, ref String errorMessage)
                {
                    String filePath = Path.Combine(path, result);
                    filePath = Path.ChangeExtension(filePath, ".mvc");
                    if (editorController.ResourceProvider.exists(filePath))
                    {
                        MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle overrideResult)
                        {
                            if (overrideResult == MessageBoxStyle.Yes)
                            {
                                createNewContext(filePath);
                            }
                        });
                    }
                    else
                    {
                        createNewContext(filePath);
                    }
                    return true;
                });
            }));
        }

        void createNewContext(String filePath)
        {
            AnomalousMvcContext mvcContext = new AnomalousMvcContext();
            creatingNewFile(filePath);
            saveObject(filePath, mvcContext);
            openFile(filePath);
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            close();
        }

        private void close()
        {
            editor.CurrentEditInterface = null;
            editor.changeCaption(null);
            closeCurrentCachedResource();
        }
    }
}
