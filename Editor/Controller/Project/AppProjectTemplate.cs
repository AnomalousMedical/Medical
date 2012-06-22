using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using System.Xml;
using System.IO;

namespace Medical
{
    class AppProjectTemplate : ProjectTemplate
    {
        public void createProject(EditorResourceProvider resourceProvider, string projectName)
        {
            String mvcContextName = "MvcContext.mvc";

            using (Stream writeStream = resourceProvider.openWriteStream(mvcContextName))
            {
                using (Stream resourceStream = GetType().Assembly.GetManifestResourceStream("Medical.Controller.Project.SimpleMvcContext.mvc"))
                {
                    resourceStream.CopyTo(writeStream, 4096);
                }
            }

            DDAtlasPlugin ddPlugin = new DDAtlasPlugin();
            ddPlugin.PluginName = projectName;
            ddPlugin.PluginNamespace = projectName;
            StartAnomalousMvcTask mvcTask = new StartAnomalousMvcTask("Task", projectName, "", "Apps");
            mvcTask.ContextFile = mvcContextName;
            ddPlugin.addTask(mvcTask);
            saveObject(ddPlugin, resourceProvider, "Plugin.ddp");

            resourceProvider.createDirectory("", "Timeline");
            resourceProvider.createDirectory("", "Resources");

            using (StreamWriter streamWriter = new StreamWriter(resourceProvider.openWriteStream("MasterTemplate.trml")))
            {
                streamWriter.Write(TRmlTypeController.DefaultMasterPage);
            }

            using (StreamWriter streamWriter = new StreamWriter(resourceProvider.openWriteStream("Index.rml")))
            {
                streamWriter.Write(indexRml, projectName);
            }
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
