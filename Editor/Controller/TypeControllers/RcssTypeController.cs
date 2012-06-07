using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.IO;
using MyGUIPlugin;
using libRocketPlugin;

namespace Medical
{
    class RcssTypeController : EditorTypeController
    {
        public const String WILDCARD = "RCSS Files (*.rcss)|*.rcss";

        private EditorController editorController;
        private GUIManager guiManager;
        private ExtensionActionCollection extensionActions = new ExtensionActionCollection();
        private TextEditor currentEditor;

        public RcssTypeController(EditorController editorController, GUIManager guiManager)
            : base(".rcss")
        {
            //this.editor = editor;
            this.editorController = editorController;
            this.guiManager = guiManager;

            extensionActions.Add(new ExtensionAction("Close RCSS File", "File", close));
            extensionActions.Add(new ExtensionAction("Save RCSS File", "File", save));
            extensionActions.Add(new ExtensionAction("Save RCSS File As", "File", saveAs));
        }

        public override void openFile(string file)
        {
            String rmlText = null;
            using (StreamReader streamReader = new StreamReader(editorController.ResourceProvider.openFile(file)))
            {
                rmlText = streamReader.ReadToEnd();
            }
            TextEditor textEditor = TextEditor.openTextEditor(guiManager);
            textEditor.Caption = String.Format("{0} - Rcss Text Editor", file);
            textEditor.WordWrap = false;
            textEditor.Text = rmlText;
            textEditor.CurrentFile = file;
            textEditor.GotFocus += new EventHandler(textEditor_GotFocus);
            textEditor.Closing += new EventHandler<DialogCancelEventArgs>(textEditor_Closing);
            focusEditor(textEditor);
        }

        public void save()
        {
            if (currentEditor != null)
            {
                using (StreamWriter streamWriter = new StreamWriter(editorController.ResourceProvider.openWriteStream(currentEditor.CurrentFile)))
                {
                    streamWriter.Write(currentEditor.Text);
                }
                Factory.ClearStyleSheetCache();
            }
            else
            {
                saveAs();
            }
        }

        public void saveAs()
        {
            if (currentEditor != null)
            {
                using (FileSaveDialog fileDialog = new FileSaveDialog(MainWindow.Instance, "Save a Rcss File", "", "", WILDCARD))
                {
                    if (fileDialog.showModal() == NativeDialogResult.OK)
                    {
                        try
                        {
                            using (StreamWriter streamWriter = new StreamWriter(fileDialog.Path))
                            {
                                streamWriter.Write(currentEditor.Text);
                            }
                            currentEditor.CurrentFile = fileDialog.Path;
                            currentEditor.Caption = String.Format("{0} - Rcss Text Editor", currentEditor.CurrentFile);
                        }
                        catch (Exception e)
                        {
                            MessageBox.show("Save error", String.Format("Exception saving Rcss File to {0}:\n{1}.", fileDialog.Path, e.Message), MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                        }
                    }
                }
            }
        }

        void textEditor_GotFocus(object sender, EventArgs e)
        {
            focusEditor((TextEditor)sender);
        }

        void textEditor_Closing(object sender, DialogCancelEventArgs e)
        {
            if (currentEditor == (TextEditor)sender)
            {
                currentEditor = null;
                if (editorController.ExtensionActions == extensionActions)
                {
                    editorController.ExtensionActions = null;
                }
            }
        }

        public override void addCreationMethod(ContextMenu contextMenu, string path, bool isDirectory, bool isTopLevel)
        {
            contextMenu.add(new ContextMenuItem("Create Rcss File", path, delegate(ContextMenuItem item)
            {
                InputBox.GetInput("Rcss File Name", "Enter a name for the rcss file.", true, delegate(String result, ref String errorMessage)
                {
                    String filePath = Path.Combine(path, result);
                    filePath = Path.ChangeExtension(filePath, ".rcss");
                    if (editorController.ResourceProvider.exists(filePath))
                    {
                        MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle overrideResult)
                        {
                            if (overrideResult == MessageBoxStyle.Yes)
                            {
                                createNewRcssFile(filePath);
                            }
                        });
                    }
                    else
                    {
                        createNewRcssFile(filePath);
                    }
                    return true;
                });
            }));
        }

        private void focusEditor(TextEditor sender)
        {
            currentEditor = sender;
            editorController.ExtensionActions = extensionActions;
        }

        private void close()
        {
            if (currentEditor != null)
            {
                currentEditor.Visible = false;
            }
        }

        void createNewRcssFile(String filePath)
        {
            Timeline timeline = new Timeline();
            using (StreamWriter sw = new StreamWriter(editorController.ResourceProvider.openWriteStream(filePath)))
            {
                sw.Write(defaultRcss);
            }
            openFile(filePath);
        }

        private const String defaultRcss = "";
    }
}
