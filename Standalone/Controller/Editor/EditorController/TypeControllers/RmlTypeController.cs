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
    public class RmlTypeController : TextTypeController
    {
        public const String Icon = "EditorFileIcon/.rml";

        public RmlTypeController(EditorController editorController)
            :base(".rml", editorController)
        {
            
        }

        public override void openEditor(string file)
        {
            if (!EditorController.ResourceProvider.exists(file))
            {
                createNewRmlFile(file);
            }

            base.openEditor(file);

            LastRmlFile = file;
        }

        public void saveFile(String rml, String file)
        {
            saveText(file, rml);
            EditorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
        }

        public String LastRmlFile { get; private set; }

        public override ProjectItemTemplate createItemTemplate()
        {
            return new ProjectItemTemplateDelegate("Rml File", Icon, delegate(String path, String fileName, EditorController editorController)
            {
                String filePath = Path.Combine(path, fileName);
                filePath = Path.ChangeExtension(filePath, ".rml");
                if (EditorController.ResourceProvider.exists(filePath))
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
            });
        }

        void createNewRmlFile(String filePath)
        {
            creatingNewFile(filePath);
            saveText(filePath, defaultRml);
            openEditor(filePath);
        }

        private const String defaultRml = @"<rml>
	<head>
		<link type=""text/template"" href=""/MasterTemplate.trml"" />
	</head>
	<body template=""MasterTemplate"">
        <h1>Empty Rml View</h1>
        <p style=""white-space: pre-wrap;"">You can start creating your Rml View here. You can erase this text to start.</p>
    </body>
</rml>
";
    }
}
