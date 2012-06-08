using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.IO;
using MyGUIPlugin;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;

namespace Medical
{
    class RmlTypeController : EditorTypeController
    {
        public const String WILDCARD = "RML Files (*.rml)|*.rml";

        //private RmlViewer editor;
        private EditorController editorController;
        private GUIManager guiManager;

        public RmlTypeController(RmlViewer editor, EditorController editorController, GUIManager guiManager)
            :base(".rml")
        {
            //this.editor = editor;
            this.editorController = editorController;
            this.guiManager = guiManager;
        }

        public override void openFile(string file)
        {
            //editor.changeDocument(editorController.ResourceProvider.getFullFilePath(file));
            //if (!editor.Visible)
            //{
            //    editor.open(false);
            //}
            //editor.activateExtensionActions();
            //editor.bringToFront();
            String rmlText = null;
            using (StreamReader streamReader = new StreamReader(editorController.ResourceProvider.openFile(file)))
            {
                rmlText = streamReader.ReadToEnd();
            }
            //TextEditor textEditor = TextEditor.openTextEditor(guiManager);
            //textEditor.Caption = String.Format("{0} - Rml Text Editor", file);
            //textEditor.WordWrap = false;
            //textEditor.Text = rmlText;
            //textEditor.CurrentFile = file;
            //textEditor.GotFocus += new EventHandler(textEditor_GotFocus);
            //textEditor.Closing += new EventHandler<DialogCancelEventArgs>(textEditor_Closing);
            //focusEditor(textEditor);

            AnomalousMvcContext mvcContext = new AnomalousMvcContext();
            mvcContext.Views.add(new TextEditorView("RmlEditor", rmlText, wordWrap: false));
            RmlView rmlView = new RmlView("RmlView");
            rmlView.ViewLocation = ViewLocations.Right;
            rmlView.RmlFile = file;
            mvcContext.Views.add(rmlView);
            EditorInfoBarView infoBar = new EditorInfoBarView("InfoBar", String.Format("{0} - Rml", file), "Editor/Close");
            infoBar.addAction(new EditorInfoBarAction("Close Rml File", "File", "Editor/CloseCurrentFile"));
            infoBar.addAction(new EditorInfoBarAction("Save Rml File", "File", "Editor/Save"));
            infoBar.addAction(new EditorInfoBarAction("Save Rml File As", "File", "Editor/SaveAs"));
            //infoBar.addAction(new EditorInfoBarAction("Cut", "Edit", "Editor/Cut"));
            //infoBar.addAction(new EditorInfoBarAction("Copy", "Edit", "Editor/Copy"));
            //infoBar.addAction(new EditorInfoBarAction("Paste", "Edit", "Editor/Paste"));
            //infoBar.addAction(new EditorInfoBarAction("Select All", "Edit", "Editor/SelectAll"));
            mvcContext.Views.add(infoBar);
            MvcController timelineEditorController = new MvcController("Editor");
            RunCommandsAction showAction = new RunCommandsAction("Show");
            showAction.addCommand(new ShowViewCommand("RmlEditor"));
            showAction.addCommand(new ShowViewCommand("RmlView"));
            showAction.addCommand(new ShowViewCommand("InfoBar"));
            timelineEditorController.Actions.add(showAction);
            RunCommandsAction closeAction = new RunCommandsAction("Close");
            closeAction.addCommand(new CloseAllViewsCommand());
            timelineEditorController.Actions.add(closeAction);
            timelineEditorController.Actions.add(new CallbackAction("CloseCurrentFile", context =>
            {
                close();
                context.runAction("Editor/Close");
            }));
            timelineEditorController.Actions.add(new CallbackAction("Save", context =>
            {
                save();
            }));
            timelineEditorController.Actions.add(new CallbackAction("SaveAs", context =>
            {
                saveAs();
            }));
            //timelineEditorController.Actions.add(new CutAction());
            //timelineEditorController.Actions.add(new CopyAction());
            //timelineEditorController.Actions.add(new PasteAction());
            //timelineEditorController.Actions.add(new SelectAllAction());
            mvcContext.Controllers.add(timelineEditorController);
            MvcController common = new MvcController("Common");
            RunCommandsAction startup = new RunCommandsAction("Start");
            startup.addCommand(new RunActionCommand("Editor/Show"));
            common.Actions.add(startup);
            CallbackAction shutdown = new CallbackAction("Shutdown", context =>
            {

            });
            common.Actions.add(shutdown);
            mvcContext.Controllers.add(common);

            editorController.runEditorContext(mvcContext);
        }

        public void save()
        {
            //if (currentEditor != null)
            //{
            //    using (StreamWriter streamWriter = new StreamWriter(editorController.ResourceProvider.openWriteStream(currentEditor.CurrentFile)))
            //    {
            //        streamWriter.Write(currentEditor.Text);
            //    }
            //}
            //else
            //{
            //    saveAs();
            //}
        }

        public void saveAs()
        {
            //if (currentEditor != null)
            //{
            //    using (FileSaveDialog fileDialog = new FileSaveDialog(MainWindow.Instance, "Save a MVC Context", "", "", WILDCARD))
            //    {
            //        if (fileDialog.showModal() == NativeDialogResult.OK)
            //        {
            //            try
            //            {
            //                using (StreamWriter streamWriter = new StreamWriter(fileDialog.Path))
            //                {
            //                    streamWriter.Write(currentEditor.Text);
            //                }
            //                currentEditor.CurrentFile = fileDialog.Path;
            //                currentEditor.Caption = String.Format("{0} - Rml Text Editor", currentEditor.CurrentFile);
            //            }
            //            catch (Exception e)
            //            {
            //                MessageBox.show("Save error", String.Format("Exception saving RML File to {0}:\n{1}.", fileDialog.Path, e.Message), MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            //            }
            //        }
            //    }
            //}
        }

        public override void addCreationMethod(ContextMenu contextMenu, string path, bool isDirectory, bool isTopLevel)
        {
            contextMenu.add(new ContextMenuItem("Create Rml File", path, delegate(ContextMenuItem item)
            {
                InputBox.GetInput("Rml File Name", "Enter a name for the rml file.", true, delegate(String result, ref String errorMessage)
                {
                    String filePath = Path.Combine(path, result);
                    filePath = Path.ChangeExtension(filePath, ".rml");
                    if (editorController.ResourceProvider.exists(filePath))
                    {
                        MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle overrideResult)
                        {
                            if (overrideResult == MessageBoxStyle.Yes)
                            {
                                createNewRmlFile(filePath);
                            }
                        });
                    }
                    else
                    {
                        createNewRmlFile(filePath);
                    }
                    return true;
                });
            }));
        }

        private void close()
        {
            //if (currentEditor != null)
            //{
            //    currentEditor.Visible = false;
            //}
        }

        void createNewRmlFile(String filePath)
        {
            Timeline timeline = new Timeline();
            using (StreamWriter sw = new StreamWriter(editorController.ResourceProvider.openWriteStream(filePath)))
            {
                sw.Write(defaultRml);
            }
            openFile(filePath);
        }

        private const String defaultRml = @"<rml>
  <head>
    <link type=""text/rcss"" href=""/libRocketPlugin.Resources.rkt.rcss""/>
    <link type=""text/rcss"" href=""/libRocketPlugin.Resources.Anomalous.rcss""/>
  </head>
  <body>
    <div class=""ScrollArea"">
      <h1>Empty Rml View</h1>
      <p>You can start creating your Rml View here. You can erase this text to start.</p>
    </div>
  </body>
</rml>
";
    }
}
