using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Medical
{
    public static class EmbeddedResourceHelpers
    {
        public static void CopyResourceToStream(String embeddedResource, String destinationFile, EditorResourceProvider resourceProvider)
        {
            CopyResourceToStream(embeddedResource, destinationFile, resourceProvider, Assembly.GetExecutingAssembly());
        }

        public static void CopyResourceToStream(String embeddedResource, String destinationFile, EditorResourceProvider resourceProvider, Assembly assembly)
        {
            using (Stream stream = resourceProvider.openWriteStream(destinationFile))
            {
                using (Stream resourceStream = assembly.GetManifestResourceStream(embeddedResource))
                {
                    resourceStream.CopyTo(stream);
                }
            }
        }

        public static String ReadResourceContents(String embeddedResource)
        {
            return ReadResourceContents(embeddedResource, Assembly.GetExecutingAssembly());
        }

        public static String ReadResourceContents(String embeddedResource, Assembly assembly)
        {
            using (StreamReader reader = new StreamReader(assembly.GetManifestResourceStream(embeddedResource)))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
