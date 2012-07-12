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
        public const String Icon = "EditorFileIcon/.rcss";

        public RcssTypeController(EditorController editorController)
            : base(".rcss", editorController)
        {

        }

        public override void openFile(string file)
        {
            if (!EditorController.ResourceProvider.exists(file))
            {
                createNewRcssFile(file);
            }

            base.openFile(file);
        }

        public void saveFile(String rcss, String file)
        {
            saveText(file, rcss);
            Factory.ClearStyleSheetCache();
            EditorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
        }

        public override void addCreationMethod(ContextMenu contextMenu, string path, bool isDirectory, bool isTopLevel)
        {
            contextMenu.add(new ContextMenuItem("Create Rcss File", path, delegate(ContextMenuItem item)
            {
                InputBox.GetInput("Rcss File Name", "Enter a name for the rcss file.", true, delegate(String result, ref String errorMessage)
                {
                    String filePath = Path.Combine(path, result);
                    filePath = Path.ChangeExtension(filePath, ".rcss");
                    if (EditorController.ResourceProvider.exists(filePath))
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

        private const String defaultRcss = "";
    }
}
