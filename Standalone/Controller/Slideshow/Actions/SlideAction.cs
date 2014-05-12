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

        /// <summary>
        /// Add the slide action to a given mvc controller.
        /// </summary>
        /// <param name="slide"></param>
        /// <param name="controller"></param>
        public abstract void addToController(Slide slide, MvcController controller);

        /// <summary>
        /// Setup a RunCommandsAction that works for this SlideAction.
        /// </summary>
        /// <param name="slide"></param>
        /// <param name="action"></param>
        public abstract void setupAction(Slide slide, RunCommandsAction action);

        public virtual void configureThumbnailProperties(ImageRendererProperties imageProperties)
        {
            
        }

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

        [DoNotSave]
        private bool allowPreview = false;
        /// <summary>
        /// Set this to true to allow a preview of the slide action.
        /// You must do this in all constructors including the deserializing
        /// constructor.
        /// </summary>
        public bool AllowPreview
        {
            get
            {
                return allowPreview;
            }
            set
            {
                allowPreview = value;
            }
        }

        protected SlideAction(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info);
        }

        public virtual void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info);
        }
    }
}
