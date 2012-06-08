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
            
            //TextEditor textEditor = TextEditor.openTextEditor(guiManager);
            //textEditor.Caption = String.Format("{0} - Rml Text Editor", file);
            //textEditor.WordWrap = false;
            //textEditor.Text = rmlText;
            //textEditor.CurrentFile = file;
            //textEditor.GotFocus += new EventHandler(textEditor_GotFocus);
            //textEditor.Closing += new EventHandler<DialogCancelEventArgs>(textEditor_Closing);
            //focusEditor(textEditor);

            RmlEditorContext editorContext = new RmlEditorContext(editorController, file);
            editorController.runEditorContext(editorContext.MvcContext);
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
