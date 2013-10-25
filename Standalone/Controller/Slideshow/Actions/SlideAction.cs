using Engine.Attributes;
using Engine.Editing;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.SlideshowActions
{
    public abstract class SlideAction : Saveable
    {
        [DoNotSave]
        public event Action<SlideAction> ChangesMade;

        public SlideAction()
        {

        }

        public abstract EditInterface getEditInterface();

        public abstract void addToController(Slide slide, MvcController controller);

        public virtual void cleanup(Slide slide, CleanupInfo info, ResourceProvider resourceProvider)
        {

        }

        protected void fireChangesMade()
        {
            if (ChangesMade != null)
            {
                ChangesMade.Invoke(this);
            }
        }

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
