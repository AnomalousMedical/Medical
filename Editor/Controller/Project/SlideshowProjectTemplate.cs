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
        private const String SlideshowName = "Slides.show";

        public void createProject(EditorResourceProvider resourceProvider, string projectName)
        {
            EmbeddedResourceHelpers.CopyResourceToStream(EmbeddedTemplateNames.SimpleMvcContext_mvc, SlideshowName, resourceProvider);

            DDAtlasPlugin ddPlugin = new DDAtlasPlugin();
            ddPlugin.PluginName = projectName;
            ddPlugin.PluginNamespace = projectName;
            StartSlideshowTask mvcTask = new StartSlideshowTask("Task", projectName, "", "Slideshows");
            mvcTask.SlideshowFile = SlideshowName;
            ddPlugin.addTask(mvcTask);
            saveObject(ddPlugin, resourceProvider, "Plugin.ddp");

            Assembly editorAssembly = Assembly.GetExecutingAssembly();

            EmbeddedResourceHelpers.CopyResourceToStream(EmbeddedTemplateNames.MasterTemplate_trml, "MasterTemplate.trml", resourceProvider);
            EmbeddedResourceHelpers.CopyResourceToStream(EmbeddedTemplateNames.Wysiwyg_rcss, "Wysiwyg.rcss", resourceProvider);

            Slideshow slideshow = new Slideshow();
            saveObject(slideshow, resourceProvider, SlideshowName);
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
