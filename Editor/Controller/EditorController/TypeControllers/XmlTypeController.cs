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
    public class XmlTypeController : TextTypeController
    {
        public const String Icon = CommonResources.NoIcon;

        public XmlTypeController(EditorController editorController)
            : base(".xml", editorController)
        {

        }

        public override void openEditor(string file)
        {
            if (!EditorController.ResourceProvider.exists(file))
            {
                createNewXmlFile(file);
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
            return new ProjectItemTemplateDelegate("Xml File", Icon, "File", delegate(String path, String fileName, EditorController editorController)
            {
                String filePath = Path.Combine(path, fileName);
                filePath = Path.ChangeExtension(filePath, ".xml");
                if (EditorController.ResourceProvider.exists(filePath))
                {
                    MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle overrideResult)
                    {
                        if (overrideResult == MessageBoxStyle.Yes)
                        {
                            createNewXmlFile(filePath);
                        }
                    });
                }
                else
                {
                    createNewXmlFile(filePath);
                }
            });
        }

        void createNewXmlFile(String filePath)
        {
            creatingNewFile(filePath);
            saveText(filePath, defaultXml);
            openEditor(filePath);
        }

        private const String defaultXml = "";
    }
}
