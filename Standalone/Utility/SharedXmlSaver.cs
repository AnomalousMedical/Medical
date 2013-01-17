using Engine.Saving;
using Engine.Saving.XMLSaver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Medical
{
    /// <summary>
    /// A shared xml saver for the program, not thread safe
    /// </summary>
    public static class SharedXmlSaver
    {
        private static XmlSaver xmlSaver = new XmlSaver();

        public static T Load<T>(Stream stream)
            where T : Saveable
        {
            using (XmlReader xmlReader = new XmlTextReader(stream))
            {
                T saveable = (T)xmlSaver.restoreObject(xmlReader);
                return saveable;
            }
        }

        public static void Save(Saveable save, Stream stream)
        {
            using (XmlTextWriter textWriter = new XmlTextWriter(stream, Encoding.Default))
            {
                textWriter.Formatting = Formatting.Indented;
                xmlSaver.saveObject(save, textWriter);
            }
        }
    }
}
