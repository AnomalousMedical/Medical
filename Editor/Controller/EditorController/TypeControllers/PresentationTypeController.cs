using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Presentation;
using Medical.GUI;
using MyGUIPlugin;
using System.IO;
using Medical.Controller.AnomalousMvc;

namespace Medical
{
    public class PresentationTypeController : SaveableTypeController<PresentationIndex>
    {
        public const String PresentationExtension = ".amp";
        private const String Icon = "EditorFileIcon/" + PresentationExtension;


        public PresentationTypeController(EditorController editorController)
            : base(PresentationExtension, editorController)
        {
            
        }

        public void saveFile(PresentationIndex presentation, string file)
        {
            saveObject(file, presentation);
            EditorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
        }

        public override ProjectItemTemplate createItemTemplate()
        {
            return new ProjectItemTemplateDelegate("Presentation", Icon, "File", delegate(String path, String fileName, EditorController editorController)
            {
                String filePath = Path.Combine(path, fileName);
                filePath = Path.ChangeExtension(filePath, ".amp");
                if (EditorController.ResourceProvider.exists(filePath))
                {
                    MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle overrideResult)
                    {
                        if (overrideResult == MessageBoxStyle.Yes)
                        {
                            createNewPresentation(filePath);
                        }
                    });
                }
                else
                {
                    createNewPresentation(filePath);
                }
            });
        }

        private void createNewPresentation(String filePath)
        {
            PresentationIndex newPresentation = new PresentationIndex();
            creatingNewFile(filePath);
            saveObject(filePath, newPresentation);
            openEditor(filePath);
        }
    }
}
