using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Medical
{
    class DependencyProjectTemplate : ProjectTemplate
    {
        public string createProject(EditorResourceProvider resourceProvider, string projectName)
        {
            DDAtlasDependency dependency = new DDAtlasDependency()
            {
                DependencyNamespace = projectName,
                VersionString = "1.0.0.0",
                DependencyId = Guid.NewGuid()
            };

            saveObject(dependency, resourceProvider, "Dependency.ddd");

            return null;
        }

        private void saveObject(Saveable saveable, EditorResourceProvider resourceProvider, String filename)
        {
            using (XmlTextWriter writer = new XmlTextWriter(resourceProvider.openWriteStream(filename), Encoding.Unicode))
            {
                writer.Formatting = Formatting.Indented;
                EditorController.XmlSaver.saveObject(saveable, writer);
            }
        }
    }
}
