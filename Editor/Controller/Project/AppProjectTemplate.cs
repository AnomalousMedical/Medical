using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using System.Xml;
using System.IO;
using System.Reflection;

namespace Medical
{
    class AppProjectTemplate : ProjectTemplate
    {
        private const String MvcContextName = "MvcContext.mvc";

        public void createProject(EditorResourceProvider resourceProvider, string projectName)
        {
            EmbeddedResourceHelpers.CopyResourceToStream(EmbeddedTemplateNames.SimpleMvcContext_mvc, MvcContextName, resourceProvider, Assembly.GetExecutingAssembly());

            DDAtlasPlugin ddPlugin = new DDAtlasPlugin();
            ddPlugin.PluginName = projectName;
            ddPlugin.PluginNamespace = projectName;
            StartAnomalousMvcTask mvcTask = new StartAnomalousMvcTask("Task", projectName, "", "Apps");
            mvcTask.ContextFile = MvcContextName;
            ddPlugin.addTask(mvcTask);
            saveObject(ddPlugin, resourceProvider, "Plugin.ddp");

            resourceProvider.createDirectory("", "Timeline");
            resourceProvider.createDirectory("", "Resources");

            Assembly editorAssembly = Assembly.GetExecutingAssembly();

            EmbeddedResourceHelpers.CopyResourceToStream(EmbeddedTemplateNames.MasterTemplate_trml, "MasterTemplate.trml", resourceProvider, Assembly.GetExecutingAssembly());
            EmbeddedResourceHelpers.CopyResourceToStream(EmbeddedTemplateNames.Wysiwyg_rcss, "Wysiwyg.rcss", resourceProvider, Assembly.GetExecutingAssembly());

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
        <p>This is the first view for the {0} app. You can modify this file as much as you want to create your view.</p>
    </body>
</rml>";
    }
}
