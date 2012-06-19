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
        private EditorController editorController;
        private GUIManager guiManager;
        public const String Icon = "RmlEditorIcon";
        private RcssEditorContext rcssContext;
        private RmlTypeController rmlTypeController;

        public RcssTypeController(EditorController editorController, GUIManager guiManager, RmlTypeController rmlTypeController)
            : base(".rcss")
        {
            this.editorController = editorController;
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);
            this.guiManager = guiManager;
            this.rmlTypeController = rmlTypeController;
        }

        public override void openFile(string file)
        {
            if (!editorController.ResourceProvider.exists(file))
            {
                createNewRcssFile(file);
            }

            String rmlText = null;
            using (StreamReader streamReader = new StreamReader(editorController.ResourceProvider.openFile(file)))
            {
                rmlText = streamReader.ReadToEnd();
            }
            rcssContext = new RcssEditorContext(rmlText, file, rmlTypeController.LastRmlFile, this);
            rcssContext.Shutdown += new Action<RcssEditorContext>(rcssContext_Shutdown);
            editorController.runEditorContext(rcssContext.MvcContext);
        }

        public override void closeFile(string file)
        {
            //does nothing for now cause these are not cached
        }

        public void saveFile(String rcss, String file)
        {
            using (StreamWriter streamWriter = new StreamWriter(editorController.ResourceProvider.openWriteStream(file)))
            {
                streamWriter.Write(rcss);
            }
            Factory.ClearStyleSheetCache();
            editorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
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

        void createNewRcssFile(String filePath)
        {
            Timeline timeline = new Timeline();
            using (StreamWriter sw = new StreamWriter(editorController.ResourceProvider.openWriteStream(filePath)))
            {
                sw.Write(defaultRcss);
            }
            openFile(filePath);
        }

        void rcssContext_Shutdown(RcssEditorContext obj)
        {
            if (rcssContext == obj)
            {
                rcssContext = null;
            }
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            if (rcssContext != null)
            {
                rcssContext.close();
            }
        }

        private const String defaultRcss = "";
    }
}
