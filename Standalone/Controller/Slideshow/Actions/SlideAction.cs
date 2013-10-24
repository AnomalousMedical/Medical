using Engine.Editing;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public abstract class SlideAction : Saveable
    {
        public SlideAction()
        {

        }

        public abstract EditInterface getEditInterface();

        public abstract void addToController(MvcController controller);

        public abstract String Name { get; set; }

        protected SlideAction(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info);
        }

        public void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info);
        }
    }
}
