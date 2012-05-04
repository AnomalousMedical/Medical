using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;
using Engine.Saving.XMLSaver;
using System.Xml;

namespace Medical
{
    class MVCGenericEditorObject : GenericSaveableEditorObject
    {
        private AnomalousMvcContext currentContext;

        public void createNew()
        {
            currentContext = new AnomalousMvcContext();
        }

        public EditInterface getEditInterface()
        {
            return currentContext.getEditInterface();
        }

        public void save(XmlSaver saver, XmlTextWriter xmlWriter)
        {
            saver.saveObject(currentContext, xmlWriter);
        }

        public bool load(XmlSaver saver, XmlReader xmlReader)
        {
            try
            {
                currentContext = (AnomalousMvcContext)saver.restoreObject(xmlReader);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void unloaded()
        {
            
        }

        public string ObjectTypeName
        {
            get
            {
                return "MVC";
            }
        }
    }
}
