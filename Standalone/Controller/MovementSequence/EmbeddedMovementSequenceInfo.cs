using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Muscles;
using Engine.Saving.XMLSaver;
using System.IO;
using Logging;
using System.Reflection;
using System.Xml;

namespace Medical.Controller
{
    public class EmbeddedMovementSequenceInfo : MovementSequenceInfo
    {
        public EmbeddedMovementSequenceInfo(Assembly assembly, String name, String embeddedResourceName)
        {
            this.Name = name;
            this.EmbeddedResourceName = embeddedResourceName;
            this.Assembly = assembly;
        }

        public String EmbeddedResourceName { get; set; }

        public Assembly Assembly { get; set; }

        public override MovementSequence loadSequence(XmlSaver xmlSaver)
        {
            MovementSequence loadingSequence = null;
            try
            {
                using (XmlTextReader xmlReader = new XmlTextReader(Assembly.GetManifestResourceStream(EmbeddedResourceName)))
                {
                    loadingSequence = xmlSaver.restoreObject(xmlReader) as MovementSequence;
                    loadingSequence.Name = Name;
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not read muscle sequence {0}.\nReason: {1}.", EmbeddedResourceName, e.Message);
            }
            return loadingSequence;
        }
    }
}
