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
    public class MvcTypeController : SaveableTypeController<AnomalousMvcContext>
    {
        public const String Icon = "EditorFileIcon/.mvc";

        public MvcTypeController(EditorController editorController)
            :base(".mvc", editorController)
        {
            
        }

        public void saveFile(AnomalousMvcContext context, string file)
        {
            saveObject(file, context);
            EditorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
        }

        public override ProjectItemTemplate createItemTemplate()
        {
            return new ProjectItemTemplateDelegate("MVC Context", Icon, "File", delegate(String path, String fileName, EditorController editorController)
            {
                String filePath = Path.Combine(path, fileName);
                filePath = Path.ChangeExtension(filePath, ".mvc");
                if (EditorController.ResourceProvider.exists(filePath))
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
            });
        }

        void createNewContext(String filePath)
        {
            AnomalousMvcContext mvcContext = new AnomalousMvcContext();
            creatingNewFile(filePath);
            saveObject(filePath, mvcContext);
            openEditor(filePath);
        }
    }
}
