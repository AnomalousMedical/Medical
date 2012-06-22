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
    class RcssTypeController : TextTypeController
    {
        private EditorController editorController;
        private GUIManager guiManager;
        public const String Icon = "Editor/RCSSIcon";
        private RcssEditorContext rcssContext;
        private RmlTypeController rmlTypeController;

        public RcssTypeController(EditorController editorController, GUIManager guiManager, RmlTypeController rmlTypeController)
            : base(".rcss", editorController)
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

            String rmlText = loadText(file);

            rcssContext = new RcssEditorContext(rmlText, file, rmlTypeController.LastRmlFile, this);
            rcssContext.Shutdown += new Action<RcssEditorContext>(rcssContext_Shutdown);
            editorController.runEditorContext(rcssContext.MvcContext);
        }

        public void saveFile(String rcss, String file)
        {
            saveText(file, rcss);
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
            creatingNewFile(filePath);
            saveText(filePath, defaultRcss);
            openFile(filePath);
        }

        void rcssContext_Shutdown(RcssEditorContext obj)
        {
            updateCachedText(obj.CurrentFile, obj.CurrentText);
            if (rcssContext == obj)
            {
                rcssContext = null;
            }
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            closeCurrentCachedResource();
            if (rcssContext != null)
            {
                rcssContext.close();
            }
        }

        private const String defaultRcss = "";
    }
}
