using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;
using Engine.Saving.XMLSaver;
using System.Xml;
using System.IO;

namespace Medical
{
    class MVCGenericEditorObject : GenericEditorObject
    {
        private XmlSaver saver = new XmlSaver();
        private AnomalousMvcContext currentContext;

        public void createNew()
        {
            currentContext = new AnomalousMvcContext();
        }

        public EditInterface getEditInterface()
        {
            return currentContext.getEditInterface();
        }

        public void save(Stream stream)
        {
            using (XmlTextWriter xmlWriter = new XmlTextWriter(stream, Encoding.Default))
            {
                xmlWriter.Formatting = Formatting.Indented;
                saver.saveObject(currentContext, xmlWriter);
            }
        }

        public bool load(Stream stream)
        {
            try
            {
                using (XmlTextReader xmlReader = new XmlTextReader(stream))
                {
                    currentContext = (AnomalousMvcContext)saver.restoreObject(xmlReader);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string ObjectTypeName
        {
            get
            {
                return "MVC";
            }
        }

        public String FileWildcard
        {
            get
            {
                return "Anomalous MVC Context (*.mvc)|*.mvc;";
            }
        }
    }
}
