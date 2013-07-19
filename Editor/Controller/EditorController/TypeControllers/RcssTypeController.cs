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
    public class RcssTypeController : TextTypeController
    {
        public const String Icon = "EditorFileIcon/.rcss";

        public RcssTypeController(EditorController editorController)
            : base(".rcss", Encoding.UTF8, editorController)
        {

        }

        public override void openEditor(string file)
        {
            if (!EditorController.ResourceProvider.exists(file))
            {
                createNewRcssFile(file);
            }

            base.openEditor(file);
        }

        public void saveFile(String rcss, String file)
        {
            saveText(file, rcss);
            Factory.ClearStyleSheetCache();
            EditorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
        }

        public override ProjectItemTemplate createItemTemplate()
        {
            return new ProjectItemTemplateDelegate("Rcss File", Icon, "File", delegate(String path, String fileName, EditorController editorController)
            {
                String filePath = Path.Combine(path, fileName);
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
            });
        }

        void createNewRcssFile(String filePath)
        {
            creatingNewFile(filePath);
            saveText(filePath, defaultRcss);
            openEditor(filePath);
        }

        private const String defaultRcss = "";
    }
}
