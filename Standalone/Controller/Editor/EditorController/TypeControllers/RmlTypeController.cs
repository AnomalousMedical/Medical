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
        public event Action<RmlTypeController, String> FileCreated;

        public const String Icon = "EditorFileIcon/.rml";

        public RmlTypeController(EditorController editorController)
            :base(".rml", editorController)
        {
            
        }

        public override void openFile(string file)
        {
            if (!EditorController.ResourceProvider.exists(file))
            {
                createNewRmlFile(file);
            }

            base.openFile(file);

            LastRmlFile = file;
        }

        public void saveFile(String rml, String file)
        {
            saveText(file, rml);
            EditorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
        }

        public String LastRmlFile { get; private set; }

        public override void addCreationMethod(ContextMenu contextMenu, string path, bool isDirectory, bool isTopLevel)
        {
            contextMenu.add(new ContextMenuItem("Create Rml File", path, delegate(ContextMenuItem item)
            {
                InputBox.GetInput("Rml File Name", "Enter a name for the rml file.", true, delegate(String result, ref String errorMessage)
                {
                    String filePath = Path.Combine(path, result);
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
                    return true;
                });
            }));
        }

        void createNewRmlFile(String filePath)
        {
            Timeline timeline = new Timeline();
            creatingNewFile(filePath);
            saveText(filePath, defaultRml);
            openFile(filePath);
            if (FileCreated != null)
            {
                FileCreated.Invoke(this, filePath);
            }
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
