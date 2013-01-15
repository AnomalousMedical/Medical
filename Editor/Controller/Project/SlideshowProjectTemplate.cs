using Engine.Saving;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Medical.GUI
{
    class SlideshowProjectTemplate : ProjectTemplate
    {
        private const String MvcContextName = "MvcContext.mvc";

        public void createProject(EditorResourceProvider resourceProvider, string projectName)
        {
            EmbeddedResourceHelpers.CopyResourceToStream(EmbeddedTemplateNames.SimpleMvcContext_mvc, MvcContextName, resourceProvider);

            DDAtlasPlugin ddPlugin = new DDAtlasPlugin();
            ddPlugin.PluginName = projectName;
            ddPlugin.PluginNamespace = projectName;
            StartAnomalousMvcTask mvcTask = new StartAnomalousMvcTask("Task", projectName, "", "Slideshows");
            mvcTask.ContextFile = MvcContextName;
            ddPlugin.addTask(mvcTask);
            saveObject(ddPlugin, resourceProvider, "Plugin.ddp");

            Assembly editorAssembly = Assembly.GetExecutingAssembly();

            EmbeddedResourceHelpers.CopyResourceToStream(EmbeddedTemplateNames.MasterTemplate_trml, "MasterTemplate.trml", resourceProvider);
            EmbeddedResourceHelpers.CopyResourceToStream(EmbeddedTemplateNames.Wysiwyg_rcss, "Wysiwyg.rcss", resourceProvider);
            EmbeddedResourceHelpers.CopyResourceToStream(EmbeddedTemplateNames.SlideshowMvcContext.File, "MvcContext.mvc", resourceProvider);

            using (StreamWriter streamWriter = new StreamWriter(resourceProvider.openWriteStream("Index.slides")))
            {
                streamWriter.Write("This file a slideshow makes");
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
    }
}
