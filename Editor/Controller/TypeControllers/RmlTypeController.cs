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
    class RmlTypeController : TextTypeController
    {
        private EditorController editorController;
        private GUIManager guiManager;
        public const String Icon = "EditorFileIcon/.rml";
        private RmlEditorContext editorContext;

        public RmlTypeController(EditorController editorController, GUIManager guiManager)
            :base(".rml", editorController)
        {
            this.editorController = editorController;
            this.guiManager = guiManager;
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);
        }

        public override void openFile(string file)
        {
            if (!editorController.ResourceProvider.exists(file))
            {
                createNewRmlFile(file);
            }

            String rmlText = loadText(file);

            editorContext = new RmlEditorContext(rmlText, file, this);
            editorContext.Shutdown += new Action<RmlEditorContext>(editorContext_Shutdown);
            editorController.runEditorContext(editorContext.MvcContext);

            LastRmlFile = file;
        }

        public void saveFile(String rml, String file)
        {
            saveText(file, rml);
            editorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
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

        void editorContext_Shutdown(RmlEditorContext obj)
        {
            updateCachedText(obj.CurrentFile, obj.CurrentText);
            if (editorContext == obj)
            {
                editorContext = null;
            }
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            closeCurrentCachedResource();
            if (editorContext != null)
            {
                editorContext.close();
            }
        }

        void createNewRmlFile(String filePath)
        {
            Timeline timeline = new Timeline();
            creatingNewFile(filePath);
            saveText(filePath, defaultRml);
            openFile(filePath);
        }

        private const String defaultRml = @"<rml>
	<head>
		<link type=""text/template"" href=""/MasterTemplate.trml"" />
	</head>
	<body template=""MasterTemplate"">
        <h1>Empty Rml View</h1>
        <p>You can start creating your Rml View here. You can erase this text to start.</p>
    </body>
</rml>
";
    }
}
