using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public class DataModelItem : Saveable
    {
        public DataModelItem(String name)
        {
            Name = name;
        }

        public EditInterface getEditInterface()
        {
            return ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, Name, null);
        }

        public String Name { get; set; }

        [Editable]
        public String Value { get; set; }

        protected DataModelItem(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info, ReflectedSaver.DefaultScanner);
        }

        public void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info, ReflectedSaver.DefaultScanner); 
        }
    }
}
