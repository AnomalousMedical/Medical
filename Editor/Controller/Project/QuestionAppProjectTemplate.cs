using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using System.Xml;
using System.IO;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;

namespace Medical
{
    class QuestionAppProjectTemplate : ProjectTemplate
    {
        private const String MvcContextName = "MvcContext.mvc";
        private const String TimelineName = "MainTimeline.tl";

        public void createProject(EditorResourceProvider resourceProvider, string projectName)
        {
            AnomalousMvcContext mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Startup";
            mvcContext.ShutdownAction = "Common/Shutdown";

            RmlView view = new RmlView("Index");
            view.RmlFile = "Index.rml";
            view.ViewLocation = ViewLocations.Left;
            view.IsWindow = false;
            view.Buttons.add(new CloseButtonDefinition("Close", "Common/ExitApp"));
            mvcContext.Views.add(view);

            mvcContext.Controllers.add(new MvcController("Common",
                new RunCommandsAction("Startup",
                    new SaveCameraPositionCommand(),
                    new SaveLayersCommand(),
                    new SaveMusclePositionCommand(),
                    new SaveMedicalStateCommand(),
                    new HideMainInterfaceCommand(),
                    new RunActionCommand("Index/Show")),
                new RunCommandsAction("Shutdown",
                    new RestoreCameraPositionCommand(),
                    new RestoreLayersCommand(),
                    new RestoreMusclePositionCommand(),
                    new RestoreMedicalStateCommand(),
                    new ShowMainInterfaceCommand()
                ),
                new RunCommandsAction("ExitApp", 
                    new ShutdownCommand())
            ));

            mvcContext.Controllers.add(new MvcController("Index",
                new RunCommandsAction("Show",
                    new PlayTimelineCommand(TimelineName),
                    new ShowViewCommand("Index")
                )
            ));

            saveObject(mvcContext, resourceProvider, MvcContextName);

            DDAtlasPlugin ddPlugin = new DDAtlasPlugin();
            ddPlugin.PluginName = projectName;
            ddPlugin.PluginNamespace = projectName;
            StartAnomalousMvcTask mvcTask = new StartAnomalousMvcTask("Task", projectName, "", "Apps");
            mvcTask.ContextFile = MvcContextName;
            ddPlugin.addTask(mvcTask);
            saveObject(ddPlugin, resourceProvider, "Plugin.ddp");

            Timeline mainTimeline = new Timeline();
            saveObject(mainTimeline, resourceProvider, TimelineName);

            resourceProvider.createDirectory("", "Resources");

            EmbeddedResourceHelpers.CopyResourceToStream(EmbeddedTemplateNames.MasterTemplate_trml, "MasterTemplate.trml", resourceProvider);
            EmbeddedResourceHelpers.CopyResourceToStream(EmbeddedTemplateNames.Wysiwyg_rcss, "Wysiwyg.rcss", resourceProvider);

            using (StreamWriter streamWriter = new StreamWriter(resourceProvider.openWriteStream("Index.rml")))
            {
                streamWriter.Write(indexRml, projectName);
            }
        }

        public String getDefaultFileName(String projectName)
        {
            return null;
        }

        private void saveObject(Saveable saveable, EditorResourceProvider resourceProvider, String filename)
        {
            using (XmlTextWriter writer = new XmlTextWriter(resourceProvider.openWriteStream(filename), Encoding.Default))
            {
                writer.Formatting = Formatting.Indented;
                EditorController.XmlSaver.saveObject(saveable, writer);
            }
        }

        private static String indexRml = @"<rml>
	<head>
	    <link type=""text/template"" href=""/MasterTemplate.trml"" />
	</head>
	<body template=""MasterTemplate"">
        <h1>{0}</h1>
        <p>This is the first view for the {0} question app. You can modify this file as much as you want to create your view.</p>
    </body>
</rml>";
    }
}
