using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using System.Xml;
using System.IO;
using Medical.Presentation;
using Medical;

namespace PresentationEditor
{
    class PresentationProjectTemplate : ProjectTemplate
    {
        public void createProject(EditorResourceProvider resourceProvider, string projectName)
        {
            String mvcContextName = "MvcContext.mvc";

            DDAtlasPlugin ddPlugin = new DDAtlasPlugin();
            ddPlugin.PluginName = projectName;
            ddPlugin.PluginNamespace = projectName;
            StartAnomalousMvcTask mvcTask = new StartAnomalousMvcTask("Task", projectName, "", "Apps");
            mvcTask.ContextFile = mvcContextName;
            ddPlugin.addTask(mvcTask);
            saveObject(ddPlugin, resourceProvider, "Plugin.ddp");

            using (StreamWriter streamWriter = new StreamWriter(resourceProvider.openWriteStream("MasterTemplate.trml")))
            {
                streamWriter.Write(TRmlTypeController.DefaultMasterPage);
            }

            PresentationIndex presentationIndex = new PresentationIndex();
            PresentationController.AddSlide(presentationIndex, resourceProvider);
            saveObject(presentationIndex, resourceProvider, getDefaultFileName(projectName));
        }

        public String getDefaultFileName(String projectName)
        {
            return projectName + ".amp";
        }

        private void saveObject(Saveable saveable, EditorResourceProvider resourceProvider, String filename)
        {
            using (XmlTextWriter writer = new XmlTextWriter(resourceProvider.openWriteStream(filename), Encoding.Default))
            {
                writer.Formatting = Formatting.Indented;
                EditorController.XmlSaver.saveObject(saveable, writer);
            }
        }
    }
}
